using System;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Entities.Commands;
using Yui.Extensions;

namespace Yui.Modules.UserCommands
{
    public class UserInteraction : CommandModule
    {
        public UserInteraction(SharedData data, Random random, HttpClient http) : base(data, random, http)
        {
        }

        [Command("hug"), RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task HugAsync(CommandContext ctx, DiscordMember member, [RemainingText]string rest)
        {
            var hug = Data.HugGifs.ToArray()[Random.Next(0, Data.HugGifs.Count)];
            string text;
            if (member.Id == ctx.Member.Id)
            {
                var yuiMember = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
                text = ctx.Guild.GetTranslation(Data).HugLonelyText
                    .Replace("{{botDisplay}}", yuiMember.DisplayName).Replace("{{targetDisplay}}", member.DisplayName);
                await ctx.RespondWithFileAsync(hug, text);
                return;
            }
            text = ctx.Guild.GetTranslation(Data).HugText
                .Replace("{{senderDisplay}}", ctx.Member.DisplayName).Replace("{{targetDisplay}}", member.DisplayName);
            await ctx.RespondWithFileAsync(hug, text);
        }
    }
}