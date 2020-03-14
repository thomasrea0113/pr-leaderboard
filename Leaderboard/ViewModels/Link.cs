using System;

namespace Leaderboard.ViewModels
{
    // An enum of bootstrap 4 button classes, for customizing buttons server-side
    // https://getbootstrap.com/docs/4.4/components/buttons/
    public enum BootstrapColorClass
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark,
        Link,
    }

    public enum FontawesomeIcon
    {
        Go,
        User,
    }

    public class Link
    {
        public FontawesomeIcon Addon { get; set; }
        public string Label { get; set; }
        public BootstrapColorClass ClassName { get; set; }

        public string Url { get; set; }
    }
}
