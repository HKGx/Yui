using System.Dynamic;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class EmojiToRole
    {
        [JsonProperty("isGuild")] public bool IsGuild { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("id")] public ulong Id { get; set; }
        [JsonProperty("role")] public ulong Role { get; set; }
    }
}