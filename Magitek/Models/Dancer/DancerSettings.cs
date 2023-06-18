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

        #region buff
        [Setting]
        [DefaultValue(true)]
        public bool UseDevilment { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseFlourish { get; set; }
        #endregion

        #region singletarget
        [Setting]
        [DefaultValue(true)]
        public bool UseStandardStep { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTechnicalStep { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OnlyFinishStepInRange { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FanDance1 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool StarfallDance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FanDance3 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FanDance4 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SaberDance { get; set; }

        [Setting]
        [DefaultValue(85)]
        public int SaberDanceEsprit { get; set; }
        #endregion

        #region aoe
        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Windmill { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int WindmillEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bladeshower { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int BladeshowerEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RisingWindmill { get; set; }

        [Setting]
        [DefaultValue(3)]
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
        #endregion

        #region utility
        [Setting]
        [DefaultValue(true)]
        public bool UseRangeAndFacingChecks { get; set; }



        [Setting]
        [DefaultValue(true)]
        public bool UseCuringWaltz { get; set; }

        [Setting]
        [DefaultValue(40f)]
        public float CuringWaltzHP { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int CuringWaltzCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseImprovisation { get; set; }
        #endregion

        #region partner
        [Setting]
        [DefaultValue(true)]
        public bool UseClosedPosition { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DancePartnerChocobo { get; set; }

        [Setting]
        [DefaultValue(DancePartnerStrategy.ClosestDps)]
        public DancePartnerStrategy SelectedStrategy { get; set; }
        #endregion

        #region Partner Weights

        [Setting]
        [DefaultValue(1)]
        public int SamPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int NinPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int MnkPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int RprPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int DrgPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int BlmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(7)]
        public int SmnPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(8)]
        public int RdmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(9)]
        public int MchPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int BrdPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(11)]
        public int DncPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int GnbPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(13)]
        public int DrkPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(14)]
        public int WarPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(15)]
        public int PldPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(16)]
        public int WhmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(17)]
        public int SchPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(18)]
        public int AstPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(19)]
        public int SagPartnerWeight { get; set; }

        #endregion

        #region PVP
        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseCuringWaltz { get; set; }

        [Setting]
        [DefaultValue(60f)]
        public float Pvp_CuringWaltzHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseContradance { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int Pvp_ContradanceMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseHoningDance { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int Pvp_HoningDanceMinimumEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseClosedPosition { get; set; }

        [Setting]
        [DefaultValue(DancePartnerStrategy.ClosestDps)]
        public DancePartnerStrategy Pvp_DancePartnerSelectedStrategy { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int Pvp_SamPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int Pvp_NinPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int Pvp_MnkPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int Pvp_RprPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int Pvp_DrgPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int Pvp_BlmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(7)]
        public int Pvp_SmnPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(8)]
        public int Pvp_RdmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(9)]
        public int Pvp_MchPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int Pvp_BrdPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(11)]
        public int Pvp_DncPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int Pvp_GnbPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(13)]
        public int Pvp_DrkPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(14)]
        public int Pvp_WarPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(15)]
        public int Pvp_PldPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(16)]
        public int Pvp_WhmPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(17)]
        public int Pvp_SchPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(18)]
        public int Pvp_AstPartnerWeight { get; set; }

        [Setting]
        [DefaultValue(19)]
        public int Pvp_SagPartnerWeight { get; set; }
        #endregion

        [Setting]
        [DefaultValue(false)]
        public bool DontDanceIfCurrentTargetIsDyingSoon { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int DontDanceIfCurrentTargetIsDyingWithinXSeconds { get; set; }

    }
}