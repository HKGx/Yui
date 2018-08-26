using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Yui.Converters
{
    public class EmojiEnumerableConverter : IArgumentConverter<IEnumerable<DiscordEmoji>>
    {
        private static DiscordEmojiConverter _converter;

        public EmojiEnumerableConverter()
        {
            _converter = new DiscordEmojiConverter();
        }
        public async Task<Optional<IEnumerable<DiscordEmoji>>> ConvertAsync(string value, CommandContext ctx)
        {
            if(!value.StartsWith("[") && !value.EndsWith("]"))
                return Optional<IEnumerable<DiscordEmoji>>.FromNoValue();
            
            value = value.TrimStart('[').TrimEnd(']');
            var tokenize = value.Split("|");
            if(tokenize.Length == 0)
                return Optional<IEnumerable<DiscordEmoji>>.FromNoValue();
            var roles = new List<DiscordEmoji>();
            foreach (var token in tokenize)
            {
                var role = await _converter.ConvertAsync(token, ctx);
                if(!role.HasValue)
                    return Optional<IEnumerable<DiscordEmoji>>.FromNoValue();
                roles.Add(role.Value);
            }
            
            return Optional<IEnumerable<DiscordEmoji>>.FromValue(roles.ToArray());
        }
    }
    //TODO:
}