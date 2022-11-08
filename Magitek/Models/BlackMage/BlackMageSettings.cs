using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.BlackMage
{
    [AddINotifyPropertyChangedInterface]
    public class BlackMageSettings : MagicDpsSettings, IRoutineSettings
    {
        public BlackMageSettings() : base(CharacterSettingsDirectory + "/Magitek/BlackMage/BlackMageSettings.json") { }

        public static BlackMageSettings Instance { get; set; } = new BlackMageSettings();

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        [Setting]
        [DefaultValue(BuffStrategy.Always)]
        public BuffStrategy BuffStrategy { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ThunderSingle { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Scathe { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float ScatheOnlyAboveManaPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int ThunderRefreshSecondsLeft { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int ThunderTimeTillDeathSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Sharpcast { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TripleCast { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LeyLines { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LeyLinesBossOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ConvertAfterFire3 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool QuadFlare { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoeEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TriplecastFire4 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastThunder3 { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int TransposeIfMovingAndAstralWillExpireAstral { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int TransposeIfMovingAndAstralWillExpireMoving { get; set; }

        #region PVP
        [Setting]
        [DefaultValue(true)]
        public bool Pvp_ToggleFireOrIceCombo { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseParadoxOnFire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseParadoxOnIce { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseSuperFlareOnFire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseSuperFlareOnIce { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_UseAetherialManipulation { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float Pvp_UseAetherialManipulationtHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_SoulResonance { get; set; }
        #endregion
    }
}