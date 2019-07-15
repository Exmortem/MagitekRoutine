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
        public bool IsMainTank { get; set; }

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
        [DefaultValue(false)]
        public bool OverpowerNeverInterruptCombo { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int OverpowerMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOverpowerInterval { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int OverpowerIntervalSeconds { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int OverpowersOnPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OverpowerOnlyAsMainTank { get; set; }

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
        public bool UseShakeItOffOnAnyDebuff { get; set; }

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
        [DefaultValue(50.0f)]
        public float UseTomahawkMinTpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOnslaught { get; set; }

        [Setting]
        [DefaultValue(70)]
        public int UseOnslaughtMinBeastGauge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AttemptToStayOnTopOfEnmityInMainTankMode { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int AttemptToStayOnTopOfEnmityByPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OpenWithThreatCombo { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int UIseInfurateAtBeastGauge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseInnerReleaseDefiance { get; set; }
    }
}