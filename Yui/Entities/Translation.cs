using Newtonsoft.Json;

namespace Yui.Entities
{
    public class Translation
    {
        [JsonProperty("getDev")] public string GetDevText                             { get; set; } = "";
        
        [JsonProperty("hugText")] public string HugText                               { get; set; } = "";
        [JsonProperty("hugLonely")] public string HugLonelyText                       { get; set; } = "";
        
        [JsonProperty("laughingReaction1")] public string LaughingReactionText1       { get; set; } = "";
        [JsonProperty("laughingReaction2")] public string LaughingReactionText2       { get; set; } = "";
        [JsonProperty("laughingReaction3")] public string LaughingReactionText3       { get; set; } = "";

        [JsonProperty("reloadedTranslations")] public string ReloadedTranslationsText { get; set; } = "";
        [JsonProperty("reloadedHugs")] public string ReloadedHugsText                 { get; set; } = "";

        [JsonProperty("clearCommandOutOfBoundaries")]
        public string ClearCommandOutOfBoundariesText                                 { get; set; } = "";
        [JsonProperty("clearCommandDone")] public string ClearCommandDone             { get; set; } = "";
        
        [JsonProperty("setModRole")] public string SetModRoleText                     { get; set; } = "";
        [JsonProperty("setPrefix")] public string SetPrefixText                       { get; set; } = "";
        [JsonProperty("setLanguage")] public string SetLanguageText                   { get; set; } = "";
        
        [JsonProperty("langFlag")] public string LangFlagText                         { get; set; } = "";
        [JsonProperty("langJoke")] public string LangJokeText                         { get; set; } = "";
        [JsonProperty("rolesText")] public string AllTheRolesText                     { get; set; } = "";

        [JsonProperty("reactionCommandEmojiUnreachable")]
        public string ReactionCommandEmojiUnreachableText                             { get; set; } = "";
        [JsonProperty("reactionCommandOverLimit")]
        public string ReactionCommandOverLimitText                                    { get; set; } = "";

        [JsonProperty("reactionCommandEmbedName")]
        public string ReactionCommandEmbedNameText                                    { get; set; } = "";

        [JsonProperty("spoilerCreated")] public string SpoilerCreatedText             { get; set; } = "";
        [JsonProperty("spoilerDecoded")] public string SpoilerDecodedText             { get; set; } = "";

        [JsonProperty("profileEmbedTitle")] public string ProfileEmbedTitleText       { get; set; } = "";
        [JsonProperty("profileMoney")] public string ProfileMoneyText                 { get; set; } = "";
        [JsonProperty("profileId")] public string ProfileIdText                       { get; set; } = "";

        [JsonProperty("dailyAcquired")] public string DailyAcquiredText               { get; set; } = "";
        [JsonProperty("dailyTooEarly")] public string DailyTooEarly                   { get; set; } = "";
    }
}