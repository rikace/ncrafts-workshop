using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;

namespace ParallelPatterns
{
    public class AgentAggregate
    {
        public static string createFileNameFromUrl(string url) =>
            Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

        public static void Run()
        {
            //   Producer/consumer using TPL Dataflow
            List<string> urls = new List<string>
            {
                @"http://www.google.com",
                @"http://www.microsoft.com",
                @"http://www.bing.com",
                @"http://www.google.com"
            };


            // TODO (8)
            // Agent fold over state and messages - Aggregate
            urls.Aggregate(ImmutableDictionary<string, string>.Empty,
                (state, url) =>
                {
                    if (!state.TryGetValue(url, out string content))
                        using (var webClient = new WebClient())
                        {
                            content = webClient.DownloadString(url);
                            System.IO.File.WriteAllText(createFileNameFromUrl(url), content);
                            return state.Add(url, content);
                        }

                    return state;
                });

            // TODO (8)  
            // replace the implementation using the urls.Aggregate with a new one that uses an Agent.

            var agentStateful = Agent.Start(new Dictionary<string, string>(),
                (Dictionary<string, string> state, string msg) =>
                {
                    // Add code implementation
                    return state;
                });


            // run this code 
            urls.ForEach(url => { agentStateful.Post(url); });
        }
    }
}