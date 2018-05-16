using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using ParallelPatterns.Fsharp;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FuzzyMatch;
using ParallelPatterns.Common;
using ParallelPatterns.Common.TaskEx;
using static FuzzyMatch.JaroWinklerModule.FuzyMatchStructures;
using static ParallelPatterns.Common.FuzzyMatchHelpers;

namespace ParallelPatterns
{
    public partial class ParallelFuzzyMatch
    {
        public static async Task RunFuzzyMatchDataFlow(string[] wordsLookup, IList<string> files)
        {
            var cts = new CancellationTokenSource();
            var opt = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 10,
                MaxDegreeOfParallelism = 1,
                CancellationToken = cts.Token
            };

            int fileCount = files.Count;

            var inputBlock = new BufferBlock<string>(opt);

            var readLinesBlock =
                new TransformBlock<string, string>(
                    async file => await File.ReadAllTextAsync(file, cts.Token), opt);

            var splitWordsBlock = 
                new TransformBlock<string, HashSet<string>>(
                    text => WordRegex.Value.Split(text).Where(w => !IgnoreWords.Contains(w)).AsSet(), opt);

            var batch = 
                new BatchBlock<HashSet<string>>(fileCount);

            var foundMatchesBlock =
                new TransformBlock<HashSet<string>[], WordDistanceStruct[]>(
                    async wordSet =>
                    {
                        var wordSetFlatten = wordSet.Flatten().AsSet();
                        var matches =
                            await wordsLookup.Traverse(wl =>
                                JaroWinklerModule.bestMatchTask(wordSetFlatten, wl, threshold));
                        return matches.Flatten().ToArray();
                    }, opt);


            // TODO (5)
            // Implement a block name "printBlock", which prints the output of the foundMatchesBlock using the "PrintSummary" method 
            // Then link the block to the "foundMatchesBlock" block
            
            /* code here */
            
            // var printBlock = ...
            
            // TODO (6)
            // After have completed TODO (5), remove or unlink the printBlock, and replace the output of the "foundMatchesBlock" block 
            // with Reactive Extensions "AsObservable", maintaining the call to the "PrintSummary" method 
            
            /* code here */ 
            // foundMatchesBlock ...  
            
            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};

            IDisposable disposeAll = new CompositeDisposable(
                inputBlock.LinkTo(readLinesBlock, linkOptions),
                readLinesBlock.LinkTo(splitWordsBlock, linkOptions),
                splitWordsBlock.LinkTo(batch, linkOptions),
                batch.LinkTo(foundMatchesBlock, linkOptions)
                // foundMatchesBlock.LinkTo(printBlock)
            );
            cts.Token.Register(disposeAll.Dispose);

            foreach (var file in files)
                await inputBlock.SendAsync(file, cts.Token);

            inputBlock.Complete();
            await foundMatchesBlock.Completion.ContinueWith(_ => disposeAll.Dispose());
        }

        // C# example 
        public static async Task RunFuzzyMatchAgentCSharp(string[] wordsLookup, IList<string> files)
        {
            var cts = new CancellationTokenSource();
            var opt = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 10,
                MaxDegreeOfParallelism = 4,
                CancellationToken = cts.Token
            };

            var inputBlock = new BufferBlock<string>(opt);

            var readLinesBlock =
                new TransformBlock<string, string>(
                    async file => await File.ReadAllTextAsync(file, cts.Token), opt);

            var splitWordsBlock = 
                new TransformBlock<string, string[]>(
                    text => WordRegex.Value.Split(text).Where(w => !IgnoreWords.Contains(w)).AsSet().ToArray(), opt);

            var foundMatchesBlock =
                new TransformBlock<string[], WordDistanceStruct[]>(async wordSet =>
                {
                    var matches =
                        await wordsLookup.Traverse(wl => JaroWinklerModule.bestMatchTask(wordSet, wl, threshold));
                    return matches.Flatten().ToArray();
                }, opt);

            
            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            
            // TODO (7) (for C#)
            // Implement a stateful agent using the TPL Dataflow.
            // The Agent should have an internal state protected from external access.
            // The function passed in the constractor applies a project/reduce to the incoming messages and in the current state,
            // to return a new state
            // (see AgentAggregator.cs)

            /* code here
 
                var agent = Agent.Start  ... 
            */
            
            
            IDisposable disposeAll = new CompositeDisposable(
                inputBlock.LinkTo(readLinesBlock, linkOptions),
                readLinesBlock.LinkTo(splitWordsBlock, linkOptions),
                splitWordsBlock.LinkTo(foundMatchesBlock, linkOptions)

                // agent.AsObservable().Subscribe(summaryMathces =>
                //        PrintSummary(summaryMathces))
            );

            cts.Token.Register(disposeAll.Dispose);

            foreach (var file in files)
                await inputBlock.SendAsync(file, cts.Token);

            inputBlock.Complete();
            await foundMatchesBlock.Completion.ContinueWith(_ => 
                disposeAll.Dispose());
        }
        
        
        // F# example
        public static async Task RunFuzzyMatchAgentFSharp(string[] wordsLookup, IList<string> files)
        {
            var cts = new CancellationTokenSource();
            var opt = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 10,
                MaxDegreeOfParallelism = 4,
                CancellationToken = cts.Token
            };

            var inputBlock = new BufferBlock<string>(opt);

            var readLinesBlock =
                new TransformBlock<string, string>(
                    file => File.ReadAllTextAsync(file, cts.Token), opt);

            var splitWordsBlock = new TransformBlock<string, string[]>(
                text => WordRegex.Value.Split(text).Where(w => !IgnoreWords.Contains(w)).AsSet().ToArray(), opt);

            var foundMatchesBlock =
                new TransformBlock<string[], WordDistanceStruct[]>(async wordSet =>
                {
                    var matches =
                        await wordsLookup.Traverse(wl => JaroWinklerModule.bestMatchTask(wordSet, wl, threshold));
                    return matches.Flatten().ToArray();
                }, opt);

            
            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            
            // TODO (7) (for F#)
            // Implement a Reactive MailboxProcessor in F#.
            // Go to the Fsharp project, Module 3 and follow the instructions (7.a)
            // then, uncomment the following code and remove the previous code that uses
            // the Agent based on TPL Dataflow  

            /* code here
 
                var agent = new ReactiveAgent.AgentObservable ... 
            */
         
            
            IDisposable disposeAll = new CompositeDisposable(
                inputBlock.LinkTo(readLinesBlock, linkOptions),
                readLinesBlock.LinkTo(splitWordsBlock, linkOptions),
                splitWordsBlock.LinkTo(foundMatchesBlock, linkOptions)
                //foundMatchesBlock.LinkTo(agent),
                //agent.AsObservable() ... code missing here
            );

            cts.Token.Register(disposeAll.Dispose);

            foreach (var file in files)
                await inputBlock.SendAsync(file, cts.Token);

            inputBlock.Complete();
            await foundMatchesBlock.Completion.ContinueWith(_ => disposeAll.Dispose());
        }
    }
}