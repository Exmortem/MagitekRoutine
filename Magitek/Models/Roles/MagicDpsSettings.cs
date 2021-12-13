using ff14bot.Helpers;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;

namespace Magitek.Models.Roles
{
    [AddINotifyPropertyChangedInterface]
    public abstract class MagicDpsSettings : JsonSettings
    {
        protected MagicDpsSettings(string path) : base(path) { }

        [Setting]
        [DefaultValue(true)]
        public bool UseLucidDreaming { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float LucidDreamingMinimumManaPercent { get; set; }


        [Setting]
        [DefaultValue(InterruptStrategy.AnyEnemy)]
        public InterruptStrategy Strategy { get; set; }
    }
}
