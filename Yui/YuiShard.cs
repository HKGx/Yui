using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Yui.Entities.Database;
using Yui.Extensions;
using Yui.Modules.Special;
using Yui.Modules.UserCommands;

namespace Yui
{
    public class YuiShard
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        
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
            
            Client.MessageCreated += MessageCreated;
            Client.MessageDeleted += ClientOnMessageDeleted;
            
            Client.MessageReactionAdded += MessageReactionAdd;
            Client.MessageReactionRemoved += MessageReactionRemove;
            
            Client.GuildMemberAdded += ClientOnGuildMemberAdded;
            Client.GuildMemberUpdated += ClientOnGuildMemberUpdated;
            Client.GuildAvailable += ClientOnGuildAvailable;
            

            
            var services = new ServiceCollection()
                .AddSingleton(_data)
                .AddSingleton(new Random())
                .AddSingleton(new HttpClient())
                .BuildServiceProvider();
            var commandsConfig = new CommandsNextConfiguration
            {
                EnableDefaultHelp = false,
                Services = services,
                PrefixResolver = ResolvePrefixAsync,
            };
            Commands = Client.UseCommandsNext(commandsConfig);
            
            Commands.RegisterConverter(new Converters.ProfileTypeConverter());
            Commands.RegisterConverter(new Converters.LangConverter());
            Commands.RegisterConverter(new Converters.EmojiEnumerableConverter());
            Commands.RegisterConverter(new Converters.RoleEnumerableConverter());

            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            
            Commands.CommandErrored += async args => { Console.WriteLine(args.Exception); };
            Commands.CommandExecuted += async args => { Console.WriteLine("done " + args.Command.Name); };
            
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration());
            var scheduler = new Scheduler();
            

        }

        private async Task ClientOnGuildMemberUpdated(GuildMemberUpdateEventArgs args)
        {
            await Task.Yield();
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == args.Guild.Id);
                if (guild.NightWatchEnabled)
                {
                    await Handlers.NightWatch.NightWatchUserChange(args);
                }
            }
            
        }

        //TODO: track invites
        private async Task ClientOnGuildMemberAdded(GuildMemberAddEventArgs args)
        {
            
            await Task.Yield();
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == args.Guild.Id);
                if (guild.NightWatchEnabled)
                {
                    await Handlers.NightWatch.NightWatchUserJoin(args);
                }
            }
            await Handlers.SpecialJoin.OnSpecialJoin(args);
            

            
        }

        private async Task ClientOnMessageDeleted(MessageDeleteEventArgs args)
        {
            if (args.Channel.IsPrivate)
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var rms = db.GetCollection<ReactionMessage>();
                var rm = rms.FindOne(x =>
                    x.GuildId   == args.Channel.Guild.Id
                    && x.ChannelId == args.Channel.Id
                    && x.MessageId == args.Message.Id);
                if (rm is null)
                    return;
                rms.Delete(rm.DbId);
            }
        }

        private async Task MessageReactionAdd(MessageReactionAddEventArgs args)
        {
            if (args.Channel.IsPrivate)
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var rms = db.GetCollection<ReactionMessage>();
                var rm = rms.FindOne(x =>
                    x.GuildId   == args.Channel.Guild.Id
                    && x.ChannelId == args.Channel.Id
                    && x.MessageId == args.Message.Id);
                if (rm is null)
                    return;
                foreach (var e in rm.EmojiToRole)
                {
                    var emoji = await e.GetEmoji();
                    if (emoji != args.Emoji) continue;
                    var member = await args.Channel.Guild.GetMemberAsync(args.User.Id);
                    await member.GrantRoleAsync(args.Channel.Guild.GetRole(e.Role));
                    return;
                }
            }
        }

        private async Task MessageReactionRemove(MessageReactionRemoveEventArgs args)
        {
            if (args.Channel.IsPrivate)
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var rms = db.GetCollection<ReactionMessage>();
                var rm = rms.FindOne(x =>
                    x.GuildId   == args.Channel.Guild.Id
                    && x.ChannelId == args.Channel.Id
                    && x.MessageId == args.Message.Id);
                if (rm is null)
                    return;
                foreach (var e in rm.EmojiToRole)
                {
                    var emoji = await e.GetEmoji();
                    if (emoji != args.Emoji) continue;
                    var member = await args.Channel.Guild.GetMemberAsync(args.User.Id);
                    await member.RevokeRoleAsync(args.Channel.Guild.GetRole(e.Role));
                    return;
                }
            }
        }

        private async Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            Console.WriteLine(e.Guild.Name + " | " + _shardId);
            using (var db = new LiteDB.LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                if (guilds.Exists(x => x.Id == e.Guild.Id))
                    return;
                guilds.Insert(Helpers.DefaultGuild(e.Guild.Id));
            }
        }

        private async Task OnReady(ReadyEventArgs args)
        {
            await Client.UpdateStatusAsync(
                new DiscordActivity("people hugging on 2 shards!", ActivityType.Watching), UserStatus.DoNotDisturb);
        }
        

        private async Task MessageCreated(MessageCreateEventArgs args)
        {
            await Task.Yield();
            if (args.Channel.IsPrivate)
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == args.Guild.Id);
                if (guild.HandleUsers)
                {
                    
                }
                else if (guild.NightWatchEnabled)
                {
                    await Handlers.NightWatch.NightWatchMessage(args);
                }
                
            }
        }
        public async Task StartAsync()
        {
            await Client.ConnectAsync().ConfigureAwait(false);
        }

        public async Task DisconnectAsync()
        {
            await Client.DisconnectAsync();
            Client.Dispose();
        }


        private Task<int> ResolvePrefixAsync(DiscordMessage msg)
        {
            using (var db = new LiteDatabase(@"Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var g = guilds.FindOne(x => x.Id == msg.Channel.GuildId);
                return Task.FromResult(g is null
                    ? msg.GetStringPrefixLength("!")
                    : msg.GetStringPrefixLength(g.Prefix));
            }
        }
        
    }
}