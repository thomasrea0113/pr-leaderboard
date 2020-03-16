using System;

namespace Leaderboard.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str) => Char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
