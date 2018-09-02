using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Yui.Commands.Converters
{
    public class ListConverter 
    {
        
    }

    public class ListConverter<T1, T2> : IArgumentConverter<List<T1>>
        where T1 : SnowflakeObject
        where T2 : IArgumentConverter<T1>, new()
    {
        private readonly IArgumentConverter<T1> _converter;
        public ListConverter()
        {
            _converter = new T2();
        }
        
        public async Task<Optional<List<T1>>> ConvertAsync(string value, CommandContext ctx)
        {
            if (!value.StartsWith('[') && !value.EndsWith(']'))
            {
                return Optional<List<T1>>.FromNoValue();
            }
            value = value.Trim('[', ']');
            value = new string(value.Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            var tokens = value.Split(",|".ToCharArray());
            var list = new List<T1>();
            foreach (var token in tokens)
            {
                var converted = await _converter.ConvertAsync(token, ctx);
                if (!converted.HasValue)
                {
                    return Optional<List<T1>>.FromNoValue();
                }
                list.Add((T1) converted);
            }
            return list;
        }
    }
}