namespace ParallelPatterns.Fsharp
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices


module TaskComposition =

    [<Extension>]
    type TaskExt() =

        // TODO (1)
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, Task<'U>>) =
            let tcs = new TaskCompletionSource<'U>()
            input.ContinueWith(fun (task:Task<'T>) ->
               if (task.IsFaulted) then
                    tcs.SetException(task.Exception.InnerExceptions)
               elif (task.IsCanceled) then tcs.SetCanceled()
               else
                    try
                       (binder.Invoke(task.Result)).ContinueWith(
                            fun(nextTask:Task<'U>) -> tcs.SetResult(nextTask.Result))
                       |> ignore
                    with
                    | ex -> tcs.SetException(ex)) |> ignore
            tcs.Task

        // TODO (1)
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, Task<'U>>, projection:Func<'T, 'U, 'R>) =
            TaskExt.Then(input, fun outer ->
                TaskExt.Then(binder.Invoke(outer), fun inner ->
                    Task.FromResult(projection.Invoke(outer, inner))))

        // TODO (1)
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, 'U>) =
            let tcs = new TaskCompletionSource<'U>()
            input.ContinueWith(fun (task:Task<'T>) ->
               if task.IsFaulted then
                    tcs.SetException(task.Exception.InnerExceptions)
               elif task.IsCanceled then tcs.SetCanceled()
               else
                    try
                        tcs.SetResult(binder.Invoke(task.Result))
                    with
                    | ex -> tcs.SetException(ex)) |> ignore
            tcs.Task


        [<Extension>]
        static member Return value : Task<'T> = Task.FromResult<'T> (value)

        [<Extension>]
        static member Select (task : Task<'T>, selector : 'T -> 'U) : Task<'U> =
            let r = new TaskCompletionSource<'U>()
            task.ContinueWith(fun (t:Task<'T>) ->
                if t.IsFaulted then r.SetException(t.Exception.InnerExceptions)
                elif t.IsCanceled then r.SetCanceled()
                else r.SetResult(selector(t.Result));
                selector(t.Result)
            )
            r.Task

        [<Extension>]
        static member SelectMany (input : Task<'T>, binder : 'T -> Task<'U>) =
            let tcs = new TaskCompletionSource<'U>()
            input.ContinueWith(fun (task:Task<'T>) ->
               if task.IsFaulted then
                    tcs.SetException(task.Exception.InnerExceptions)
               elif task.IsCanceled then tcs.SetCanceled()
               else
                    try
                        let t = binder(task.Result)
                        if t = null then tcs.SetCanceled()
                        else
                            t.ContinueWith(fun(nextT:Task<'U>) ->
                                if nextT.IsFaulted then tcs.TrySetException(nextT.Exception.InnerExceptions);
                                elif nextT.IsCanceled then tcs.TrySetCanceled()
                                else tcs.TrySetResult(nextT.Result);
                            , TaskContinuationOptions.ExecuteSynchronously)
                            |> ignore
                    with
                    | ex -> tcs.SetException(ex)) |> ignore
            tcs.Task

        [<Extension>]
        static member SelectMany (input : Task<'T>, binder : 'T -> Task<'U>, projection: 'T -> 'U -> 'R) =
            TaskExt.SelectMany(input, fun outer ->
                TaskExt.SelectMany(binder(outer), fun inner ->
                    Task.FromResult(projection outer inner)))

(**
        [<Extension>]
        static member SelectMany (input : Task<'T>, binder : 'T -> 'U) =
            let tcs = new TaskCompletionSource<'U>()
            input.ContinueWith(fun (task:Task<'T>) ->
               if (task.IsFaulted) then
                    tcs.SetException(task.Exception.InnerExceptions)
               elif (task.IsCanceled) then tcs.SetCanceled()
               else
                    try
                        tcs.SetResult(binder task.Result)
                    with
                    | ex -> tcs.SetException(ex)) |> ignore
            tcs.Task
*)