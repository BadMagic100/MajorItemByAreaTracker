using System.Linq;

namespace MajorItemByAreaTracker
{
    internal static class StringExtensions
    {
        public static string Merge(this string value, string desiredResult, params string[] optionsToMerge)
        {
            if (optionsToMerge.Contains(value))
            {
                return desiredResult;
            }
            else
            {
                return value;
            }
        }

        public static bool IsOneOf(this string val, params string[] others) => others.Contains(val);
    }
}
