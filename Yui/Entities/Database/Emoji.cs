using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using LiteDB;
using Yui.Extensions;

namespace Yui.Entities.Database
{
    public class EmojiToRole
    {
        [BsonId]
        public ObjectId DbId { get; set; }
        public string FullName { get; set; }
        public ulong Role { get; set; }

        public async Task<DiscordEmoji> GetEmoji()
        {
            var conv = new DiscordEmojiConverter();
            var o = ((DSharpPlus.CommandsNext.CommandContext) typeof(DSharpPlus.CommandsNext.CommandContext)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
                .Invoke(null));

            foreach(var d in YuiToolbox.YToolbox.Shards){
                Helpers.SetProperty(o, "Client", d.Client);
                break;
            }

            var converted = await conv.ConvertAsync(FullName, o);
            return converted.HasValue ? converted.Value : null;
        }
        
    }
}