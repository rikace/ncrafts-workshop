using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelPatterns.Common.TaskEx
{
    public static class TaskEx
    {
        public static Task<R[]> Traverse<T, R>(this IEnumerable<T> collection, Func<T, Task<R>> projection)
            => Task.WhenAll(collection.Select(projection).ToArray());

        public static Task<R[]> Traverse<T, R>(this IEnumerable<T> collection, Func<T, R> projection)
            => Task.FromResult(collection.Select(projection).ToArray());
        
        public static void FromTask<TResult, TTaskResult>(
            this TaskCompletionSource<TResult> tcs, Task<TTaskResult> task, Func<TTaskResult, TResult> resultSelector)
        {
            if (task.Status == TaskStatus.Faulted)
            {
                var ae = task.Exception;
                var targetException = ae.InnerExceptions.Count == 1 ? ae.InnerExceptions[0] : ae;
                tcs.TrySetException(targetException);
            }
            else if (task.Status == TaskStatus.Canceled)
                tcs.TrySetCanceled();
            else if (task.Status == TaskStatus.RanToCompletion)
                tcs.TrySetResult(resultSelector(task.Result));
            else
                throw new InvalidOperationException($"Task should be in one of the final states! Current state: {task.Status.ToString()}");
        }
    }
}