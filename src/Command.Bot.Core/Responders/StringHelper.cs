using System;
using System.Collections.Generic;
using System.Linq;

namespace Command.Bot.Core.Responders
{
    public static class StringHelper
    {
        public static string StringJoinAnd(this IEnumerable<object> values, string lastJoin = " and ")
        {
            if (values == null) return null;
            var stringValues = values.Select(x => x.ToString()).ToArray();  
            if (stringValues.Length <= 1) return stringValues.FirstOrDefault();
            return string.Join(", ", stringValues.Take(stringValues.Length - 1)) + lastJoin + stringValues.Last();
        }

        public static string StringJoin(this IEnumerable<object> values, string separator = ", ")
        {
            if (values == null) return null;
            var stringValues = values.Select(x => x.ToString()).ToArray();
            if (stringValues.Length <= 1) return stringValues.FirstOrDefault();
            return string.Join(separator, stringValues) ;
        }
    }
}