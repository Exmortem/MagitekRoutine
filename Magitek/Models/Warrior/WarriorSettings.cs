using System.ComponentModel;
using System.Configuration;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Warrior
{
    [AddINotifyPropertyChangedInterface]
    public class WarriorSettings : TankSettings, IRoutineSettings
    {
        public WarriorSettings() : base(CharacterSettingsDirectory + "/Magitek/Warrior/WarriorSettings.json") { }

        public static WarriorSettings Instance { get; set; } = new WarriorSettings();

        [Setting]
        [DefaultValue(20)]
        public int KeepAtLeastXBeastGauge { get; set; }

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

        #region Defensives
        [Setting]
        [DefaultValue(true)]
        public bool UseEquilibrium { get; set; }

        [Setting]
        [DefaultValue(60)]
        public int EquilibriumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseRawIntuition { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int RawIntuitionHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseThrillOfBattle { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int ThrillOfBattleHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseShakeItOff { get; set; }

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
        #endregion

        #region AoEs
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
        public bool UseUpheaval { get; set; }
        #endregion
    }
}