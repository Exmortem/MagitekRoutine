using ff14bot.Helpers;
using Magitek.Enumerations;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Roles
{
    [AddINotifyPropertyChangedInterface]
    public abstract class HealerSettings : JsonSettings
    {
        protected HealerSettings(string path) : base(path) { }

        [Setting]
        [DefaultValue(3)]
        public int StopDamageWhenMoreThanEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSafeguard { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float SafeguardHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMuse { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float MuseManaPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UsePotion { get; set; }

        [Setting]
        [DefaultValue(PotionEnum.None)]
        public PotionEnum PotionTypeAndGradeLevel { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastRes { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool SlowcastRes { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResOutOfCombat { get; set; }

        #region pvp
        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseRecuperate { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float Pvp_RecuperateHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UsePurify { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseGuard { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float Pvp_GuardHealthPercent { get; set; }
        #endregion
    }
}
