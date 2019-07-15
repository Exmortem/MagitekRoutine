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
        [DefaultValue(true)]
        public bool DarkArts { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float DarkArtsMinimumMp { get; set; }

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
        public bool Unleash { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UnleashAlwaysUseProc { get; set; }
        
        [Setting]
        [DefaultValue(2)]
        public int UnleashEnemies { get; set; }
        
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
        [DefaultValue(5)]
        public double UnleashMinTimeLeftInCombat{ get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool SaltedEarth { get; set; }
        
        [Setting]
        [DefaultValue(2)]
        public int SaltedEarthEnemies { get; set; }
                
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
        public bool Plunge { get; set; }

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
        [DefaultValue(2)]
        public int AbyssalDrainEnemies { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float AbyssalDrainMinimumMp { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CarveAndSpit { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CarveAndSpitDarkArtsOnly{ get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float CarveAndSpitMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShadowWall { get; set; }
        
        [Setting]
        [DefaultValue(60.0f)]
        public float ShadowWallHealth { get; set; }
                        
        [Setting]
        [DefaultValue(true)]
        public bool LivingDead { get; set; }
        
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
        public bool LowBlow { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bloodspiller { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Quietus { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int QuietusEnemies { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool SouleaterComboWithGrit { get; set; }
    }
}