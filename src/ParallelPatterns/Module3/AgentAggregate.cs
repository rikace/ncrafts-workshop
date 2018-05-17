using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace ParallelPatterns
{
    public class AgentAggregate
    {
        private static string CreateFileNameFromUrl(string _) =>
            Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

        public static void Run()
        {
            // Producer/consumer using TPL Dataflow
            var urls = new List<string>
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
                    if (state.TryGetValue(url, out var content)) 
                        return state;
                    
                    using (var webClient = new HttpClient())
                    {
                        System.Console.WriteLine($"Downloading '{url}' sync ...");
                        content = webClient.GetStringAsync(url).GetAwaiter().GetResult();
                        File.WriteAllText(CreateFileNameFromUrl(url), content);
                        return state.Add(url, content);
                    }
                });

            // TODO (8)  
            // replace the implementation using the urls.Aggregate with a new one that uses an Agent.

            var agentStatefulTODO = Agent.Start(
                new Dictionary<string, string>(),
                (Dictionary<string, string> state, string msg) => state);


            #region Solution

            var agentStateful = Agent.Start(
                ImmutableDictionary<string, string>.Empty,
                async (ImmutableDictionary<string, string> state, string url) =>
                {
                    if (state.TryGetValue(url, out string content)) 
                        return state;
                    
                    using (var webClient = new HttpClient())
                    {
                        System.Console.WriteLine($"Downloading '{url}' async ...");
                        content = await webClient.GetStringAsync(url);
                        await File.WriteAllTextAsync(CreateFileNameFromUrl(url), content);
                        return state.Add(url, content);
                    }
                });

            #endregion

            // run this code 
            urls.ForEach(agentStateful.Post);
        }
    }
}