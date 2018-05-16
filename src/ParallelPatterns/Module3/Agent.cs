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
        public static IAgent<TMessage, TState> Start<TMessage, TState>(TState initialState,
            Func<TState, TMessage, Task<TState>> action, CancellationTokenSource cts = null)
            => new Agent<TMessage, TState>(initialState, action, cts);

        public static IAgent<TMessage, TState> Start<TMessage, TState>(TState initialState,
            Func<TState, TMessage, TState> action, CancellationTokenSource cts = null)
            => new Agent<TMessage, TState>(initialState, action, cts);

        public static IDisposable LinkTo<TOutput, TState>(this ISourceBlock<TOutput> source,
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

            // Add code implementation

            // Remember, this is a stateful agent
        }

        public Agent(
            TState initialState,
            Func<TState, TMessage, Task<TState>> action,
            CancellationTokenSource cts = null)
        {
            // TODO (7.a)

            // Add code implementation
        }


        public Task Send(TMessage message)
            => _actionBlock.SendAsync(message);

        public void Post(TMessage message)
            => _actionBlock.Post(message);

        public IObservable<TState> AsObservable()
            => _actionBlock.AsObservable();
    }
}