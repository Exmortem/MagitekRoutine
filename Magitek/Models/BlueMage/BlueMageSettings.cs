using ff14bot.Helpers;
using Magitek.Enumerations;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.BlueMage
{
    [AddINotifyPropertyChangedInterface]
    public class BlueMageSettings : JsonSettings, IRoutineSettings
    {
        public BlueMageSettings() : base(CharacterSettingsDirectory + "/Magitek/BlueMage/BlueMageSettings.json") { }

        public static BlueMageSettings Instance { get; set; } = new BlueMageSettings();

        [Setting]
        [DefaultValue(BlueMageOpenerStrategy.OpenerOnlyBosses)]
        public BlueMageOpenerStrategy OpenerStrategy { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool OnlyWaterCannon { get; set; }
    }
}