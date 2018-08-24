using System.Reflection;
using DSharpPlus.Entities;
using LiteDB;
using SixLabors.ImageSharp.PixelFormats;
using Yui.Entities;
using Yui.Entities.Database;

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

        public static string GetFullName(this DiscordUser user)
        {
            return $"{user.Username}#{user.Discriminator}";
        }

        public static string GetFullName(this DiscordMember member)
        {
            return $"{member.Username}#{member.Discriminator}"; 
        }
        public static Guild DefaultGuild(ulong id) => new Guild(id);

        public static Guild WithModRole(this Guild guild, ulong id)
        {
            guild.ModRole = id;
            return guild;
        }
        public static void SetProperty<T1, T2>(T1 o1, string name, T2 o2)
        {
            var property = o1.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            property = property.DeclaringType.GetProperty(property.Name);
            property.SetValue(o1, o2);
        }

        public static Rgba32 FromDiscordColor(DiscordColor color)
        {
            return new Rgba32(color.R, color.G, color.B);
        }

    }
}