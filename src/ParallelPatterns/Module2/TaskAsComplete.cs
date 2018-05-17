using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelPatterns.TaskComposition
{
    public static class TaskAsComplete
    { 
        public static IEnumerable<Task<R>> ContinueAsComplete<T, R>(
            this IEnumerable<T> input, 
            Func<T, Task<R>> selector)
        {
            var inputTaskList = (from el in input select selector(el)).ToList();
 
            var completionSourceList = new List<TaskCompletionSource<R>>(inputTaskList.Count);
            for (var i = 0; i < inputTaskList.Count; i++)
                completionSourceList.Add(new TaskCompletionSource<R>());

            int prevIndex = -1;

            // TODO 4
            Action<Task<R>> continuation = completedTask =>
            {
                int index = Interlocked.Increment(ref prevIndex);
                var source = completionSourceList[index];

                switch (completedTask.Status)
                {
                    case TaskStatus.Canceled:
                        source.TrySetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        source.TrySetException(completedTask.Exception.InnerExceptions);
                        break;
                    default:
                        source.TrySetResult(completedTask.Result);
                        break;
                }
            };
            
            foreach (var inputTask in inputTaskList)
            {
                inputTask.ContinueWith(continuation,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }

            return completionSourceList.Select(source => source.Task);
        }

    }
}