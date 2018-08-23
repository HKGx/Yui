using System;
using LiteDB;

namespace Yui.Entities.Database
{
    public class DbUser
    {
        [BsonId] public Guid DbId     { get; set; }
        public ulong Id               { get; set; }
        public ulong Money            { get; set; } = 0;
        public long LastDaily        { get; set; } = 0;
    }
}