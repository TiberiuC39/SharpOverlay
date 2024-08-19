using System.Text.RegularExpressions;

namespace SharpOverlay.Utilities
{
    public static class StringCleaner
    {
        public static string ExtractNumbers(string value)
        {
            string cleanValue = Regex.Replace(value, @"[^\d.,-]", string.Empty);

            return cleanValue;
        }
    }
}
