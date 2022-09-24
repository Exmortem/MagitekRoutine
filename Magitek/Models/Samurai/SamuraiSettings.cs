using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Samurai
{
    [AddINotifyPropertyChangedInterface]
    public class SamuraiSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public SamuraiSettings() : base(CharacterSettingsDirectory + "/Magitek/Samurai/SamuraiSettings.json") { }

        public static SamuraiSettings Instance { get; set; } = new SamuraiSettings();

        #region general

        [Setting]
        [DefaultValue(SamuraiOpenerStrategy.OpenerOnlyBosses)]
        public SamuraiOpenerStrategy OpenerStrategy { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HidePositionalMessage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EnemyIsOmni { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int ReservedKenki { get; set; }

        [Setting]
        [DefaultValue(SamuraiFillerStrategy.ThreeGCD)]
        public SamuraiFillerStrategy SamuraiFillerStrategy { get; set; }


        [Setting]
        [DefaultValue(true)]
        public bool UseConeBasedAoECalculationMethod { get; set; }
        
        #endregion

        #region utility

        [Setting]
        [DefaultValue(true)] 
        public bool UseHagakure { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SecondWind { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public new float SecondWindHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceLimitBreak { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bloodbath { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float BloodbathHealthPercent { get; set; }

        #endregion

        #region Buff

        [Setting]
        [DefaultValue(true)]
        public bool UseMeikyoShisui { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMeikyoShisuiOnlyWithZeroSen { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseIkishoten { get; set; }
        #endregion

        #region singletarget
        [Setting]
        [DefaultValue(true)]
        public bool UseEnpi { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuGyoten { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuGyotenOnlyWhenOutOfMeleeRange { get; set; }       

        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuYaten { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuSenei { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuShinten { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShoha { get; set; }

        #endregion

        #region aoe
        [Setting]
        [DefaultValue(false)]
        public bool UseShohaII { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoeEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOgiNamikiri { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseKaeshiNamikiri { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuKyuten { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHissatsuGuren { get; set; }

        #endregion

        #region Iaijutsu
        [Setting]
        [DefaultValue(true)]
        public bool UseHigabana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMidareSetsugekka { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTenkaGoken { get; set; }

        #endregion

        #region Tsubamegaeshi
        [Setting]
        [DefaultValue(false)]
        public bool UseKaeshiHiganbana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseKaeshiSetsugekka { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseKaeshiGoken { get; set; }
        #endregion

        

    }
}