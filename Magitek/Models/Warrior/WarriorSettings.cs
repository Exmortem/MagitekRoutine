using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Warrior
{
    [AddINotifyPropertyChangedInterface]
    public class WarriorSettings : TankSettings, IRoutineSettings
    {
        public WarriorSettings() : base(CharacterSettingsDirectory + "/Magitek/Warrior/WarriorSettings.json") { }

        public static WarriorSettings Instance { get; set; } = new WarriorSettings();

        [Setting]
        [DefaultValue(true)]
        public bool UseBeastGauge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseFellCleave { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseInnerChaos { get; set; }

        #region Buffs
        [Setting]
        [DefaultValue(true)]
        public bool UseInfuriate { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int UseInfuriateAtBeastGauge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseInnerRelease { get; set; }
        #endregion

        #region Heal
        [Setting]
        [DefaultValue(true)]
        public bool UseHeal { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int MaxHealAtOnce { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int MaxHealUnderHp { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float MoreHealHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseEquilibrium { get; set; }

        [Setting]
        [DefaultValue(60)]
        public int EquilibriumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseThrillOfBattle { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int ThrillOfBattleHpPercentage { get; set; }
        #endregion

        #region Defensives
        [Setting]
        [DefaultValue(true)]
        public bool UseBloodWhetting { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int BloodWhettingHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseShakeItOff { get; set; }

        [Setting]
        [DefaultValue(70)]
        public int ShakeItOffHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseVengeance { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int VengeanceHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseHolmgang { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int HolmgangHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseNascentFlash { get; set; }

        [Setting]
        [DefaultValue(55)]
        public int NascentFlashHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NascentFlashTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NascentFlashHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NascentFlashDps { get; set; }
        #endregion

        #region AoEs
        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseChaoticCyclone { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int ChaoticCycloneMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDecimate { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int DecimateMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOverpower { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int OverpowerMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int MythrilTempestMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int OrogenyMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePrimalRend { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int PrimalRendMinimumEnemies { get; set; }

        #endregion

        #region Aggro
        [Setting]
        [DefaultValue(true)]
        public bool UseDefiance { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTomahawk { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTomahawkToPull { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTomahawkOnLostAggro { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTomahawkToPullExtraEnemies { get; set; }
        #endregion

        #region oGCDs
        [Setting]
        [DefaultValue(true)]
        public bool UseOnslaught { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OnslaughtOnlyInMelee { get; set; }

        [Setting]
        [DefaultValue(0)]
        public int SaveOnslaughtCharges { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseUpheaval { get; set; }
        #endregion
    }
}