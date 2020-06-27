using System;
using System.Collections.Generic;
using Leaderboard.Services;

namespace Leaderboard
{
    public class AppConfiguration
    {
        public NavConfiguration Nav { get; set; } = new NavConfiguration();

        public IList<string> HomeBackgroundUrls { get; set; }
        public IList<string> AdminUsers { get; set; } = Array.Empty<string>();
        public MigrationConfiguration AutoMigrate { get; set; } = new MigrationConfiguration();
        public SmtpEmailSenderConfig Mail { get; set; } = new SmtpEmailSenderConfig();
    }

    public class NavConfiguration
    {
        public string BrandImage { get; set; } = "/images/brand.png";
    }

    public class MigrationConfiguration
    {
        public bool Enabled { get; set; } = false;
        public bool AutoSeed { get; set; } = false;
        public int TimeoutInSeconds { get; set; } = 30;
    }
}
