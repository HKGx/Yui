using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
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
            
            
            #region Get Translations


            if (!File.Exists(currentDirectory + "/data/translations.json"))
            {
                await File.WriteAllTextAsync(currentDirectory + "/data/translations.json",
                    JsonConvert.SerializeObject(new ConcurrentDictionary<Guild.Languages, Translation>(), Formatting.Indented));
            }

            await sharedData.LoadTranslationsAsync();
            #endregion
            
            #region  GET TOKENS

            if (!File.Exists(currentDirectory + "/token.json"))
            {
                await File.WriteAllTextAsync(currentDirectory + "/token.json",
                    JsonConvert.SerializeObject(new Keys(), Formatting.Indented));
                Console.WriteLine("Oops, I've generated you token file!");
                Console.WriteLine("Click any key to exit.");
                Console.ReadKey();
                return;
            }
            
            var token = JsonConvert.DeserializeObject<Keys>(
                await File.ReadAllTextAsync(currentDirectory + "/token.json"));
            if (string.IsNullOrWhiteSpace(token.BotToken) || string.IsNullOrWhiteSpace(token.ImgurClientKey))
            {
                Console.WriteLine("Oops, check your tokens, please!");
                Console.WriteLine("Click any key to exit.");
                Console.ReadKey();
                return;
            }
            #endregion
            
            YToolbox = new YuiToolbox();
            var imgurClient = new Api.Imgur.Client(token.ImgurClientKey);
            
            sharedData.Keys = token;
            sharedData.CTS = YToolbox._cts;
            
            for (var i = 0; i < 2; i++)
            {
                var shard = new YuiShard(i, sharedData, imgurClient);
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
            GC.Collect();
        }

        private static async Task WaitForCancellation()
        {
            while (!YToolbox._cts.IsCancellationRequested)
                await Task.Delay(500);
        }

        
        public async Task DoOnShards(Func<YuiShard, Task> toDo)
        {
            var tasks = Shards.Select(async x => await toDo(x));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetFromShards<T>(Func<YuiShard, Task<T>> toGet)
        {
            var e = new List<T>();
            foreach (var shard in Shards)
            {
                e.Add(await toGet(shard));
            }
            return e.Where(x => x != null);
        }
    }
}