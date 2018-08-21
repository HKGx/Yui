using System;
using System.Dynamic;
using LiteDB;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class EmojiToRole
    {
        [BsonId]
        public Guid DbId { get; set; }
        public string Name { get; set; }
        public ulong Id { get; set; }
        public ulong Role { get; set; }
    }
}