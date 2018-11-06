using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LiteDB;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Yui.Entities.Commands;
using Yui.Entities.Database;
using Yui.Extensions;

namespace Yui.Modules.UserCommands
{
    public class User : CommandModule
    {
        public User(SharedData data, Random random, HttpClient http, Api.Imgur.Client client) : base(data, random, http, client)
        {
        }

        [Command("profile"), RequireBotPermissions(Permissions.SendMessages)]
        public async Task GetProfile(CommandContext ctx, DiscordUser user = null)
        {
            if (user == null)
                user = ctx.User;

            DbUser u = null;
            using (var db = new LiteDatabase("Data.db"))
            {
                var users = db.GetCollection<DbUser>();
                u = users.FindOne(x => x.Id == user.Id);
            }
            var trans = ctx.Guild.GetTranslation(Data);
            var embed = new DiscordEmbedBuilder()
                .WithTitle(trans.ProfileEmbedTitleText.Replace("{{userTag}}", $"{user.Username}#{user.Discriminator}"))
                .WithThumbnailUrl(user.AvatarUrl)
                .WithFooter(ctx.Member.GetFullName(), ctx.Member.AvatarUrl);
            embed.AddField(trans.ProfileIdText, user.Id.ToString(), true);
            embed.AddField(trans.ProfileMoneyText, $"{u?.Money.ToString() ?? "0"} <:crystal:481916594864390145>", true);
            await ctx.RespondAsync(embed: embed);
        }

        /*
         * this was just a test
         * it kinda works but looks ugly af
         */
        [Command("imgprofile")]
        public async Task GetImgProfile(CommandContext ctx, DiscordUser user = null)
        {
            if (ctx.User.Id != ctx.Client.CurrentApplication.Owner.Id)
                return;
            if (user == null)
                user = ctx.User;
            DbUser u = null;
            using (var db = new LiteDatabase("Data.db"))
            {
                var users = db.GetCollection<DbUser>();
                u = users.FindOne(x => x.Id == user.Id);
            }

            using (var ms = new MemoryStream())
            {
                using (var img = new Image<Rgba32>(1024, 768))
                {
                    const int size = 39;
                    var font = SystemFonts.CreateFont("Arial", size, FontStyle.Regular);
                    var text = user.Username;
                    var tm = TextMeasurer.Measure(text, new RendererOptions(font));
                    Console.WriteLine($"{tm.Width}, {tm.Height}");
                    try
                    {
                        img.Mutate(c => c
                            .FillPolygon(Helpers.FromDiscordColor(DiscordColor.Blurple),
                                new PointF(0, 470), new PointF(300, 470), /*upper bound*/
                                new PointF(300, 770),
                                new PointF(0, 470 + tm.Height) /* right bound*/));
                        img.Mutate(c => c.DrawText(TextGraphicsOptions.Default, text, font, Rgba32.White,
                            new PointF(100 - tm.Width, 470 + tm.Height)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        img.Dispose();
                        ms.Dispose();
                        return;
                    }

                    img.SaveAsPng(ms);
                }

                ms.Seek(0, SeekOrigin.Begin);
                await ctx.RespondWithFileAsync("file.png", ms);
            }
        }

        [Command("daily"), Aliases("dly"), Cooldown(1, 10, CooldownBucketType.User), RequireBotPermissions(Permissions.SendMessages)]
        public async Task GetDaily(CommandContext ctx)
        {
            using (var db = new LiteDatabase("Data.db"))
            {
                var users = db.GetCollection<DbUser>();
                var user = users.FindOne(x => x.Id == ctx.Member.Id);

                var trans = ctx.Guild.GetTranslation(Data);

                var timeLeft = TimeSpan.FromHours(20).Subtract(TimeSpan.FromTicks(DateTime.Now.Ticks - user.LastDaily));
                if (timeLeft.Ticks >= 0)
                {
                    var timeRemaining = "";
                    if (timeLeft.Hours > 0)
                        timeRemaining = $"{Math.Ceiling(timeLeft.TotalHours)} h";
                    else if (timeLeft.Minutes > 0)
                        timeRemaining = $"{Math.Ceiling(timeLeft.TotalMinutes)} m";
                    else if (timeLeft.Seconds >= 0)
                        timeRemaining = $"{Math.Ceiling(timeLeft.TotalSeconds)} s";
                    await ctx.RespondAsync(trans.DailyTooEarly.Replace("{{timeRemaining}}", timeRemaining));
                    return;
                    
                }

                user.Money += 100;
                user.LastDaily = DateTime.Now.Ticks;
                users.Update(user);
                await ctx.RespondAsync(trans.DailyAcquiredText.Replace("{{dailyValue}}", "100"));
            }
        }
        
    }
}