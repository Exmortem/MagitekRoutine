using System.ComponentModel;
using System.Configuration;
using ff14bot.Helpers;
using PropertyChanged;

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
    }
}
