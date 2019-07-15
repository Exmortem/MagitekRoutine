using System.ComponentModel;
using System.Configuration;
using ff14bot.Helpers;
using PropertyChanged;

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
        public bool PrioritizeTankBusters { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTankBusters { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float TankBusterMinimumMpPercent { get; set; }

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
    }
}
