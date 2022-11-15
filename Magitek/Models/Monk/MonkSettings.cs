using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Configuration;

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
        [DefaultValue(false)]
        public bool HidePositionalMessage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EnemyIsOmni { get; set; }

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
        [DefaultValue(60.0f)]
        public float MantraHealthPercent { get; set; }

        [Setting]
        [DefaultValue(8)]
        public int TwinSnakesRefresh { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int DemolishRefresh { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DemolishUseTtd { get; set; }

        [Setting]
        [DefaultValue(6)]
        public double DemolishMinimumTtd { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int DragonKickRefresh { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoeEnemies { get; set; }

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
        [DefaultValue(true)]
        public bool UseRiddleOfWind { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTheForbiddenChakra { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTornadoKick { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMasterfulBlitz { get; set; }

        #region PVP

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_SixSidedStar { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_RisingPhoenix { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Enlightenment { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_RiddleofEarth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_EarthReply { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float Pvp_EarthReplyHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Thunderclap { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Meteodrive { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float Pvp_MeteodriveHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_MeteodriveWithEnlightenment { get; set; }
        #endregion
    }
}