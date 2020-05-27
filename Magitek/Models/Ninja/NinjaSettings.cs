using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Ninja
{
    [AddINotifyPropertyChangedInterface]
    public class NinjaSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public NinjaSettings() : base(CharacterSettingsDirectory + "/Magitek/Ninja/NinjaSettings.json") { }

        public static NinjaSettings Instance { get; set; } = new NinjaSettings();

        [Setting]
        [DefaultValue(NinjaOpenerStrategy.AlwaysUseOpener)]
        public NinjaOpenerStrategy NinjaOpenerStrategy { get; set; }

        public void CycleOpenerStrategy()
        {
            switch (NinjaOpenerStrategy)
            {
                case NinjaOpenerStrategy.NeverOpener:
                    NinjaOpenerStrategy = NinjaOpenerStrategy.OpenerOnlyBosses;
                    break;
                case NinjaOpenerStrategy.OpenerOnlyBosses:
                    NinjaOpenerStrategy = NinjaOpenerStrategy.AlwaysUseOpener;
                    break;
                case NinjaOpenerStrategy.AlwaysUseOpener:
                    NinjaOpenerStrategy = NinjaOpenerStrategy.NeverOpener;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Setting]
        [DefaultValue(NinjaOpeners.Default)]
        public NinjaOpeners NinjaOpener { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDualityWithAeolianEdge { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceRaiton { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDualityWithArmorCrush { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShadowFang { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int ShadowFangRefresh { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int HutonRefreshTimer { get; set; }

        [Setting]
        [DefaultValue(5000)]
        public int ShadowFangMinimumHealth { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float ShadowFangMinimumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShadowFangUseTtd { get; set; }

        [Setting]
        [DefaultValue(25)]
        public double ShadowFangMinimumTtd { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDeathBlossom { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int DeathBlossomEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAssassinate { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseForceNinjutsu { get; set; }

        //[Setting]
        //[DefaultValue(false)]
        //public bool ForceArmsLenght { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceTrueNorth { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceBloodBath { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceFeint { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceShadeShift { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceSecondWind { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceLimitBreak { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMug { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTrickAttack { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDreamWithinADream { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHellfrogMedium { get; set; }

        [DefaultValue(1)]
        public int HellfrogMediumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBhavacakra { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShadeShift { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float ShadeShiftHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTenChiJin { get; set; }

        [Setting]
        [DefaultValue(TcjMode.Doton)]
        public TcjMode TenChiJinMode { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseKassatsu { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseFumaShuriken { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseKaton { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int KatonMinEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRaiton { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHuton { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHutonOutsideOfCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHutonOutsideOfCombatOnlyInInstance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBunshin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDoton { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int DotonMinEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGokaMekkyaku { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int GokaMekkyakuMinEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHyoshoRanryu { get; set; }
    }
}