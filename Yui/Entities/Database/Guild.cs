using System;
using LiteDB;

namespace Yui.Entities.Database
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