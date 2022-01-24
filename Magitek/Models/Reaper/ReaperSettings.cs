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
        // MOve Omni Setting to General Settings
        [Setting]
        [DefaultValue(false)]
        public bool EnemyIsOmni { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTTD { get; set; }

        [Setting]
        [DefaultValue(13)]
        public int SaveIfEnemyDyingWithin { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool BurnEverything { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HidePositionalMessage { get; set; }

        #endregion

        #region SingleTarget-Abilities

        [Setting]
        [DefaultValue(true)]
        public bool UseSlice { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWaxingSlice { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseInfernalSlice { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShadowOfDeath { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGibbet { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGallows { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool TrueNorthWithSoulReaverOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSoulSlice { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBloodStalk { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGluttony { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool GluttonySaveSoulGuage { get; set; }

        [Setting]
        [DefaultValue(30)]
        public int GluttonySaveSoulGuageCooldown { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseVoidReaping { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCrossReaping { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseLemuresSlice { get; set; }

        #endregion

        #region AoE-Abilities

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSpinningScythe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int SpinningScytheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseNightmareScythe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int NightmareScytheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWhorlOfDeath { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int WhorlOfDeathTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSoulScythe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int SoulScytheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGrimSwathe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int GrimSwatheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGuillotine { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int GuillotineTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGrimReaping { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int GrimReapingTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseLemuresScythe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int LemuresScytheTargetCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EfficientAoEPotencyCalculation { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCommunio { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseArcaneCircle { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int ArcaneCircleCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ArcaneCircleEntireParty { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePlentifulHarvest { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHarvestMoon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseEnhancedHarpe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int HarvestMoonTargetCount { get; set; }

        #endregion

        #region Cooldowns

        [Setting]
        [DefaultValue(true)]
        public bool UseEnshroud { get; set; }

        #endregion

        #region Utility-Abilities

        [Setting]
        [DefaultValue(true)]
        public bool UseSoulsow { get; set; }

        #endregion

    }
}