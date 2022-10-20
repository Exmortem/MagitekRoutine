using ff14bot.Helpers;
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
        public bool ForceLimitBreak { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRecuperate { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float RecuperateHealthPercent { get; set; }
    }
}
