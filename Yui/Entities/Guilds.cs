using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class Guild
    {
        [JsonProperty("id")] public ulong Id { get; set; }

        [JsonProperty("lang")] public Languages Lang { get; set; }

        [JsonProperty("reactionMessages")] public ConcurrentBag<ReactionMessage> ReactionMessages { get; set; }
        
        [JsonProperty("modRole")] public ulong ModRole { get; set; }

        public enum Languages
        {
            EN,
            PL
        }
    }
}