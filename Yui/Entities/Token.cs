using System.Text;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class ApiKeys
    {
        [JsonProperty("token")]
        public string BotToken { get; set; }
        [JsonProperty("danbooruLogin")]
        public string DanbooruLogin { get; set; }
        [JsonProperty("danbooruApiKey")]
        public string DanbooruApiKey { get; set; }
    }
}