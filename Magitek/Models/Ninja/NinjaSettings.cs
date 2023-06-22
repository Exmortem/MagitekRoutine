using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Ninja
{
    [AddINotifyPropertyChangedInterface]
    public class NinjaSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public NinjaSettings() : base(CharacterSettingsDirectory + "/Magitek/Ninja/NinjaSettings.json") { }

        public static NinjaSettings Instance { get; set; } = new NinjaSettings();

        #region PVP

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Assassinate { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Mug { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Bunshin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_FumaShuriken { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool Pvp_DoNotUseThreeMudra { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Doton { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Huton { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float Pvp_HutonHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Meisui { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float Pvp_MeisuiHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_ForkedRaiju { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_FleetingRaiju { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_HyoshoRanryu { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_GokaMekkyaku { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int Pvp_GokaMekkyakuMinEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Shukuchi { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_SeitonTenchu { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float Pvp_SeitonTenchuHealthPercent { get; set; }

        #endregion
    }
}