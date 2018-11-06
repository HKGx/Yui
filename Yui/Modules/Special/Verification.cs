using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Entities.Commands;

namespace Yui.Modules.Special
{
    public class Verification : CommandModule
    {
        private const ulong ChannelId = 482670759970603019;
        private const ulong GuildId = 477477417351512105;
        private const ulong RoleId = 482915346530041866;
        private const ulong NewUsersChannelId = 482528066556657665;
        public Verification(SharedData data, Random random, HttpClient http, Api.Imgur.Client client) : base(data, random, http, client)
        {
        }
        /*
         * server that used this is dead, but it works
         */
        [Command("wer")]
        public async Task Verify(CommandContext ctx, DiscordMember user = null)
        {
            if (ctx.Guild.Id != GuildId)
            {
                return;
            }

            if (ctx.Channel.Id != ChannelId)
            {
                return;
            }
            var role = ctx.Guild.Roles.FirstOrDefault(x => x.Id == RoleId);
            if (!ctx.Member.Roles.Contains(role))
            {
                return;
            }
            await ctx.Member.RevokeRoleAsync(role);
            DiscordChannel newUsers;
            DiscordEmbedBuilder embed;
            if (user == null || user.Id == ctx.User.Id)
            {
                newUsers = (await ctx.Guild.GetChannelsAsync()).First(x => x.Id == NewUsersChannelId);
                embed = new DiscordEmbedBuilder()
                    .WithTitle("Nowa osoba!")
                    .WithDescription($"{ctx.Member.Mention} dołączył(-ła) do nas!").WithColor(DiscordColor.Blurple);
                await newUsers.SendMessageAsync(embed: embed);
                return;
            }

            if (!(await ctx.Guild.GetAllMembersAsync()).Contains(user))
            {
                newUsers = (await ctx.Guild.GetChannelsAsync()).First(x => x.Id == NewUsersChannelId);
                embed = new DiscordEmbedBuilder()
                    .WithTitle("Nowa osoba!")
                    .WithDescription($"{user.Mention} dołączył(-ła) do nas!").WithColor(DiscordColor.Blurple);
                await newUsers.SendMessageAsync(embed: embed);
                return;
            }

            newUsers = (await ctx.Guild.GetChannelsAsync()).First(x => x.Id == NewUsersChannelId);
            embed = new DiscordEmbedBuilder()
                .WithTitle("Nowa osoba!")
                .WithDescription($"{user.Mention} zaprosił(-ła) {ctx.Member.Mention}!").WithColor(DiscordColor.Blurple);
            await newUsers.SendMessageAsync(embed: embed);
        }
        
        
    }
}