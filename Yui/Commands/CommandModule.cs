using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using LiteDB;
using Microsoft.CodeAnalysis.CSharp;
using Yui.Entities.Database;

namespace Yui.Commands
{
    public class CommandModule : BaseCommandModule
    {
        protected List<DiscordMessage> ToDelete = new List<DiscordMessage>();
        protected SharedData Data;
        protected Random Random;
        protected HttpClient Http;
        private CommandContext _ctx;

        protected User User
        {
            get
            {
                using (var db = new LiteDatabase("data.db"))
                {
                    var users = db.GetCollection<User>();
                    return users.GetOrAdd(x => x.Id == _ctx.Guild.Id, new User(_ctx.Member.Id));
                }
            }
        }

        protected Guild Guild
        {
            get
            {
                using (var db = new LiteDatabase("data.db"))
                {
                    var guilds = db.GetCollection<Guild>();
                    return guilds.GetOrAdd(x => x.Id == _ctx.Guild.Id, new Guild(_ctx.Guild.Id));
                }
            }
        }

        protected Dictionary<string, string> CurrentTranslation
        {
            get
            {
                return new Dictionary<string, string>(Data.GetTranslations(
                    Guild.Language));
            }
        }

        public CommandModule(SharedData data, Random random, HttpClient client)
        {
            Data = data;
            Random = random;
            Http = client;
        }

        public virtual async Task BeforeCallingAsync(CommandContext ctx) => await Task.Delay(0);
        public virtual async Task AfterCallingAsync(CommandContext ctx) => await Task.Delay(0);
        
        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            _ctx = ctx;
            await BeforeCallingAsync(ctx);
        }

        public override async Task AfterExecutionAsync(CommandContext ctx)
        {
            if (ToDelete.Count > 0)
            {
                await ctx.Channel.DeleteMessagesAsync(ToDelete);
            }

            await AfterCallingAsync(ctx);
        }
    }
}