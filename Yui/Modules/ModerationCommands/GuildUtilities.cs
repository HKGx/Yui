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
using Yui.Entities;
using Yui.Extensions;

namespace Yui.Modules.ModerationCommands
{
    public class GuildUtilities : BaseCommandModule
    {
        private SharedData _data;
        private Random _random;

        public GuildUtilities(SharedData data, Random random, HttpClient client)
        {
            _data = data;
            _random = random;
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
            if (!IsAdmin(ctx, _data))
                return;
            foreach (var guild in _data.Guilds)
            {
                if (guild.Id != ctx.Guild.Id) continue;
                guild.ModRole = modRole.Id;
                await _data.SaveGuildsAsync();
                break;
            }
            var trans = ctx.Guild.GetTranslation(_data);
            var text = trans.SetModRoleText.Replace("{{roleName}}", modRole.Name);
            await ctx.RespondAsync(text);


        }
        [Command("lang"), Cooldown(1, 10, CooldownBucketType.Guild), RequireGuild]
        public async Task SetLangAsync(CommandContext ctx, Guild.Languages lang)
        {
            (await ctx.Client.GetCurrentApplicationAsync()).GenerateBotOAuth(Permissions.Administrator);
            if (!IsAdmin(ctx, _data))
                return;
            foreach (var guild in _data.Guilds)
            {
                if (guild.Id != ctx.Guild.Id) continue;
                guild.Lang = lang;
                await _data.SaveGuildsAsync();
                break;
            }
            var trans = ctx.Guild.GetTranslation(_data);
            var text = trans.SetLanguageText.Replace("{{langFlag}}", trans.LangFlagText).Replace("{{langJoke}}", trans.LangJokeText);
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
        internal static bool IsAdmin(CommandContext ctx, SharedData data)
        {
            if (ctx.Member.Roles.Any(role => role.Permissions.HasPermission(Permissions.Administrator)))
                return true;
            if (ctx.Member.IsOwner)
                return true;
            foreach (var guild in data.Guilds)
            {
                if (guild.Id != ctx.Guild.Id) continue;
                if (guild.ModRole == 0) return false;
                if (ctx.Member.Roles.First(x => x.Id == guild.ModRole) != null)
                    return true;
            }
            return false;
        }
    }
}