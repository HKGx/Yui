using System;
using System.Collections.Generic;
using LiteDB;
using Yui.Entities;
using Yui.Entities.Database;

namespace Yui
{
    public static class Helpers
    {
        public static T GetOrAdd<T>(this LiteCollection<T> self, System.Linq.Expressions.Expression<Func<T, bool>> predicate,  T toAdd)
        {
            var item = self.FindOne(predicate);
            if (item != null)
            {
                return item;
            }
            item = toAdd;
            self.Insert(item);
            return item;
        }
    }
}