using System.Text;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class ApiKeys
    {
        [JsonProperty("token")] public string BotToken { get; set; } = "";

        [JsonProperty("imgurClientKey")] public string ImgurClientKey { get; set; } = "";

        [JsonProperty("hugAlbum")] public string HugAlbum { get; set; } = "";
    }
}