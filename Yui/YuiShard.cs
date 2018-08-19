using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Yui.Extensions;

namespace Yui
{
    public class YuiShard
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        
        private int _shardId;
        private SharedData _data;

        public YuiShard(int shardId, SharedData data)
        {
            _shardId = shardId;
            _data = data;
        }

        public void Initialize(string token)
        {
            var clientConfig = new DiscordConfiguration
            {
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true,
                TokenType = TokenType.Bot,
                Token = token,
                ShardCount = 2,
                ShardId = _shardId
            };
            Client = new DiscordClient(clientConfig);
            Client.ClientErrored += async args =>
            {
                await Task.Yield();
                Console.WriteLine(args.Exception);
            };
            
            Client.Ready += OnReady;
            Client.MessageCreated += YuiRespondReactions;
            Client.MessageReactionAdded += MessageReactionAdd;
            Client.MessageReactionRemoved += MessageReactionRemove;
            Client.GuildAvailable += ClientOnGuildAvailable;
            Client.GuildEmojisUpdated += ClientOnGuildEmojisUpdated;
            Client.MessageDeleted += ClientOnMessageDeleted;
            
            var services = new ServiceCollection()
                .AddSingleton(_data)
                .AddSingleton(new Random())
                .AddSingleton(new HttpClient())
                .BuildServiceProvider();
            var commandsConfig = new CommandsNextConfiguration
            {
                EnableDefaultHelp = false,
                Services = services,
                StringPrefixes = new []{ "!" }
            };
            Commands = Client.UseCommandsNext(commandsConfig);
            
            Commands.RegisterConverter(new Converters.LangConverter());
            Commands.RegisterConverter(new Converters.EmojiEnumerableConverter());
            Commands.RegisterConverter(new Converters.RoleEnumerableConverter());
            
            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            
            Commands.CommandErrored += async args => { Console.WriteLine(args.Exception); };
            Commands.CommandExecuted += async args => { Console.WriteLine("done " + args.Command.Name); };

        }

        private async Task ClientOnMessageDeleted(MessageDeleteEventArgs e)
        {
            
        }

        private async Task ClientOnGuildEmojisUpdated(GuildEmojisUpdateEventArgs args)
        {
            var es = Yui.YuiToolbox.YToolbox.Emojis.ToList();
            es.RemoveAll(x => args.EmojisBefore.Contains(x));
            es.AddRange(await args.Guild.GetEmojisAsync());
            YuiToolbox.YToolbox.Emojis.Clear();
            foreach (var e in es)
            {
                Console.WriteLine("added" + e);
                YuiToolbox.YToolbox.Emojis.Add(e);
            }
        }

        private async Task MessageReactionAdd(MessageReactionAddEventArgs args)
        {
            if (args.Channel.IsPrivate)
                return;
            foreach (var guild in _data.Guilds)
            {
                if (guild.Id != args.Channel.Guild.Id) continue;
                foreach (var rm in guild.ReactionMessages)
                {
                    if (rm.Channel != args.Channel.Id) continue;
                    if (rm.MessageId != args.Message.Id) continue;
                    foreach (var erm in rm.EmojiToRole)
                    {
                        
                        var emoji = erm.IsGuild
                            ? GetEmoji(erm.Id)
                            : DiscordEmoji.FromUnicode(args.Client, erm.Name);
                        if (emoji != args.Emoji) continue;
                        await (await args.Channel.Guild.GetMemberAsync(args.User.Id)).GrantRoleAsync(
                            args.Channel.Guild.GetRole(erm.Role));
                        return;
                    }
                }
            }
        }
        private async Task MessageReactionRemove(MessageReactionRemoveEventArgs args)
        {
            
            if (args.Channel.IsPrivate)
                return;
            foreach (var guild in _data.Guilds)
            {
                if (guild.Id != args.Channel.Guild.Id) continue;
                foreach (var rm in guild.ReactionMessages)
                {
                    if (rm.Channel != args.Channel.Id) continue;
                    if (rm.MessageId != args.Message.Id) continue;
                    foreach (var erm in rm.EmojiToRole)
                    {
                        var emoji = erm.IsGuild
                            ? GetEmoji(erm.Id)
                            : DiscordEmoji.FromUnicode(args.Client, erm.Name);
                        if (emoji != args.Emoji) continue;
                        await (await args.Channel.Guild.GetMemberAsync(args.User.Id)).RevokeRoleAsync(
                            args.Channel.Guild.GetRole(erm.Role));
                        return;
                    }
                }
            }
        }

        private async Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            Console.WriteLine(e.Guild.Name + " | " + _shardId);
            foreach (var em in await e.Guild.GetEmojisAsync())
            {
                YuiToolbox.YToolbox.Emojis.Add(em);
            }
            if (_data.Guilds.FirstOrDefault(x => x.Id == e.Guild.Id) != null)
                return;
            _data.Guilds.Add(Helpers.DefaultGuild(e.Guild.Id));
            

            await _data.SaveGuildsAsync();
        }

        private async Task OnReady(ReadyEventArgs args)
        {
            await Client.UpdateStatusAsync(
                new DiscordActivity("people hugging on 2 shards!", ActivityType.Watching), UserStatus.DoNotDisturb);
        }
        

        private async Task YuiRespondReactions(MessageCreateEventArgs args)
        {
            if (args.Author.IsBot)
                return;
            if (Regex.IsMatch(args.Message.Content, @"(X)(D{7,})", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                var trans = args.Guild.GetTranslation(_data);
                var msg = new List<DiscordMessage>();
                await Task.Delay(500);
                msg.Add(await args.Message.RespondAsync(trans.LaughingReactionText1));
                await Task.Delay(2000);
                msg.Add(await args.Message.RespondAsync(trans.LaughingReactionText2));
                await Task.Delay(1000);
                msg.Add(await args.Message.RespondAsync(trans.LaughingReactionText3));
                await Task.Delay(5000);
                await args.Channel.DeleteMessagesAsync(msg);
            }
        }
        public async Task StartAsync()
        {
            await Client.ConnectAsync().ConfigureAwait(false);
        }

        private static DiscordEmoji GetEmoji(ulong id)
        {
            foreach (var emoji in YuiToolbox.YToolbox.Emojis)
            {
                if (emoji.Id == id)
                    return emoji;
            }
            throw new Exception();
        }
    }
}