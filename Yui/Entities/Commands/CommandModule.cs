using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using LiteDB;
using Yui.Entities.Database;
using Yui.Modules.UserCommands;

namespace Yui.Entities.Commands
{
    public class CommandModule : BaseCommandModule
    {
        protected SharedData Data;
        protected Random Random;
        protected HttpClient Http;
        protected Api.Imgur.Client ImgurClient;
        protected InteractivityExtension Interactivity;
        protected List<DiscordMessage> ToDelete = new List<DiscordMessage>();

        public CommandModule(SharedData data, Random random, HttpClient http, Api.Imgur.Client imgurClient)
        {
            Data = data;
            Random = random;
            Http = http;
            ImgurClient = imgurClient;
        }

        public virtual Task BeforeCallingAsync(CommandContext ctx) => Task.Delay(0);


        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            Interactivity = ctx.Client.GetInteractivity();
            using (var db = new LiteDatabase("Data.db"))
            {
                var users = db.GetCollection<DbUser>();
                if (users.FindOne(u => u.Id == ctx.Member.Id) == null)
                    users.Insert(new DbUser
                    {
                        Id = ctx.Member.Id,
                    });
            }

            await BeforeCallingAsync(ctx);
        }

        public virtual Task AfterCallingAsync(CommandContext ctx) => Task.Delay(0);
        
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