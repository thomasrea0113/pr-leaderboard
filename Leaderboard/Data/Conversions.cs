using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Leaderboard.Data
{

    public static class Conversions
    {
        /// <summary>
        /// Store the DateTime as UTC, and return it as UTC. Can then be parsed by the controller to
        /// represent the time as the user's local time.
        /// </summary>
        /// <value></value>
        public static ValueConverter<DateTime, DateTime> LocalToUtcDateTime { get; }
            = new ValueConverter<DateTime, DateTime>(v => DateTime.SpecifyKind(v, DateTimeKind.Utc), v => v);
    }
}