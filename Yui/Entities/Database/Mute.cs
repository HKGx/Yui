using System;
using LiteDB;

namespace Yui.Entities.Database
{
    public class Mute
    {
        [BsonId] public ObjectId DbId;
        public ulong Id;
        public long ToTime;
        public string Reason;
    }
}