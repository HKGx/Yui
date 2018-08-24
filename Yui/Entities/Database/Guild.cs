using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LiteDB;

namespace Yui.Entities.Database
{
    public class Guild
    {
        public Guild(ulong id) => Id = id;
        public Guild() {}
        [BsonId]
        public ObjectId DbId { get; set; }
        public ulong Id { get; set; }
        public Languages Lang { get; set; } = Languages.EN;
        public ulong ModRole { get; set; }
        public string Prefix { get; set; } = "!";
        public bool HandleUsers { get; set; } = false;
        public bool TrackInvites { get; set; } = false;
        public bool NightWatchEnabled { get; set; } = false;
        public List<GuildUser> GuildsUsers { get; set; } = new List<GuildUser>();
        public List<PermanentInvite> Invites { get; set; } = new List<PermanentInvite>();
        public enum Languages
        {
            EN,
            PL
        }
    }
}