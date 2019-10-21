using System;
using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Monk
{
    [AddINotifyPropertyChangedInterface]
    public class MonkSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public MonkSettings() : base(CharacterSettingsDirectory + "/Magitek/Monk/MonkSettings.json") { }

        public static MonkSettings Instance { get; set; } = new MonkSettings();


        [Setting]
        [DefaultValue(MonkOpenerStrategy.AlwaysUseOpener)]
        public MonkOpenerStrategy MonkOpenerStrategy { get; set; }

        public void CycleOpenerStrategy()
        {
            switch (MonkOpenerStrategy)
            {
                case MonkOpenerStrategy.NeverOpener:
                    MonkOpenerStrategy = MonkOpenerStrategy.OpenerOnlyBosses;
                    break;
                case MonkOpenerStrategy.OpenerOnlyBosses:
                    MonkOpenerStrategy = MonkOpenerStrategy.AlwaysUseOpener;
                    break;
                case MonkOpenerStrategy.AlwaysUseOpener:
                    MonkOpenerStrategy = MonkOpenerStrategy.NeverOpener;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Setting]
        [DefaultValue(true)]
        public bool UseMantra { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int MantraAllies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseAutoMeditate { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseEnlightenment { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int EnlightenmentEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseAutoFormShift { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool AutoFormShiftStopCoeurl { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseManualPB { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool AutoFormShiftStopRaptor { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float MantraHealthPercent { get; set; }

        [Setting]
        [DefaultValue(MonkFists.Fire)]
        public MonkFists SelectedFist { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int TwinSnakesRefresh { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int DemolishRefresh { get; set; }

        [Setting]
        [DefaultValue(5000)]
        public int DemolishMinimumHealth { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float DemolishMinimumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DemolishUseTtd { get; set; }

        [Setting]
        [DefaultValue(15)]
        public double DemolishMinimumTtd { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int DragonKickRefresh { get; set; }

        [Setting]
        [DefaultValue(5000)]
        public int DragonKickMinimumHealth { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float DragonKickMinimumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DragonKickUseTtd { get; set; }

        [Setting]
        [DefaultValue(25)]
        public double DragonKickMinimumTtd { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBrotherhood { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePerfectBalance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRiddleOfFire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRiddleOfEarth { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int RockbreakerEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShoulderTackle { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseElixerField { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int ElixerFieldEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTheForbiddenChakra { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UsePositionalToasts { get; set; }
    }
}