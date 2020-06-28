using Common.Enums;

namespace Common.Settings
{
    public class AppSettings : SettingsManager<AppSettings>
    {
        public bool DarkMode { get; set; }
        public FileAction FileAction { get; set; } = FileAction.Copy;
    }
}
