namespace Yui.Api.Kitsu
{
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Anime
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("meta")]
        public AnimeMeta Meta { get; set; }

        [JsonProperty("links")]
        public AnimeLinks Links { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("links")]
        public DatumLinks Links { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }

        [JsonProperty("relationships")]
        public Dictionary<string, Relationship> Relationships { get; set; }
    }

    public partial class Attributes
    {
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("coverImageTopOffset")]
        public long CoverImageTopOffset { get; set; }

        [JsonProperty("titles")]
        public Titles Titles { get; set; }

        [JsonProperty("canonicalTitle")]
        public string CanonicalTitle { get; set; }

        [JsonProperty("abbreviatedTitles")]
        public List<string> AbbreviatedTitles { get; set; }

        [JsonProperty("averageRating")]
        public double? AverageRating { get; set; }

        [JsonProperty("ratingFrequencies")]
        public Dictionary<string, string> RatingFrequencies { get; set; }

        [JsonProperty("userCount")]
        public long UserCount { get; set; }

        [JsonProperty("favoritesCount")]
        public long FavoritesCount { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [JsonProperty("nextRelease")]
        public string NextRelease { get; set; }

        [JsonProperty("popularityRank")]
        public long PopularityRank { get; set; }

        [JsonProperty("ratingRank")]
        public long? RatingRank { get; set; }

        [JsonProperty("ageRating")]
        public string AgeRating { get; set; }

        [JsonProperty("ageRatingGuide")]
        public string AgeRatingGuide { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tba")]
        public string Tba { get; set; }

        [JsonProperty("posterImage")]
        public PosterImage PosterImage { get; set; }

        [JsonProperty("coverImage")]
        public CoverImage CoverImage { get; set; }

        [JsonProperty("episodeCount")]
        public long? EpisodeCount { get; set; }

        [JsonProperty("episodeLength")]
        public long? EpisodeLength { get; set; }

        [JsonProperty("totalLength")]
        public long? TotalLength { get; set; }

        [JsonProperty("youtubeVideoId")]
        public string YoutubeVideoId { get; set; }

        [JsonProperty("showType")]
        public string ShowType { get; set; }

        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }
    }

    public partial class CoverImage
    {
        [JsonProperty("tiny")]
        public string Tiny { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("meta")]
        public CoverImageMeta Meta { get; set; }
    }

    public partial class CoverImageMeta
    {
        [JsonProperty("dimensions")]
        public PurpleDimensions Dimensions { get; set; }
    }

    public partial class PurpleDimensions
    {
        [JsonProperty("tiny")]
        public Large Tiny { get; set; }

        [JsonProperty("small")]
        public Large Small { get; set; }

        [JsonProperty("large")]
        public Large Large { get; set; }
    }

    public partial class Large
    {
        [JsonProperty("width")]
        public long? Width { get; set; }

        [JsonProperty("height")]
        public long? Height { get; set; }
    }

    public partial class PosterImage
    {
        [JsonProperty("tiny")]
        public string Tiny { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("meta")]
        public PosterImageMeta Meta { get; set; }
    }

    public partial class PosterImageMeta
    {
        [JsonProperty("dimensions")]
        public FluffyDimensions Dimensions { get; set; }
    }

    public partial class FluffyDimensions
    {
        [JsonProperty("tiny")]
        public Large Tiny { get; set; }

        [JsonProperty("small")]
        public Large Small { get; set; }

        [JsonProperty("medium")]
        public Large Medium { get; set; }

        [JsonProperty("large")]
        public Large Large { get; set; }
    }

    public partial class Titles
    {
        [JsonProperty("en", NullValueHandling = NullValueHandling.Ignore)]
        public string En { get; set; }

        [JsonProperty("en_jp")]
        public string EnJp { get; set; }

        [JsonProperty("en_us", NullValueHandling = NullValueHandling.Ignore)]
        public string EnUs { get; set; }

        [JsonProperty("ja_jp")]
        public string JaJp { get; set; }
    }

    public partial class DatumLinks
    {
        [JsonProperty("self")]
        public string Self { get; set; }
    }

    public partial class Relationship
    {
        [JsonProperty("links")]
        public RelationshipLinks Links { get; set; }
    }

    public partial class RelationshipLinks
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("related")]
        public string Related { get; set; }
    }

    public partial class AnimeLinks
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }
    }

    public partial class AnimeMeta
    {
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public partial class Anime
    {
        public static Anime FromJson(string json) => JsonConvert.DeserializeObject<Anime>(json, Yui.Api.Kitsu.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Anime self) => JsonConvert.SerializeObject(self, Yui.Api.Kitsu.Converter.Settings);
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
