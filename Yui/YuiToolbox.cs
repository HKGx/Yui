using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Yui.Entities;

namespace Yui
{
    internal class YuiToolbox
    {
        public ConcurrentBag<YuiShard> Shards = new ConcurrentBag<YuiShard>();
        public ConcurrentBag<DiscordGuildEmoji> Emojis = new ConcurrentBag<DiscordGuildEmoji>();
        public static YuiToolbox YToolbox;
        
        private static async Task Main(string[] args)
        {
            var sharedData = new SharedData();
            var currentDirectory = Directory.GetCurrentDirectory();
            #region GET HUGS
            Directory.CreateDirectory(currentDirectory + "/hugs");
            foreach (var file in Directory.GetFiles(currentDirectory + "/hugs"))
            {
                if (!file.Contains("hug"))
                    continue;
                if (file.EndsWith(".gif"))
                {
                    sharedData.HugGifs.Add(file);
                }
            }
            #endregion

            #region Get Guilds

            Directory.CreateDirectory(currentDirectory + "/data");
            if (!File.Exists(currentDirectory + "/data/guilds.json"))
            {
                await File.WriteAllTextAsync(currentDirectory + "/data/guilds.json",
                    JsonConvert.SerializeObject(new ConcurrentBag<Guild>()));
            }

            sharedData.Guilds =
                JsonConvert.DeserializeObject<ConcurrentBag<Guild>>(
                    await File.ReadAllTextAsync(currentDirectory + "/data/guilds.json"));
            
            #endregion

            #region Get Translations

            Directory.CreateDirectory(currentDirectory + "/data");
            if (!File.Exists(currentDirectory + "/data/translations.json"))
            {
                await File.WriteAllTextAsync(currentDirectory + "/data/translations.json",
                    JsonConvert.SerializeObject(new ConcurrentDictionary<Guild.Languages, Translation>(), Formatting.Indented));
            }
            sharedData.Translations =
                JsonConvert.DeserializeObject<ConcurrentDictionary<Guild.Languages, Translation>>(
                    await File.ReadAllTextAsync(currentDirectory + "/data/translations.json"));
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
            await Task.Delay(-1);

        }
    }
}