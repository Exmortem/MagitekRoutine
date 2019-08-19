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
        [DefaultValue(false)]
        public bool UseDefiance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseEquilibrium { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseInfuriate { get; set; }

        [Setting]
        [DefaultValue(70)]
        public int EquilibriumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSteelCyclone { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int SteelCycloneMinimumEnemies { get; set; }

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
        [DefaultValue(true)]
        public bool OverpowerNeverInterruptCombo { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int OverpowerMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool OverpowerOnlyAsMainTank { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int OverpowersOnPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOverpowerInterval { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int OverpowerIntervalSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRawIntuition { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int RawIntuitionHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseThrillOfBattle { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int ThrillOfBattleHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShakeItOff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseVengeance { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int VengeanceHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHolmgang { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int HolmgangHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTomahawk { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTomahawkToPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTomahawkOnLostAggro { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTomahawkToPullExtraEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOnslaught { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseUpheaval { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int UseInfuriateAtBeastGauge { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int KeepAtLeastXBeastGauge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseInnerRelease { get; set; }
    }
}