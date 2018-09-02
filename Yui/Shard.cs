using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using LiteDB;
using Yui.Commands.Converters;
using Yui.Entities;
using Yui.Entities.Database;

namespace Yui
{
    public class Shard
    {
        internal DiscordClient Client;
        private CommandsNextExtension _commandsNext;
        private Scheduler _scheduler;
        private InteractivityExtension _interactivity;
        private SharedData _data;

        public Shard(SharedData data, Scheduler scheduler)
        {
            _data = data;
            _scheduler = scheduler;
        }

        public void InitializeShard(int id, DiscordConfiguration config, CommandsNextConfiguration commandConfig)
        {
            Client = new DiscordClient(config);
            Client.Ready += args =>
            {
                Console.WriteLine("ready on shard " + id + " woop woop");
                return Task.CompletedTask;
            };

            Task OnGuildAvailable(GuildCreateEventArgs args)
            {
                using (var db = new LiteDatabase("data.db"))
                {
                    var guilds = db.GetCollection<Guild>();
                    guilds.GetOrAdd(x => x.Id == args.Guild.Id, new Guild(args.Guild.Id));
                }

                Console.WriteLine($"Guild {args.Guild.Name}({args.Guild.Id}) loaded!");
                return Task.CompletedTask;
            }

            Client.GuildAvailable += OnGuildAvailable;
            Client.GuildMemberAdded += OnMemberJoin;

            _commandsNext = Client.UseCommandsNext(commandConfig);
            _commandsNext.RegisterConverter(new EnumConverter<Language>());
            _commandsNext.RegisterConverter(new ListConverter<DiscordRole, DiscordRoleConverter>());
            _commandsNext.RegisterConverter(new ListConverter<DiscordMember, DiscordMemberConverter>());
            _commandsNext.RegisterConverter(new ListConverter<DiscordUser, DiscordUserConverter>());
            _commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());


            _commandsNext.CommandExecuted += args =>
            {
                Console.WriteLine($"Command {args.Command.Name} done");
                return Task.CompletedTask;
            };
            _commandsNext.CommandErrored += args =>
            {
                Console.WriteLine(args.Exception);
                return Task.CompletedTask;
            };
            
            _interactivity = Client.UseInteractivity(new InteractivityConfiguration());
        }

        private async Task OnMemberJoin(GuildMemberAddEventArgs e)
        {
            using (var db = new LiteDatabase("data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == e.Guild.Id);
                if (guild.AutoroleId > 0)
                {
                    await EventHandlers.OnJoin.AssignRole(e.Member, guild.AutoroleId);
                }
            }
        }

        public async Task RunShard() => await Client.ConnectAsync();

        public async Task Disconnect()
        {
            await Client.DisconnectAsync();
            Client.Dispose();
        }
    }
}