using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using LiteDB;
using Yui.Entities;
using Yui.Extensions;

namespace Yui.Modules.ModerationCommands
{
    public class GuildUtilities : BaseCommandModule
    {
        private SharedData _data;

        public GuildUtilities(SharedData data)
        {
            _data = data;
        }

        [Command("roles"), Cooldown(1, 10, CooldownBucketType.User), RequireGuild]
        public async Task GetRoles(CommandContext ctx)
        {
            var trans = ctx.Guild.GetTranslation(_data);
            var embed = new DiscordEmbedBuilder
            {
                Title = trans.AllTheRolesText
            };
            foreach (var role in ctx.Guild.Roles)
            {
                embed.AddField(role.Name, role.Id.ToString(), true);
            }

            await ctx.RespondAsync(embed: embed);
        }

        [Command("modrole"), Cooldown(1, 10, CooldownBucketType.Guild), RequireGuild]
        public async Task SetModRole(CommandContext ctx, DiscordRole modRole)
        {
            if (!IsAdmin(ctx))
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == ctx.Guild.Id);
                guild.ModRole = modRole.Id;
                guilds.Update(guild);
            }
            var trans = ctx.Guild.GetTranslation(_data);
            var text = trans.SetModRoleText.Replace("{{roleName}}", modRole.Name);
            await ctx.RespondAsync(text);


        }
        [Command("lang"), Cooldown(1, 10, CooldownBucketType.Guild), RequireGuild]
        public async Task SetLangAsync(CommandContext ctx, Guild.Languages lang)
        {
            (await ctx.Client.GetCurrentApplicationAsync()).GenerateBotOAuth(Permissions.Administrator);
            if (!IsAdmin(ctx))
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == ctx.Guild.Id);
                guild.Lang = lang;
                guilds.Update(guild.DbId, guild);
            }
            var trans = ctx.Guild.GetTranslation(_data);
            var text = trans.SetLanguageText.Replace("{{langFlag}}", trans.LangFlagText).Replace("{{langJoke}}", trans.LangJokeText);
            await ctx.RespondAsync(text);
        }

        [Command("prefix"), Cooldown(1, 10, CooldownBucketType.Guild), RequireGuild]
        public async Task SetPrefixAsync(CommandContext ctx, string prefix)
        {
            (await ctx.Client.GetCurrentApplicationAsync()).GenerateBotOAuth(Permissions.Administrator);
            if (!IsAdmin(ctx))
                return;
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == ctx.Guild.Id);
                guild.Prefix = prefix;
                guilds.Update(guild.DbId, guild);
            }
            var trans = ctx.Guild.GetTranslation(_data);
            var text = trans.SetPrefixText.Replace("{{prefix}}", prefix);
            await ctx.RespondAsync(text);
        }
        [Command("clear"), Aliases("purge"), Cooldown(1, 2, CooldownBucketType.Channel), RequireGuild, RequireBotPermissions(Permissions.ManageMessages)]
        public async Task ClearMessages(CommandContext ctx, int amount)
        {
            var trans = ctx.Guild.GetTranslation(_data);
            
            if (amount < 1 || amount > 100)
            {
                await ctx.RespondAsync(trans.ClearCommandOutOfBoundariesText);
                return;
            }

            var messages = new List<DiscordMessage>(
                (await ctx.Channel.GetMessagesAsync(amount)).Where(x =>
                    (DateTime.Now - x.CreationTimestamp).Days < 14));
            await ctx.Channel.DeleteMessagesAsync(messages);
            await ctx.RespondAsync(trans.ClearCommandDone.Replace("{{messagesCounts}}", messages.Count().ToString()));
        }
        internal static bool IsAdmin(CommandContext ctx)
        {
            if (ctx.Member.Roles.Any(role => role.Permissions.HasPermission(Permissions.Administrator)))
                return true;
            if (ctx.Member.IsOwner)
                return true;
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var guild = guilds.FindOne(x => x.Id == ctx.Guild.Id);
                if (guild.ModRole == 0) return false;
                if (ctx.Member.Roles.First(x => x.Id == guild.ModRole) != null)
                    return true;
            }
            return false;
        }
    }
}