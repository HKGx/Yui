using System;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Extensions;

namespace Yui.Modules.UserCommands
{
    public class UserInteraction : BaseCommandModule
    {
        private SharedData _data;
        private Random _random;
        public UserInteraction(SharedData data, Random random, HttpClient client)
        {
            _data = data;
            _random = random;
        }

        [Command("hug"), RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task HugAsync(CommandContext ctx, DiscordMember member, [RemainingText]string rest)
        {
            var hug = _data.HugGifs.ToArray()[_random.Next(0, _data.HugGifs.Count)];
            string text;
            if (member.Id == ctx.Member.Id)
            {
                var yuiMember = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
                text = ctx.Guild.GetTranslation(_data).HugLonelyText
                    .Replace("{{botDisplay}}", yuiMember.DisplayName).Replace("{{targetDisplay}}", member.DisplayName);
                await ctx.RespondWithFileAsync(hug, text);
                return;
            }
            text = ctx.Guild.GetTranslation(_data).HugText
                .Replace("{{senderDisplay}}", ctx.Member.DisplayName).Replace("{{targetDisplay}}", member.DisplayName);
            await ctx.RespondWithFileAsync(hug, text);
        }
    }
}