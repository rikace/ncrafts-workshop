namespace ParallelPatterns.Fsharp
open System
open System.Threading
open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices

module ReactiveAgent =  

    // Subject state maintained inside of the mailbox loop
    type State<'T> = {
        observers : IObserver<'T> list
        stopped   : bool
    }
    with static member empty() = {observers=[]; stopped=false}

    // Messages required for the mailbox loop
    type Message<'T, 'U> =
    | Add       of IObserver<'U>
    | Remove    of IObserver<'U>
    | Next      of 'T
    | Error     of exn
    | Completed

    type AgentObservable<'T, 'U>(seed:'U, selector:Func<'U, 'T, 'U>) =

        let error() = raise(new System.InvalidOperationException("Subject already completed"))

        let mbox = Agent<_>.Start(fun inbox ->
            let rec loop(t:State<_>) acc = async {
                let! req = inbox.Receive()

                match req with
                | Message.Add(observer) ->
                    if not(t.stopped) then
                        return! loop ({t with observers = t.observers @ [observer]}) acc
                    else error()

                | Message.Remove(observer) ->
                    if not(t.stopped) then
                        let t = {t with observers = t.observers |> List.filter(fun f -> f <> observer)}
                        return! loop t acc
                    else error()

                | Message.Next(value) ->
                                 let newAcc = selector.Invoke(acc, value)
                                 if not(t.stopped) then
                                     t.observers |> List.iter(fun o -> o.OnNext(newAcc))
                                     return! loop t newAcc
                                 else error()

                | Message.Error(err) ->
                    if not(t.stopped) then
                        t.observers |> List.iter(fun o -> o.OnError(err))
                        return! loop t acc
                    else error()

                | Message.Completed ->
                    if not(t.stopped) then
                        t.observers |> List.iter(fun o -> o.OnCompleted())
                        let t = {t with stopped = true}
                        return! loop t acc
                    else error()
            }

            loop (State<'U>.empty()) seed)

        /// Raises OnNext in all the observers
        member x.Next value  = Message.Next(value)  |> mbox.Post
        /// Raises OnError in all the observers
        member x.Error ex    = Message.Error(ex)    |> mbox.Post
        /// Raises OnCompleted in all the observers
        member x.Completed() = Message.Completed    |> mbox.Post
        
        // TODO 7.a
        // Implement both the reactive interfaces IObserver and IObservable, also pay attention to the generic
        // type constarctor for these intefaces, they might not be the same.
        // Then, expose the IObservable interface throughout an instance method called "AsObservable". This methid will be used
        // to register the output after each message is processed.
               
        // Add code missing for the implementation of the  IObserver and IObservable interfaces 
        
        member x.AsObservable() = Unchecked.defaultof<IObservable<_>> // TODO complete this methid after the implementation of the IObservable interface                  
        
        interface IAgent<'T, 'U> with 
            member x.Post(msg) = Message.Next(msg)  |> mbox.Post                   
            member x.Send(msg) = async { Message.Next(msg)  |> mbox.Post } |> Async.startAsPlainTask                   
            member x.AsObservable() = x.AsObservable()                   
                                
        