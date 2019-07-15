using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Bard
{
    [AddINotifyPropertyChangedInterface]
    public class BardSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public BardSettings() : base(CharacterSettingsDirectory + "/Magitek/Bard/BardSettings.json") { }

        public static BardSettings Instance { get; set; } = new BardSettings();

        [Setting]
        [DefaultValue(true)]
        public bool PerfectPitch { get; set; }
        
        [Setting]
        [DefaultValue(2)]
        public int PerfectPitchRepertoire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Barrage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BarrageOnlyWithRagingStrikes { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBattleVoice { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBattleVoiceOnBossOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RagingStrikes { get; set; }

        [Setting]
        [DefaultValue(BuffStrategy.Always)]
        public BuffStrategy CombatBuffStrategy { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWanderersMinuet { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int DotRefreshTime { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DotMultipleTargets { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int MaximumTargetsToMultiDot { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DontDotIfEnemyIsDyingWithin { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int DontDotIfEnemyIsDyingWithinSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RepellingShot { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RepellingShotOnlyWhenTargeted { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool Feint { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoeBeforeDots { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PlaySongs { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool QuickNock { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int QuickNockEnemiesInCone { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RainOfDeath { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int RainOfDeathEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Dispel { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NaturesMinne { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float NaturesMinneHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NaturesMinneTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NaturesMinneHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NaturesMinneDps { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RagingStrikeAfterWanderersMinuet { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseSideWinderOnlyOnTrick { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ApexArrow { get; set; }

        [Setting]
        [DefaultValue(70)]
        public int ApexArrowMinimumSoulVoice { get; set; }
    }
}