using System.Collections.Concurrent;
using System.Linq;
using DSharpPlus.Entities;
using Yui.Entities;

namespace Yui.Extensions
{
    public static class Helpers
    {
        public static Translation GetTranslation(this DiscordGuild guild, SharedData data) =>
            data.Translations[data.Guilds.First(x => x.Id == guild.Id)?.Lang ?? Guild.Languages.EN];

        public static Guild DefaultGuild(ulong id) => new Guild
        {
            Id = id,
            Lang = Guild.Languages.EN,
            ReactionMessages = new ConcurrentBag<ReactionMessage>(),
            ModRole = 0
        };

        public static Guild WithModRole(this Guild guild, ulong id)
        {
            guild.ModRole = id;
            return guild;
        }
    }
}