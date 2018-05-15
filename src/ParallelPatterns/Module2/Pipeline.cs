using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ParallelPatterns.Fsharp;

namespace ParallelPatterns
{
    public class Pipeline<TInput, TOutput>  
    {
        readonly Func<TInput, Task<TOutput>> _processoTask;
        readonly Func<TInput, TOutput> _processor;
        readonly BlockingCollection<TInput>[] _input;
        readonly CancellationToken _token;
        private const int count = 3;

        private Pipeline(
            Func<TInput, Task<TOutput>> processor,
            BlockingCollection<TInput>[] input = null,
            CancellationToken token = new CancellationToken())
        {
            _input = input != null
                ? input
                : Enumerable.Range(0, count - 1).Select(_ => new BlockingCollection<TInput>(10)).ToArray();
            Output = new BlockingCollection<TOutput>[_input.Length];
            for (int i = 0; i < Output.Length; i++)
                Output[i] = null == _input[i] ? null : new BlockingCollection<TOutput>(count);
            _processoTask = processor;
            _token = token;
            Task.Factory.StartNew(Run, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private Pipeline(
            Func<TInput, TOutput> processor,
            BlockingCollection<TInput>[] input = null,
            CancellationToken token = new CancellationToken())
        {
            _input = input ?? Enumerable.Range(0, count - 1).Select(_ => new BlockingCollection<TInput>(10)).ToArray();
            Output = new BlockingCollection<TOutput>[_input.Length];
            for (int i = 0; i < Output.Length; i++)
                Output[i] = null == _input[i] ? null : new BlockingCollection<TOutput>(count);
            _processor = processor;
            _token = token;
            Task.Factory.StartNew(Run, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static Pipeline<TInput, TOutput> Create(
            Func<TInput, TOutput> processor,
            CancellationToken token = new CancellationToken()) =>
            new Pipeline<TInput, TOutput>(processor, token: token);

        public static Pipeline<TInput, TOutput> Create(
            Func<TInput, Task<TOutput>> processor,
            CancellationToken token = new CancellationToken()) =>
            new Pipeline<TInput, TOutput>(processor, token: token);
        
        private BlockingCollection<TOutput>[] Output { get; }

        // TODO (3.a)
        public Pipeline<TOutput, TMid> Then<TMid>(Func<TOutput, Task<TMid>> project,
            CancellationToken token = new CancellationToken())
        {
            // Add code implementation
            return default(Pipeline<TOutput, TMid>);
        }

        public Pipeline<TOutput, TMid> Then<TMid>(Func<TOutput, TMid> project,
            CancellationToken token = new CancellationToken())
        {
            // Add code implementation
            return default(Pipeline<TOutput, TMid>);
        }
        
        // TODO (3.b)
        public void Engueue(TInput item)
        {
          // Add code implementation
        }

        private async Task Run()
        {
            SpinWait sw = new SpinWait();
            while (!_input.All(bc => bc.IsCompleted) && !_token.IsCancellationRequested)
            {
                TInput receivedItem;
                int i = BlockingCollection<TInput>.TryTakeFromAny(_input, out receivedItem, 50, _token);
                if (i >= 0)
                {
                    TOutput outputItem =
                        _processor != null ? _processor(receivedItem) : await _processoTask(receivedItem);
                    BlockingCollection<TOutput>.AddToAny(Output, outputItem);
                    sw.SpinOnce();
                }
                else
                {
                    Thread.SpinWait(1000);
                }
            }

            if (Output != null)
            {
                foreach (var bc in Output) bc.CompleteAdding();
            }
        }
    }
}