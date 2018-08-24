using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Yui.Handlers
{
    public class SpecialJoin
    {
        private const ulong ChannelId = 482670759970603019;
        private const ulong GuildId = 477477417351512105;
        public static async Task OnSpecialJoin(GuildMemberAddEventArgs args)
        {
            if (args.Guild.Id != GuildId)
            {
                return;
            }

            var channel = (await args.Guild.GetChannelsAsync()).FirstOrDefault(x => x.Id == ChannelId);
            var embed = new DiscordEmbedBuilder().WithTitle("Zweryfikuj się!").WithDescription(
                    $"Hejka {args.Member.Mention}!" +
                    "\n" +
                    "Cieszę się, że do nas dołączyłeś(-aś)!" +
                    "\n" +
                    "Jeżeli ktoś ciebie zaprosił wpisz ``!wer <nazwa użytkownika``, a jeżeli nie to wpisz samo ``!wer``")
                .WithColor(DiscordColor.Blurple);
            await channel.SendMessageAsync(embed: embed);
        }
    }
}