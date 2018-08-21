using System.Collections.Concurrent;
using System.Linq;
using DSharpPlus.Entities;
using LiteDB;
using Yui.Entities;

namespace Yui.Extensions
{
    public static class Helpers
    {
        public static Translation GetTranslation(this DiscordGuild guild, SharedData data)
        {
            using (var db = new LiteDatabase("Data.db"))
            {
                var guilds = db.GetCollection<Guild>();
                var g = guilds.FindOne(x => x.Id == guild.Id);
                return data.Translations[g.Lang];
            }
        }

        public static Guild DefaultGuild(ulong id) => new Guild(id);

        public static Guild WithModRole(this Guild guild, ulong id)
        {
            guild.ModRole = id;
            return guild;
        }
    }
}