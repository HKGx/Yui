using System;
using LiteDB;

namespace Yui.Entities.Database
{
    public class PermanentInvite
    {
        [BsonId]
        public ObjectId DbId { get; set; }
        public string Code { get; set; }
        public int Uses { get; set; }
    }
}