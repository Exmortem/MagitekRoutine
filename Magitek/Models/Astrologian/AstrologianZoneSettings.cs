using ff14bot.Helpers;
using Magitek.Models.Account;
using PropertyChanged;
using System.Collections.Generic;
using System.Configuration;

namespace Magitek.Models.Astrologian
{
    [AddINotifyPropertyChangedInterface]
    public class AstrologianZoneSettings : JsonSettings
    {
        public AstrologianZoneSettings() : base(CharacterSettingsDirectory + "/Magitek/Scholar/AstrologianZoneSettings.json") { }
        public static AstrologianZoneSettings Instance { get; set; } = new AstrologianZoneSettings();

        [Setting]
        public Dictionary<ushort, ZoneSetting> ZoneSettings { get; set; }
    }
}
