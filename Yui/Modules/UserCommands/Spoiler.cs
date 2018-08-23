using System;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Entities.Commands;
using Yui.Extensions;

namespace Yui.Modules.UserCommands
{
    [Group("spoiler"), Aliases("s")]
    public class Spoiler : CommandModule
    {


        public Spoiler(SharedData data, Random random, HttpClient http) : base(data, random, http)
        {
        }

        public override async Task BeforeCallingAsync(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
        }

        [Command("add")]
        public async Task AddSpoiler(CommandContext ctx, [RemainingText] string spoiler)
        {
            var trans = ctx.Guild.GetTranslation(Data);
            await ctx.RespondAsync(trans.SpoilerCreatedText);
            await ctx.RespondAsync($"``{Encode(spoiler)}``");
        }

        [Command("get")]
        public async Task GetSpoiler(CommandContext ctx, [RemainingText] string spoiler)
        {
            
            var trans = ctx.Guild.GetTranslation(Data);
            await ctx.Member.SendMessageAsync(trans.SpoilerDecodedText.Replace("{{decoded}}", Decode(spoiler)));
        }

        private static string Encode(string str)
        {
            var chArr = str.ToCharArray();
            for (var i = 0; i < chArr.Length; i++)
            {
                chArr[i] += (char) 5;
            }

            return string.Concat(chArr);
        }

        private static string Decode(string str)
        {
            var chArr = str.ToCharArray();
            for (var i = 0; i < chArr.Length; i++)
            {
                chArr[i] -= (char) 5;
            }

            return string.Concat(chArr);
        }
        
    }
}