using System;
using LiteDB;

namespace Yui.Entities.Database
{
    public class GuildUser
    {
        [BsonId]
        public ObjectId DbId { get; set; }
        public ulong Id { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public long LastXpAcquired { get; set; }
        
    }
}