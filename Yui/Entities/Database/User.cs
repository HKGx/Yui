using LiteDB;

namespace Yui.Entities.Database
{
    public class User
    {
        public User() {}
        public User(ulong id) => Id = id;
        [BsonId] public ObjectId DbId { get; set; }
        public ulong Id { get; set; }
        public int Money { get; set; }
       
    }
}