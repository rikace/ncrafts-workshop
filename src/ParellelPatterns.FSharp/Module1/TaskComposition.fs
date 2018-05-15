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
        static member Then (input : Task<'T>, binder :Func<'T, Task<'U>>) = ()
    
        // TODO (1) 
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, Task<'U>>, projection:Func<'T, 'U, 'R>) = ()
    
        // TODO (1) 
        // implement missing code
        [<Extension>]
        static member Then (input : Task<'T>, binder :Func<'T, 'U>) =  ()
            
    
        [<Extension>]        
        static member Return value : Task<'T> = Task.FromResult<'T> (value)  
      
     
            