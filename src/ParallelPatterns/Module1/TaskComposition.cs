using System;
using System.Threading.Tasks;

namespace ParallelPatterns.TaskComposition
{
    public static class TaskComposition
    {
        // TODO (1) 
        // implement missing code
        public static Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, Task<TOut>> next)
        {
            // Add code implementation
            return Task.FromResult(default(TOut));
        }

        // TODO (1) 
        // implement missing code
        public static Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> next)
        {
            // Add code implementation
            return Task.FromResult(default(TOut));
        }
    }
}