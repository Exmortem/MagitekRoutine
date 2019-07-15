using System.Configuration;
using ff14bot.Helpers;
using PropertyChanged;

namespace Magitek.Models.Account
{
    [AddINotifyPropertyChangedInterface]
    public class AuthenticationSettings : JsonSettings
    {
        public AuthenticationSettings() : base(CharacterSettingsDirectory + "/Magitek/AuthenticationSettings.json") { }

        private static AuthenticationSettings _instance;
        public static AuthenticationSettings Instance => _instance ?? (_instance = new AuthenticationSettings());

        [Setting]
        public string MagitekKey { get; set; }

        [Setting]
        public string MagitekLegacyKey { get; set; }
    }
}
