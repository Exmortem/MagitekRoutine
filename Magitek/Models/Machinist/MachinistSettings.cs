using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Machinist
{
    [AddINotifyPropertyChangedInterface]
    public class MachinistSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public MachinistSettings() : base(CharacterSettingsDirectory + "/Magitek/Machinist/MachinistSettings.json") { }

        public static MachinistSettings Instance { get; set; } = new MachinistSettings();


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
        public bool UseChainSaw { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGaussRound { get; set; }

        #endregion

        #region Area-Of-Effect

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseScattergun { get; set; }

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

        [Setting]
        [DefaultValue(false)]
        public bool UseFlamethrower { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int FlamethrowerEnemyCount { get; set; }

        #endregion

        #region MultiTarget

        [Setting]
        [DefaultValue(true)]
        public bool UseRicochet { get; set; }

        #endregion

        #region Pet

        [Setting]
        [DefaultValue(true)]
        public bool UseRookQueen { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseRookQueenOverdrive { get; set; }

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
        public bool UseReassemble { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBarrelStabilizer { get; set; }

        #endregion

        #region Utilty

        [Setting]
        [DefaultValue(true)]
        public bool ForceTactician { get; internal set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }
        #endregion

        #region PVP
        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Scattergun { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UsedAnalysisOnDrill { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UsedAnalysisOnBio { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UsedAnalysisOnAA { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UsedAnalysisOnChainSaw { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseMarksmansSpite { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float Pvp_UseMarksmansSpiteHealthPercent { get; set; }
        #endregion
    }
}