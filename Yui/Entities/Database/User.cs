using System;
using LiteDB;

namespace Yui.Entities.Database
{
    public class DbUser
    {
        [BsonId] public ObjectId DbId { get; set; }
        public ulong Id { get; set; }
        public int Money { get; set; } = 0;
        public long LastDaily { get; set; } = 0;
    }
}