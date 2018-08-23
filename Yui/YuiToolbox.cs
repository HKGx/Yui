using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yui.Entities;
using Yui.Entities.Database;

namespace Yui
{
    public class YuiToolbox
    {
        public ConcurrentBag<YuiShard> Shards = new ConcurrentBag<YuiShard>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public static YuiToolbox YToolbox;
        
        private static async Task Main(string[] args)
        {
            var sharedData = new SharedData();
            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.CreateDirectory(currentDirectory + "/data");
            #region GET HUGS
            Directory.CreateDirectory(currentDirectory + "/hugs");
            sharedData.ReloadHugs();
            #endregion
            
            #region Get Translations


            if (!File.Exists(currentDirectory + "/data/translations.json"))
            {
                await File.WriteAllTextAsync(currentDirectory + "/data/translations.json",
                    JsonConvert.SerializeObject(new ConcurrentDictionary<Guild.Languages, Translation>(), Formatting.Indented));
            }

            await sharedData.LoadTranslationsAsync();
            #endregion

            #region  GET TOKEN

            if (!File.Exists(currentDirectory + "/token.json"))
            {
                await File.WriteAllTextAsync(currentDirectory + "/token.json",
                    JsonConvert.SerializeObject(new Token{ BotToken = ""}, Formatting.Indented));
                Console.WriteLine("Oops, I've generated you token file!");
                Console.WriteLine("Click any key to exit.");
                Console.ReadKey();
                return;
            }
            
            var token = JsonConvert.DeserializeObject<Token>(
                await File.ReadAllTextAsync(currentDirectory + "/token.json"));
            if (string.IsNullOrWhiteSpace(token.BotToken))
            {
                Console.WriteLine("Oops, check your token, please!");
                Console.WriteLine("Click any key to exit.");
                Console.ReadKey();
                return;
            }
            #endregion

            YToolbox = new YuiToolbox();
            sharedData.CTS = YToolbox._cts;
            for (var i = 0; i < 2; i++)
            {
                var shard = new YuiShard(i, sharedData);
                shard.Initialize(token.BotToken);
                YToolbox.Shards.Add(shard);
            }

            foreach (var shard in YToolbox.Shards)
            {
                await shard.StartAsync();
            }
            GC.Collect();
            await WaitForCancellation();
            foreach (var shard in YToolbox.Shards)
                await shard.DisconnectAsync();
            YToolbox._cts.Dispose();
        }

        private static async Task WaitForCancellation()
        {
            while (!YToolbox._cts.IsCancellationRequested)
                await Task.Delay(500);
        }
    }
}