using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Api.Booru;
using Yui.Entities.Commands;

namespace Yui.Modules.Lewdery
{
    public class Lewderies : CommandModule
    {
        public Lewderies(SharedData data, Random random, HttpClient http, Api.Imgur.Client client) : base(data, random, http, client)
        {
        }

        [Command("booru"), RequireNsfw]
        public async Task GetImageFromBooru(CommandContext ctx, string tag = "", int page = 0, Order order = Order.Random)
        {
            var images = await Api.Booru.Gelbooru.GetImages(tag, page);

            if (images == null)
            {
                await ctx.RespondAsync("Aw! No images found in this tag! You're a big weirdo...");
                return;
            }
            images = images.Where(x => x.Rating == "q" || x.Rating == "e").ToList();
            if (images.Count == 0)
            {
                await ctx.RespondAsync("Aw! No ledweries for you! This page has no lewderies!");
                return;
            }
            GelbooruResult result;
            
            switch (order)
            {
                case Order.Random:
                    
                    result = images[Random.Next(0, images.Count)];
                    break;
                case Order.First:
                    result = images.First();
                    break;
                case Order.Last:
                    result = images.Last();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(order), order, null);
            }

            var embed = new DiscordEmbedBuilder().WithTitle("Lewd stuff")
                .WithDescription($"Your fresh lewderies from **GelBooru**")
                .WithColor(DiscordColor.Blurple)
                .WithImageUrl(result.FileUrl);
            await ctx.RespondAsync(embed: embed);
        }
    }
}