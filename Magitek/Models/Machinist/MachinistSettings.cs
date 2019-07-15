using System;
using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Machinist
{
    [AddINotifyPropertyChangedInterface]
    public class MachinistSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public MachinistSettings() : base(CharacterSettingsDirectory + "/Magitek/Machinist/MachinistSettings.json") { }

        public static MachinistSettings Instance { get; set; } = new MachinistSettings();

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        [Setting]
        [DefaultValue(MachinistOpenerStrategy.AlwaysUseOpener)]
        public MachinistOpenerStrategy MachinistOpenerStrategy { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int AoeEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWildfire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseFlameThrower { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int FlamethrowerEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseReassemble { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SyncHyperchargeWithWildfire { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int MaxSecondsToHoldHyperchargeForWildfire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHypercharge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBarrelStabilizer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTactician { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int MinBatteryForTurretSummon { get; set; }
    }
}