namespace Magitek.Models.Account
{
    public class ZoneSetting
    {
        public ushort ZoneId { get; set; }
        public string SettingsPath { get; set; }
        public object Settings { get; set; }
        public string Job { get; set; }
    }
}
