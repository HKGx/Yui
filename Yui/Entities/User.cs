using System;
using LiteDB;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class User
    {
        public User()
        {
        }

        [BsonId] public Guid DbId { get; set; }
        public ulong Id { get; set; }

    }
}