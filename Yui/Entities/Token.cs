using Newtonsoft.Json;

namespace Yui.Entities
{
    public class Token
    {
        [JsonProperty("token")]
        public string BotToken { get; set; }
    }
}