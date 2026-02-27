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
            return Regex.Replace(camelCaseString, @"([a-z])([A-Z])", "$1 $2");
        }

        /// <summary>
        /// Converts a display name like "Heart of the Fields" into a safe ID like "heart_of_the_fields".
        /// </summary>
        public static string GenerateIdFromName(this string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Guid.NewGuid().ToString(); // Fallback if somehow both ID and Name are missing

            // Lowercase, replace spaces and hyphens with underscores, remove special characters
            var cleanName = name.Trim().ToLowerInvariant();
            cleanName = Regex.Replace(cleanName, @"[\s\-]+", "_");
            cleanName = Regex.Replace(cleanName, @"[^a-z0-9_]", "");

            return cleanName;
        }

        public static bool EqualsIgnoreCase(this string? a, string? b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static T? GetDataByLevel<T>(this IEnumerable<T> collection, int level) where T : class
        {
            if (collection == null || level < 1 || level > 20) return null;
            return collection.ElementAtOrDefault(level - 1);
        }
    }
}
