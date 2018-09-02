using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace Yui.Entities
{
    public class BaseModel
    {
        public string ToJson(Formatting f = Formatting.None) => JsonConvert.SerializeObject(this, f);
        public static T FromJson<T>(string json) where T : BaseModel => JsonConvert.DeserializeObject<T>(json);
    }
}