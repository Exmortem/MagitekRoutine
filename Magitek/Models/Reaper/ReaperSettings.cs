using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Reaper
{
    [AddINotifyPropertyChangedInterface]
    public class ReaperSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public ReaperSettings() : base(CharacterSettingsDirectory + "/Magitek/Reaper/ReaperSettings.json") { }

        public static ReaperSettings Instance { get; set; } = new ReaperSettings();

        

    }
}