using System.Text.RegularExpressions;

namespace Wayfinder.Core.Extensions
{
    public static class PathfinderExtensions
    {
        /// <summary>
        /// Convert a camel case value into a nice string. Example: "LawfulGood" becomes "Lawful Good"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string? camelCaseString)
        {
            if (string.IsNullOrWhiteSpace(camelCaseString)) return string.Empty;
            // Use Regex to find uppercase letters and insert a space before them
            return Regex.Replace(camelCaseString, "([A-Z])", " $1").Trim();
        }
    }
}
