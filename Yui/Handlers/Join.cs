using System.Data.SqlTypes;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace Yui.Handlers
{
    public class Join
    {
        public static async Task OnJoinAddRole(GuildMemberAddEventArgs args, ulong role)
        {
            
            var r = args.Guild.GetRole(role);
            await args.Member.GrantRoleAsync(r);
        }
    }
}