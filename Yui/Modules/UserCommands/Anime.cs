using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Api.Kitsu;
using Yui.Entities.Commands;
using Yui.Extensions;

namespace Yui.Modules.UserCommands
{
    [Group("anime"), Aliases("a")]
    public class Anime : CommandModule
    {
        public Anime(SharedData data, Random random, HttpClient http, Api.Imgur.Client client) : base(data, random, http, client)
        {
        }

        [GroupCommand]
        public async Task GetAnimeAsync(CommandContext ctx, [RemainingText] string name)
        {
            var animes = (await Kitsu.GetAnimeAsync(name)).Data.Take(10).ToList();
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Select anime")
                .WithFooter(ctx.Member.GetFullName(), ctx.Member.AvatarUrl);
            for (var i = 0; i < animes.Count; i++)
            {
                var a = animes[i];
                embed.AddField($"[{i + 1}] {a.Attributes.Titles.En}({a.Attributes.Titles.JaJp})",
                    $"{a.Attributes.AverageRating?.ToString() ?? "none"}");
            }
            var index = -1;
            ToDelete.Add(ctx.Message);
            ToDelete.Add(await ctx.RespondAsync(embed: embed));
            ToDelete.Add((await Interactivity.WaitForMessageAsync(
                x => int.TryParse(x.Content, out index)
                     && index > 0
                     && index <= 10
                     || index == -1,
                TimeSpan.FromSeconds(30))).Message);
            if (index == -1)
                return;
            var anime = animes[index - 1];
            embed.WithTitle($"{anime.Attributes.Titles.En}({anime.Attributes.Titles.EnJp})");
            embed.ClearFields();
            embed.Description = anime.Attributes.Synopsis;
            await ctx.RespondAsync(embed: embed);
        }
        
        
    }
}