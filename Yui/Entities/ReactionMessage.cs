using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class ReactionMessage
    {
        [JsonProperty("channel")] public ulong Channel { get; set; }

        [JsonProperty("messageId")] public ulong MessageId { get; set; }
        
        [JsonProperty("emojiToRole")] public ConcurrentBag<EmojiToRole> EmojiToRole { get; set; }
    }
}