using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ParallelPatterns
{
    public static class Program
    {
        private static readonly string[] WordsToSearch = 
            {"ENGLISH", "RICHARD", "STEALLING", "MAGIC", "STARS", "MOON", "CASTLE"};


        private static async Task Start(IList<string> files)
        {
            Console.WriteLine(@"=============================================================
Press the number of the method you want to run and press ENTER

(1) RunFuzzyMatchTaskComposition  (TODO 1)
(2) RunFuzzyMatchPLINQ  (TODO 2)
(3) RunFuzzyMatchPipeline  (TODO 3)
(4) RunFuzzyMatchTaskProcessAsCompleteAbstracted  (TODO 4)
(5) RunFuzzyMatchDataFlow  (TODO 5 - 6)
(6) RunFuzzyMatchAgentCSharp (TODO 7 C#)
(7) RunFuzzyMatchAgentFSharp (TODO 7 F#)
(8) AgentAggregate (TODO 8 C#)
=============================================================
");
            
            var choice = Console.ReadLine();
            var indexChoice = int.Parse(choice);
            
            switch (indexChoice)
            {
                case 1:
                    // TODO 1
                    await ParallelFuzzyMatch.RunFuzzyMatchTaskComposition(WordsToSearch, files);
                    
                    break;
                case 2:
                    // TODO 2
                    await ParallelFuzzyMatch.RunFuzzyMatchPLINQ(WordsToSearch, files);

                    break;
                case 3:
                    // TODO 3
                    ParallelFuzzyMatch.RunFuzzyMatchPipeline(WordsToSearch, files);

                    break;
                case 4:
                    // TODO 4
                    await ParallelFuzzyMatch.RunFuzzyMatchTaskProcessAsCompleteAbstracted(WordsToSearch, files);

                    break;
                case 5:
                    // TODO 5 - 6
                    await ParallelFuzzyMatch.RunFuzzyMatchDataFlow(WordsToSearch, files);

                    break;
                case 6:
                    // TODO 7 (C#)
                     ParallelFuzzyMatch.RunFuzzyMatchAgentCSharp(WordsToSearch, files);
                    
                    break;
                case 7:
                    // TODO 7 (F#)
                    await ParallelFuzzyMatch.RunFuzzyMatchAgentFSharp(WordsToSearch, files);
                    
                    break;
                case 8:
                    // TODO 8 (C#)
                    AgentAggregate.Run();
                    
                    break;
                default:
                    throw new Exception("Selection not supported");
            }
        }

        static async Task Main(string[] args)
        {
            IList<string> files =
                    Directory.EnumerateFiles("./Data/Text", "*.txt")
                        .Select(f => new FileInfo(f))
                        .OrderBy(f => f.Length)
                        .Select(f => f.FullName)
                        .Take(5).ToList();

            var watch = Stopwatch.StartNew();

            await Start(files);
            watch.Stop();

            Console.WriteLine($"<< DONE in {watch.Elapsed.ToString()} >>");
            Console.ReadLine();


            // Examples
            // ParallelFuzzyMatch.RunFuzzyMatchSequential(WordsToSearch, files);
            // ParallelFuzzyMatch.RunFuzzyMatchTaskContinuation(WordsToSearch, files);
            // await ParallelFuzzyMatch.RunFuzzyMatchTaskComposition(WordsToSearch, files);
            
            // TODO 1
            // ParallelFuzzyMatch.RunFuzzyMatchTaskComposition(WordsToSearch, files);
            // TODO 2
            // ParallelFuzzyMatch.RunFuzzyMatchPLINQ(WordsToSearch, files);
            // TODO 3
            // ParallelFuzzyMatch.RunFuzzyMatchPipeline(WordsToSearch, files);
            // TODO 4
            // ParallelFuzzyMatch.RunFuzzyMatchTaskProcessAsCompleteAbstracted(WordsToSearch, files);
            // TODO 5 - 6
            // ParallelFuzzyMatch.RunFuzzyMatchDataFlow(WordsToSearch, files);

            // TODO 7 (C#)
            // ParallelFuzzyMatch.RunFuzzyMatchAgentCSharp(WordsToSearch, files);
            // TODO 7 (F#)
            // ParallelFuzzyMatch.RunFuzzyMatchAgentFSharp(WordsToSearch, files);
        }
    }
}