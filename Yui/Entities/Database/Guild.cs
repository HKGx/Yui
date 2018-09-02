using System.Collections.Generic;
using LiteDB;

namespace Yui.Entities.Database
{
    public class Guild
    {
        public Guild() {}
        public Guild(ulong id) => Id = id;
        [BsonId] public ObjectId DbId { get; set; }
        public ulong Id { get; set; }
        public ulong ModroleId { get; set; }
        public ulong AutoroleId { get; set; }
        public Language Language { get; set; } = Language.EN;
        public List<Member> Members { get; set; } = new List<Member>();
    }
}