using System;
using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Machinist
{
    [AddINotifyPropertyChangedInterface]
    public class MachinistSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public MachinistSettings() : base(CharacterSettingsDirectory + "/Magitek/Machinist/MachinistSettings.json") { }

        public static MachinistSettings Instance { get; set; } = new MachinistSettings();

        [Setting]
        [DefaultValue(0)]
        public int UserLatencyOffset { get; set; }

        #region SingleTarget

        [Setting]
        [DefaultValue(true)]
        public bool UseSplitShotCombo { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDrill { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHotAirAnchor { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHeatBlast { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGaussRound { get; set; }

        #endregion

        #region Area-Of-Effect

        [Setting]
        [DefaultValue(true)]
        public bool UseSpreadShot { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int SpreadShotEnemyCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBioBlaster { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int BioBlasterEnemyCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAutoCrossbow { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AutoCrossbowEnemyCount { get; set; }

        #endregion

        #region Cooldowns

        [Setting]
        [DefaultValue(true)]
        public bool UseHypercharge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWildfire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBarrelStabilizer { get; set; }

        #endregion


    }
}