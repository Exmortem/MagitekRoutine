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

        [Setting]
        [DefaultValue(SamuraiOpenerStrategy.OpenerOnlyBosses)]
        public SamuraiOpenerStrategy OpenerStrategy { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SecondWind { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public new float SecondWindHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bloodbath { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool MeikyoShisui { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float BloodbathHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AoeCombo { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoeComboEnemies { get; set; }


        [Setting]
        [DefaultValue(true)]
        public bool OnlyAoeComboWithJinpuShifu { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TenkaGoken { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int TenkaGokenEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HissatsuKyuten { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int HissatsuKyutenEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HissatsuGuren { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HissatsuGurenOnlyWithJinpu { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int HissatsuGurenEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HissatsuSeneiOnlyWithJinpu { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Enpi { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool OnlyUseTenkaGokenWithKaiten { get; set; }

        [Setting]
        [DefaultValue(9)]
        public int HiganbanaRefreshTime { get; set; }


        [Setting]
        [DefaultValue(true)]
        public bool HiganbanaOnlyWithJinpu { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHigabana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSenei { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int ReservedKenki { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TrueNorth { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool MeikyoOnlyWithZeroSen { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseConeBasedAoECalculationMethod { get; set; }

        [Setting]
        [DefaultValue(700)]
        public int UseOffGCDAbilitiesWithMoreThanXMSLeft { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HissatsuSeigan { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShohaII { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int ShohaIIEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool Fuko { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int FukoEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool OgiNamikiri { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int OgiNamikiriEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool KaeshiNamikiri { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int KaeshiNamikiriEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceLimitBreak { get; set; }

    }
}