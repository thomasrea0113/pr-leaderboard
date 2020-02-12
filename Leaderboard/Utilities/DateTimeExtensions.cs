using System;

namespace Leaderboard.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetAge(this DateTime time)
        {
            time = time.Date.ToUniversalTime();
            var today = DateTime.Today.ToUniversalTime();
            var age = today.Year - time.Year;
            if (time.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}