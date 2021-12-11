using ff14bot.Helpers;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.BlueMage
{
    [AddINotifyPropertyChangedInterface]

    public class BlueMageSettings : MagicDpsSettings, IRoutineSettings
    {
        public BlueMageSettings() : base(CharacterSettingsDirectory + "/Magitek/BlueMage/BlueMageSettings.json") { }

        public static BlueMageSettings Instance { get; set; } = new BlueMageSettings();

        [Setting]
        [DefaultValue(BlueMageOpenerStrategy.OpenerOnlyBosses)]
        public BlueMageOpenerStrategy OpenerStrategy { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool OnlyWaterCannon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMoonFlute { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSonicBoom { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseJKick { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePhantomFlurry { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePrimalSkills { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float LucidDreamingManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Dispel { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SelfCure { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float SelfCureHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Raise { get; set; }

    }
}