using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Yui.Converters
{
    public class EnumerableConverter<T> : DSharpPlus.CommandsNext.Converters.IArgumentConverter<IEnumerable<T>>
    {
        private readonly DSharpPlus.CommandsNext.Converters.IArgumentConverter<T> _converter;

        public EnumerableConverter(DSharpPlus.CommandsNext.Converters.IArgumentConverter<T> converter)
        {
            _converter = converter;
        }
        
        public async Task<Optional<IEnumerable<T>>> ConvertAsync(string value, CommandContext ctx)
        {
            if(value[0] != '[' || value[value.Length-1] != ']')
                return Optional<IEnumerable<T>>.FromNoValue();
            
            value = value.TrimStart('[').TrimEnd(']');
            var tokenize = value.Split("|");
            if(tokenize.Length == 0)
                return Optional<IEnumerable<T>>.FromNoValue();
            var toReturn = new List<T>();
            foreach (var token in tokenize)
            {
                var role = await _converter.ConvertAsync(token, ctx);
                if(!role.HasValue)
                    return Optional<IEnumerable<T>>.FromNoValue();
                toReturn.Add(role.Value);
            }
            return Optional<IEnumerable<T>>.FromValue(toReturn.ToArray());
        }
    }
}