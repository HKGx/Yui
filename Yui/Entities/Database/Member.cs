using System;
using LiteDB;

namespace Yui.Entities.Database
{
    public class Member
    {
        public Member() {}
        public Member(ulong id) => Id = id;
        [BsonId]
        public ObjectId DbId { get; set; }
        public ulong Id { get; set; }
    }
}