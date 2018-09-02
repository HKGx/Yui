using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Yui.EventHandlers
{
    public static class OnJoin
    {
        public static async Task AssignRole(DiscordMember member, ulong roleId)
        {
            var role = member.Guild.Roles.FirstOrDefault(x => x.Id == roleId);
            await member.GrantRoleAsync(role);
        }
        
    }
}