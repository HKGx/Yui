using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Yui.Converters
{
    public class RoleEnumerableConverter : IArgumentConverter<IEnumerable<DiscordRole>>
    {
        private static DiscordRoleConverter _converter;

        public RoleEnumerableConverter()
        {
            _converter = new DiscordRoleConverter();
        }
        public async Task<Optional<IEnumerable<DiscordRole>>> ConvertAsync(string value, CommandContext ctx)
        {
            if(!value.StartsWith("[") && !value.EndsWith("]"))
                return Optional<IEnumerable<DiscordRole>>.FromNoValue();
            
            value = value.TrimStart('[').TrimEnd(']');
            var tokenize = value.Split("|");
            if(tokenize.Length == 0)
                return Optional<IEnumerable<DiscordRole>>.FromNoValue();
            var roles = new List<DiscordRole>();
            foreach (var token in tokenize)
            {
                var role = await _converter.ConvertAsync(token, ctx);
                if(!role.HasValue)
                    return Optional<IEnumerable<DiscordRole>>.FromNoValue();
                roles.Add(role.Value);
            }
            
            return Optional<IEnumerable<DiscordRole>>.FromValue(roles.ToArray());
        }
    }
    //TODO:
}