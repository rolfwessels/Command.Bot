using System.Collections.Generic;
using System.Linq;

namespace Command.Bot.Core.Utils
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

        public static IEnumerable<List<string>> GroupByMaxLength(this IEnumerable<string> values, int maxLength =10)
        {
            var returnValue = new List<string>();
            var counter = 0;
            foreach (var value in values)
            {
                if (counter + value.Length > maxLength)
                {
                    counter = 0;
                    if (returnValue.Any()) yield return returnValue;
                }
                counter += value.Length;
                returnValue.Add(value);
            }
            if (returnValue.Any()) yield return returnValue;
        }

        
    }
}