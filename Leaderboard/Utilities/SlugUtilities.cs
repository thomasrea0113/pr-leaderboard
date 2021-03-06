using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Leaderboard.Utilities
{
    public static class SlugUtilities
    {
        const string SLUG_PATTERN = "^[a-z0-9]+(?:-[a-z0-9]+)*$";
        public static Regex SlugRegex { get; } = new Regex(SLUG_PATTERN);

        public static string Slugify(string name)
        {
            string str = RemoveAccent(name).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static bool IsSlug(string name) => SlugRegex.IsMatch(name);

        private static string RemoveAccent(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
