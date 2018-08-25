using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace Yui.Handlers
{
    public class NightWatch
    {
        private static Regex _inviteRegex;
        static NightWatch()
        {
            _inviteRegex =
                new Regex(@"/(https?:\/\/)?(www\.)?(discord\.(gg|io|me|li)|discordapp\.com\/invite)\/.+[a-z]/",
                    RegexOptions.ECMAScript | RegexOptions.Multiline);
        }
        public static async Task NightWatchMessage(MessageCreateEventArgs args)
        {
            Console.WriteLine("ddd");
            if (_inviteRegex.Matches(args.Message.Content).Count > 0)
            {
                Console.WriteLine("match");
                await args.Message.DeleteAsync();
                await (await args.Guild.GetMemberAsync(args.Author.Id)).RemoveAsync("nightwatch");
            }
        }

        public static async Task NightWatchUserChange(GuildMemberUpdateEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.NicknameAfter))
                return;
            if (_inviteRegex.Matches(args.NicknameAfter).Count > 0)
            {
                await args.Member.RemoveAsync("nightwatch");
                return;
            }

            if (_inviteRegex.Matches(args.Member.Username).Count > 0)
            {
                await args.Member.RemoveAsync("nightwatch");
            }
        }

        public static async Task NightWatchUserJoin(GuildMemberAddEventArgs args)
        {
            if (_inviteRegex.Matches(args.Member.Username).Count > 0)
            {
                await args.Member.RemoveAsync("nightwatch");
            }
        }
    }
}