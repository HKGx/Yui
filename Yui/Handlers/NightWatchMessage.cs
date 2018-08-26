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
                new Regex(@"discord(?:app\.com|\.gg)[\/invite\/]?(?:(?!.*[Ii10OolL]).[a-zA-Z0-9]{5,6}|[a-zA-Z0-9\-]{2,32})",
                    RegexOptions.ECMAScript | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }
        public static async Task NightWatchMessage(MessageCreateEventArgs args)
        {
            if (_inviteRegex.IsMatch(args.Message.Content))
            {
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