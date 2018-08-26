// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Yui.Api.Booru;
//
//    var gelbooruResult = GelbooruResult.FromJson(jsonString);

namespace Yui.Api.Booru
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GelbooruResult
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("directory")]
        public string Directory { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("change")]
        public long Change { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("parent_id")]
        public object ParentId { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("sample")]
        public bool Sample { get; set; }

        [JsonProperty("sample_height")]
        public long SampleHeight { get; set; }

        [JsonProperty("sample_width")]
        public long SampleWidth { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }

    public partial class GelbooruResult
    {
        public static List<GelbooruResult> FromJson(string json) => JsonConvert.DeserializeObject<List<GelbooruResult>>(json, Yui.Api.Booru.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<GelbooruResult> self) => JsonConvert.SerializeObject(self, Yui.Api.Booru.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
