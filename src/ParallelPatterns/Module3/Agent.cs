using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ParallelPatterns.Common;
using ParallelPatterns.Fsharp;
using static ParallelPatterns.Common.OptionHelpers;
using static ParallelPatterns.Fsharp.Interfaces;

namespace ParallelPatterns
{
    public static class Agent
    {
        public static IAgent<TMessage, TState> Start<TMessage, TState>(
            TState initialState,
            Func<TState, TMessage, Task<TState>> action, 
            CancellationTokenSource cts = null)
            => new Agent<TMessage, TState>(initialState, action, cts);

        public static IAgent<TMessage, TState> Start<TMessage, TState>(
            TState initialState,
            Func<TState, TMessage, TState> action, 
            CancellationTokenSource cts = null)
            => new Agent<TMessage, TState>(initialState, action, cts);

        public static IReplyAgent<TMessage, TReply> Start<TState, TMessage, TReply>(
            TState initialState,
            Func<TState, TMessage, Task<TState>> projection, 
            Func<TState, TMessage, Task<(TState, TReply)>> ask,
            CancellationTokenSource cts = null)
            => new AgentReply<TState, TMessage, TReply>(initialState, projection, ask, cts);

        public static IReplyAgent<TMessage, TReply> Start<TState, TMessage, TReply>(
            TState initialState,
            Func<TState, TMessage, TState> projection, 
            Func<TState, TMessage, (TState, TReply)> ask,
            CancellationTokenSource cts = null)
            => new AgentReply<TState, TMessage, TReply>(initialState, projection, ask, cts);

        public static IDisposable LinkTo<TOutput, TState>(
            this ISourceBlock<TOutput> source,
            IAgent<TOutput, TState> agent)
            => source.AsObservable().Subscribe(agent.Post);
    }

    public class Agent<TMessage, TState> : IAgent<TMessage, TState>
    {
        private TState _state;
        private readonly TransformBlock<TMessage, TState> _actionBlock;
        
        public Agent(
            TState initialState,
            Func<TState, TMessage, TState> action,
            CancellationTokenSource cts = null)
        {
            // TODO (7.a)
            // Implement Agent
            // Initialize local isolated state
            // Create Dataflow-block that receives and processes the messages, and then update the local state
            
            _state = initialState;
            var options = new ExecutionDataflowBlockOptions
            {
                CancellationToken = cts?.Token ?? CancellationToken.None
            };
            _actionBlock = new TransformBlock<TMessage, TState>(
                msg => _state = action(_state, msg)
                , options);
        }

        public Agent(
            TState initialState,
            Func<TState, TMessage, Task<TState>> action,
            CancellationTokenSource cts = null)
        {
            // TODO (7.a) 

            _state = initialState;
            var options = new ExecutionDataflowBlockOptions
            {
                CancellationToken = cts?.Token ?? CancellationToken.None
            };
            _actionBlock = new TransformBlock<TMessage, TState>(
                async msg => _state = await action(_state, msg)
                , options);
        }


        public Task Send(TMessage message) 
            => _actionBlock.SendAsync(message);
        public void Post(TMessage message) 
            => _actionBlock.Post(message);
        public IObservable<TState> AsObservable() 
            => _actionBlock.AsObservable();
    }



    public class AgentReply<TState, TMessage, TReply> : IReplyAgent<TMessage, TReply>
    {
        private TState _state;

        private readonly ActionBlock<(TMessage,
            Option<TaskCompletionSource<TReply>>)> _actionBlock;
        
        public AgentReply(TState initialState,
            Func<TState, TMessage, TState> projection,
            Func<TState, TMessage, (TState, TReply)> ask,
            CancellationTokenSource cts = null)
        {
            _state = initialState;
            var options = new ExecutionDataflowBlockOptions
            {
                CancellationToken = cts?.Token ?? CancellationToken.None
            };
            
            _actionBlock = new ActionBlock<(TMessage, Option<TaskCompletionSource<TReply>>)>(
                message =>
                {
                    (TMessage msg, Option<TaskCompletionSource<TReply>> replyOpt) = message;
                    replyOpt.Match(
                        none: () => (_state = projection(_state, msg)),
                        some: reply =>
                        {
                            (TState newState, TReply replyResult) = ask(_state, msg);
                            reply.SetResult(replyResult);
                            return _state = newState;
                        });
                }, options);
        }

        public AgentReply(TState initialState,
            Func<TState, TMessage, Task<TState>> projection,
            Func<TState, TMessage, Task<(TState, TReply)>> ask,
            CancellationTokenSource cts = null)
        {
            _state = initialState;
            var options = new ExecutionDataflowBlockOptions
            {
                CancellationToken = cts?.Token ?? CancellationToken.None
            };
            
            _actionBlock = new ActionBlock<(TMessage, Option<TaskCompletionSource<TReply>>)>(
                async message =>
                {
                    (TMessage msg, Option<TaskCompletionSource<TReply>> replyOpt) = message;
                    await replyOpt.Match(
                        none: async () => _state = await projection(_state, msg),
                        some: async reply =>
                        {
                            (TState newState, TReply replyResult) = await ask(_state, msg);
                            reply.SetResult(replyResult);
                            return _state = newState;
                        });
                }, options);
        }

        
        public Task<TReply> Ask(TMessage message)
        {
            var tcs = new TaskCompletionSource<TReply>();
            _actionBlock.Post((message, Some(tcs)));
            return tcs.Task;
        }

        public Task Send(TMessage message) =>
            _actionBlock.SendAsync((message, None));

        public void Post(TMessage message) =>
            _actionBlock.Post((message, None));
    }
}