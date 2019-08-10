using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.DarkKnight
{
    [AddINotifyPropertyChangedInterface]
    public class DarkKnightSettings : TankSettings, IRoutineSettings
    {
        public DarkKnightSettings() : base(CharacterSettingsDirectory + "/Magitek/DarkKnight/DarkKkightSettings.json") { }

        public static DarkKnightSettings Instance { get; set; } = new DarkKnightSettings();

        [Setting]
        [DefaultValue(true)]
        public bool UseTheBlackestNight { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float TheBlackestNightHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BloodWeapon { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Delirium { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DeliriumWithBloodWeapon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseUnleash { get; set; }
        
        [Setting]
        [DefaultValue(2)]
        public int AoeEnemies { get; set; }
        
        [Setting]
        [DefaultValue(2)]
        public int UnleashOnGroupPull { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UnleashOnInterval { get; set; }
        
        [Setting]
        [DefaultValue(15)]
        public int UnleashIntervalSeconds { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UseSaltedEarth { get; set; }
                
        [Setting]
        [DefaultValue(true)]
        public bool Grit { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Darkside { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DarksideAlwaysKeepOn { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float DarksideManaOn { get; set; }
        
        [Setting]
        [DefaultValue(25.0f)]
        public float DarksideManaOff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SyphonStrike { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float SyphonStrikeBelowMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePlunge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PullWithPlunge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AbyssalDrain { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool AbyssalDrainToPull { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CarveAndSpit { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CarveAndSpitDarkArtsOnly{ get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShadowWall { get; set; }
        
        [Setting]
        [DefaultValue(60.0f)]
        public float ShadowWallHealth { get; set; }
                        
        [Setting]
        [DefaultValue(true)]
        public bool UseLivingDead { get; set; }
        
        [Setting]
        [DefaultValue(15.0f)]
        public float LivingDeadHealth { get; set; }
                                               
        [Setting]
        [DefaultValue(true)]
        public bool UnmendToPullAggro { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UnmendToPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bloodspiller { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Quietus { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDarkMind { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float DarkMindHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDarkMissionary { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float DarkMissionaryHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }
    }
}