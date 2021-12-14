using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Reaper
{
    [AddINotifyPropertyChangedInterface]
    public class ReaperSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public ReaperSettings() : base(CharacterSettingsDirectory + "/Magitek/Reaper/ReaperSettings.json") { }

        public static ReaperSettings Instance { get; set; } = new ReaperSettings();

        #region General-Stuff

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        #endregion

        #region SingleTarget-Abilities

        [Setting]
        [DefaultValue(true)]
        public bool UseShadowOfDeath { get; set; }

        #endregion

        #region AoE-Abilities

        [Setting]
        [DefaultValue(true)]
        public bool UseSpinningScythe { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int SpinningScytheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseNightmareScythe { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int NightmareScytheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWhorlOfDeath { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int WhorlOfDeathTargetCount { get; set; }

        #endregion

        #region Cooldowns



        #endregion

        #region Utility-Abilities



        #endregion

    }
}