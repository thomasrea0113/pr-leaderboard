using System;
using Leaderboard.Areas.Uploads.Utilities;
using Leaderboard.Services;

namespace Leaderboard
{
    public class AppConfiguration
    {
        public string HomeBackgroundUrl { get; set; }
        public string[] AdminUsers { get; set; } = Array.Empty<string>();
        public MigrationConfiguration AutoMigrate { get; set; } = new MigrationConfiguration();
        public SmtpEmailSenderConfig Mail { get; set; } = new SmtpEmailSenderConfig();

        /// <summary>
        /// For large file uploads
        /// </summary>
        /// <returns></returns>
        public MultipartModelBinderConfig MultipartModelBinder { get; set; } = new MultipartModelBinderConfig();
    }

    public class MigrationConfiguration
    {
        public bool Enabled { get; set; } = false;
        public bool AutoSeed { get; set; } = false;
        public int TimeoutInSeconds { get; set; } = 30;
    }
}
