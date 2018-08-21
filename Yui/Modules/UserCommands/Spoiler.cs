using System;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yui.Extensions;

namespace Yui.Modules.UserCommands
{
    [Group("spoiler"), Aliases("s")]
    public class Spoiler : BaseCommandModule
    {
        private SharedData _data;

        public Spoiler(SharedData data, Random random, HttpClient client)
        {
            _data = data;
        }


        [Command("add")]
        public async Task AddSpoiler(CommandContext ctx, [RemainingText] string spoiler)
        {
            await ctx.Message.DeleteAsync();
            var trans = ctx.Guild.GetTranslation(_data);
            await ctx.RespondAsync(trans.SpoilerCreatedText);
            await ctx.RespondAsync($"``{Encode(spoiler)}``");
        }

        [Command("get")]
        public async Task GetSpoiler(CommandContext ctx, [RemainingText] string spoiler)
        {
           
            await ctx.Message.DeleteAsync();
            var trans = ctx.Guild.GetTranslation(_data);
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