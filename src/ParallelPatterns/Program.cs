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
    class Program
    {
        static string[] WordsToSearch = new string[]
            {"ENGLISH", "RICHARD", "STEALLING", "MAGIC", "STARS", "MOON", "CASTLE"};


        static void Start(IEnumerable<string> files)
        {
            string help =
                @"=============================================================
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
";
            Console.WriteLine(help);
            var choice = Console.ReadLine();
            var indexChoice = int.Parse(choice);
            switch (indexChoice)
            {
                case 1:
                    // TODO 1
                    ParallelFuzzyMatch.RunFuzzyMatchTaskComposition(WordsToSearch, files);
                    break;
                case 2:
                    // TODO 2
                    ParallelFuzzyMatch.RunFuzzyMatchPLINQ(WordsToSearch, files);

                    break;
                case 3:
                    // TODO 3
                    ParallelFuzzyMatch.RunFuzzyMatchPipeline(WordsToSearch, files);

                    break;
                case 4:
                    // TODO 4
                    ParallelFuzzyMatch.RunFuzzyMatchTaskProcessAsCompleteAbstracted(WordsToSearch, files);

                    break;
                case 5:
                    // TODO 5 - 6
                    ParallelFuzzyMatch.RunFuzzyMatchDataFlow(WordsToSearch, files);

                    break;
                case 6:
                    // TODO 7 (C#)
                    ParallelFuzzyMatch.RunFuzzyMatchAgentCSharp(WordsToSearch, files);
                    break;
                case 7:
                    // TODO 7 (F#)
                    ParallelFuzzyMatch.RunFuzzyMatchAgentFSharp(WordsToSearch, files);
                    break;
                case 8:
                    AgentAggregate.Run();
                    break;
                default:
                    throw new Exception("Selection not supported");
            }
        }

        static void Main(string[] args)
        {
            IEnumerable<string> files = Directory.EnumerateFiles("./Data/Text", "*.txt")
                .Select(f => new FileInfo(f))
                .OrderBy(f => f.Length)
                .Select(f => f.FullName)
                .Take(5);

           // Start(files);

            // Wait in a console project is ok. C# 7.1 allows to declare a no-blocking entry point in thw Main method
            // (This is not supported in dotent core)

            // Examples
            // ParallelFuzzyMatch.RunFuzzyMatchSequential(WordsToSearch, files);
            // ParallelFuzzyMatch.RunFuzzyMatchTaskContinuation(WordsToSearch, files);
            ParallelFuzzyMatch.RunFuzzyMatchTaskProcessAsCompleteAbstracted(WordsToSearch, files);
            
            


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


            Console.WriteLine("<< DONE >>");
            Console.ReadLine();
        }
    }
}