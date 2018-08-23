using System;
using System.Collections.Generic;
using LiteDB;

namespace Yui.Entities.Database
{
    public class ReactionMessage
    {
        [BsonId] public Guid DbId { get; set; }
        public ulong GuildId { get; set; }

        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        public List<EmojiToRole> EmojiToRole { get; set; }
    }
}