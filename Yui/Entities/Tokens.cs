using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Yui.Entities
{
    public class Tokens : BaseModel
    {
        [JsonProperty("botToken")] public string BotToken { get; set; } = "";
        [JsonProperty("shardsCount")] public int ShardsCount { get; set; } = 1;
    }
}