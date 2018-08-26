using System;
using System.Linq;
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
        public UserInteraction(SharedData data, Random random, HttpClient http, Api.Imgur.Client client) : base(data, random, http, client)
        {
        }

        [Command("hug"), RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task HugAsync(CommandContext ctx, DiscordMember member, [RemainingText]string rest)
        {
            var hugs = (await ImgurClient.GetAlbumImagesFromId(Data.ApiKeys.HugAlbum)).Data.Where(x =>
                x.Link.EndsWith(".gif")).ToList();
            var hug = hugs[Random.Next(0, hugs.Count)].Link;
            string text;
            var embed = new DiscordEmbedBuilder().WithColor(DiscordColor.Blurple);
            if (member.Id == ctx.Member.Id)
            {
                
                var yuiMember = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
                embed.Description = ctx.Guild.GetTranslation(Data).HugLonelyText
                    .Replace("{{botDisplay}}", yuiMember.DisplayName).Replace("{{targetDisplay}}", member.DisplayName);
                embed.ImageUrl = hug;
                await ctx.RespondAsync(embed: embed);
                return;
            }
            embed.Description = ctx.Guild.GetTranslation(Data).HugText
                .Replace("{{senderDisplay}}", ctx.Member.DisplayName).Replace("{{targetDisplay}}", member.DisplayName);
            embed.ImageUrl = hug;
            await ctx.RespondAsync(embed: embed);
        }
    }
}