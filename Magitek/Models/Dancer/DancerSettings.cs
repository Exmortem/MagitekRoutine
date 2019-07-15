using System.ComponentModel;
using System.Configuration;
using ff14bot.Helpers;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Dancer
{
    [AddINotifyPropertyChangedInterface]
    public class DancerSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public DancerSettings() : base(CharacterSettingsDirectory + "/Magitek/Dancer/DancerSettings.json") { }

        public static DancerSettings Instance { get; set; } = new DancerSettings();
        
        
        [Setting]
        [DefaultValue(true)]
        public bool DancePartnerChocobo { get; set; }

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

    }
}