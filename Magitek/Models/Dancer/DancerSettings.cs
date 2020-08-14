using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Dancer
{
    [AddINotifyPropertyChangedInterface]
    public class DancerSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public DancerSettings() : base(CharacterSettingsDirectory + "/Magitek/Dancer/DancerSettings.json") { }

        public static DancerSettings Instance { get; set; } = new DancerSettings();

        [Setting]
        [DefaultValue(true)]
        public bool UseDevilment { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DevilmentWithFlourish { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DevilmentWithTechnicalStep { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCuringWaltz { get; set; }

        [Setting]
        [DefaultValue(80f)]
        public float CuringWaltzHP { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int CuringWaltzCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseFlourish { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseStandardStep { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTechnicalStep { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Windmill { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int WindmillEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bladeshower { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int BladeshowerEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RisingWindmill { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int RisingWindmillEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bloodshower { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int BloodshowerEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FanDanceTwo { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int FanDanceTwoEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SaberDance { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int SaberDanceEnemies { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int SaberDanceEsprit { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseImprovisation { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseClosedPosition { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DancePartnerChocobo { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DontDotIfCurrentTargetIsDyingSoon { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int DontDotIfCurrentTargetIsDyingWithinXSeconds { get; set; }

        [Setting]
        [DefaultValue(DancePartnerStrategy.ClosestDps)]
        public DancePartnerStrategy SelectedStrategy { get; set; }

        #region Partner Weights
        [Setting]
        [DefaultValue(1)]
        public int MnkPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int BlmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int DrgPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int SamPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int MchPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(8)]
        public int SmnPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int BrdPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int NinPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(9)]
        public int RdmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(7)]
        public int DncPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(14)]
        public int PldPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(13)]
        public int WarPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int DrkPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(11)]
        public int GnbPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(15)]
        public int WhmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(16)]
        public int SchPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(17)]
        public int AstPartnerWeight { get; set; }

        #endregion
    }
}