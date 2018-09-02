
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Yui.Entities;

namespace Yui
{
    
    public class Toolbox
    {
        public static ConcurrentBag<Shard> Shards = new ConcurrentBag<Shard>();

        private static SharedData _data = new SharedData
        {
            Cts = new CancellationTokenSource()
        };

        public static CancellationTokenSource SchedulerCts = new CancellationTokenSource();
        
        public static async Task Main(string[] args)
        {
            var tokens = await Valid<Tokens>();
            if (tokens == null)
            {
                Console.WriteLine("Woop, woop, please check your tokens in \"tokens.json\" file");
                Console.ReadKey();
                return;
            }
            
            await _data.LoadTranslationsAsync();
            var scheduler = new Scheduler(SchedulerCts);
            await scheduler.Run();
            InitializeShards(tokens, scheduler);
            await RunShards();
            await WaitForCancellation();
            await DisconnectShards();
            Console.WriteLine("Ending!");

        }

        private static async Task WaitForCancellation()
        {
            await Task.Yield();
            while (!_data.Cts.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }
        private static void InitializeShards(Tokens tokens, Scheduler scheduler)
        {
            var clientConfig = new DiscordConfiguration
            {
                Token = tokens.BotToken,
                ShardCount = tokens.ShardsCount
            };
            var services = new ServiceCollection()
                .AddSingleton(_data)
                .AddSingleton(new Random())
                .AddSingleton(new HttpClient()).BuildServiceProvider();
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new []{ "!"},
                EnableDefaultHelp = false,
                Services = services
            };
            for (var i = 0; i < tokens.ShardsCount; i++)
            {
                clientConfig.ShardId = i;
                var shard = new Shard(_data, scheduler);
                shard.InitializeShard(i, clientConfig, commandsConfig);
                Shards.Add(shard);
            }
        }

        private static async Task DisconnectShards()
        {
            foreach (var shard in Shards)
            {
                await shard.Disconnect();
            }
        }

        private static async Task RunShards()
        {
            foreach (var shard in Shards)
            {
                await shard.RunShard();
            }
        }

        private static async Task<T> Valid<T>(string name = "") where T : BaseModel, new()
        {

            var file = string.IsNullOrWhiteSpace(name) ? (typeof(T).Name + ".json").ToLower() : name;
            
            if (!File.Exists(file))
            {
                await File.WriteAllTextAsync(file, new T().ToJson());
                return null;
            }
            
            var model = BaseModel.FromJson<T>(await File.ReadAllTextAsync(file));
            return model;
        }

        public DiscordEmoji GetEmote(ulong id)
        {
            var clients = Shards.Select(x => x.Client);
            var guilds = clients.Select(x => x.Guilds.Values);
            var emojis = guilds.SelectMany(x => x.Select(y => y.Emojis.ToDictionary(k => k.Id, v => v)));
            DiscordEmoji emoji = null;
            foreach (var emojiDict in emojis)
            {
                if (!emojiDict.ContainsKey(id))
                {
                    continue;
                }
                emoji = emojiDict[id];
            }

            if (emoji == null)
            {
                throw new KeyNotFoundException("No emoji found for id: " + id);
            }
            return emoji;

        }
    }
}