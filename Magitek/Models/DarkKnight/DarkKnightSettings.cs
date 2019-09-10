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

        //General
        #region General
        [Setting]
        [DefaultValue(3000)]
        public int SaveXMana { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UnmendToPullAggro { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UnmendToPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBloodspiller { get; set; }
        #endregion

        //Defensives
        #region Defensives
        [Setting]
        [DefaultValue(true)]
        public bool Grit { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseTheBlackestNight { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float TheBlackestNightHealth { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseDarkMind { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float DarkMindHealth { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseDarkMissionary { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float DarkMissionaryHealth { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseShadowWall { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float ShadowWallHealth { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseLivingDead { get; set; }

        [Setting]
        [DefaultValue(15.0f)]
        public float LivingDeadHealth { get; set; }
        #endregion

        //Buffs
        #region Buffs
        [Setting]
        [DefaultValue(true)]
        public bool BloodWeapon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Delirium { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LivingShadow { get; set; }
        #endregion

        //oGCDs
        #region oGCDs
        [Setting]
        [DefaultValue(false)]
        public bool UsePlunge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCarveAndSpit { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCarveOnlyWithBloodWeapon { get; set; }
        #endregion

        //AoE
        #region AoE
        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseUnleash { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int UnleashEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseQuietus { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int QuietusEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSaltedEarth { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int SaltedEarthEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAbyssalDrain { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int AbyssalDrainEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseFloodDarknessShadow { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int FloodEnemies { get; set; }
        #endregion

    }
}