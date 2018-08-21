using System;
using System.Collections.Concurrent;
using LiteDB;
using Newtonsoft.Json;

namespace Yui.Entities
{
    public class Guild
    {
        public Guild(ulong id) => Id = id;
        public Guild() {}
        [BsonId]
        public Guid DbId { get; set; }
        public ulong Id { get; set; }
        public Languages Lang { get; set; } = Languages.EN;
        public ulong ModRole { get; set; }
        public string Prefix { get; set; } = "!";
        public enum Languages
        {
            EN,
            PL
        }
    }
}