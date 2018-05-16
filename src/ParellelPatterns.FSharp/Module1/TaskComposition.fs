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
            Unchecked.defaultof<Task<'U>>

        // TODO (1)
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, Task<'U>>, projection:Func<'T, 'U, 'R>) =
            Unchecked.defaultof<Task<'R>>

        // TODO (1)
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, 'U>) =
           Unchecked.defaultof<Task<'U>>

