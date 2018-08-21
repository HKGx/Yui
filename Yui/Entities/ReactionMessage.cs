using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using DSharpPlus.Entities;
using LiteDB;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class ReactionMessage
    {
        public ReactionMessage()
        {
        }

        [BsonId] public Guid DbId { get; set; }
        public ulong GuildId { get; set; }

        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        public List<EmojiToRole> EmojiToRole { get; set; }
    }
}