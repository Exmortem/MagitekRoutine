using ff14bot.Helpers;
using Magitek.Enumerations;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Roles
{
    [AddINotifyPropertyChangedInterface]
    public abstract class TankSettings : JsonSettings
    {
        protected TankSettings(string path) : base(path)
        {
        }

        /*[Setting]
        [DefaultValue(true)]
        //public bool UseTankBusters { get; set; }*/

        [Setting]
        [DefaultValue(true)]
        public bool UseDefensives { get; set; }

        /*[Setting]
        [DefaultValue(false)]
        //public bool UseDefensivesOnlyOnTankBusters { get; set; }*/

        [Setting]
        [DefaultValue(1)]
        public int MaxDefensivesAtOnce { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int MaxDefensivesUnderHp { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float MoreDefensivesHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseProvoke { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRampart { get; set; }

        [Setting]
        [DefaultValue(75)]
        public int RampartHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseReprisal { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int ReprisalHealthPercent { get; set; }

        [Setting]
        [DefaultValue(InterruptStrategy.AnyEnemy)]
        public InterruptStrategy Strategy { get; set; }
    }
}