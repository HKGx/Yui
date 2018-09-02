using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LiteDB;
using Yui.Entities;
using Yui.Entities.Database;

namespace Yui.Commands.Moderation
{
    public class Moderation : CommandModule
    {
        public Moderation(SharedData data, Random random, HttpClient client) : base(data, random, client)
        {
        }

        [Command("setlang")]
        public async Task SetLanguage(CommandContext ctx, Language lang)
        {
            if (!IsMod(ctx))
                return;
            using (var db = new LiteDatabase("data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var toUpdate = guilds.FindOne(x => ctx.Guild.Id == x.Id);
                toUpdate.Language = lang;
                guilds.Update(toUpdate);
            }

            await ctx.RespondAsync(CurrentTranslation["setLanguageDone"]);
        }
        [Command("setmodrole")]
        public async Task SetModrole(CommandContext ctx, DiscordRole role)
        {
            if (!IsMod(ctx))
                return;
            using (var db = new LiteDatabase("data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var toUpdate = guilds.FindOne(x => ctx.Guild.Id == x.Id);
                toUpdate.ModroleId = role.Id;
                guilds.Update(toUpdate);
            }

            await ctx.RespondAsync(CurrentTranslation["setModroleDone"].Replace("{{role}}", role.Name));
        }
        [Command("setautorole")]
        public async Task SetAutorole(CommandContext ctx, DiscordRole role)
        {
            if (!IsMod(ctx))
                return;
            using (var db = new LiteDatabase("data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var toUpdate = guilds.FindOne(x => ctx.Guild.Id == x.Id);
                toUpdate.AutoroleId = role.Id;
                guilds.Update(toUpdate);
            }

            await ctx.RespondAsync(CurrentTranslation["setAutoroleDone"].Replace("{{role}}", role.Name));
        }


        public bool IsMod(CommandContext ctx)
        {
            return ctx.Member.Roles.FirstOrDefault(x => x.Id == Guild.ModroleId) != null
                   || ctx.Member.IsOwner
                   || ctx.Member.Roles.FirstOrDefault(x =>
                       x.CheckPermission(Permissions.Administrator) == PermissionLevel.Allowed) != null;
        }
    }
}