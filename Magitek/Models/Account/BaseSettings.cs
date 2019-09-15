using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using System.Windows.Input;
using ff14bot.Enums;
using ff14bot.Helpers;
using PropertyChanged;

namespace Magitek.Models.Account
{
    [AddINotifyPropertyChangedInterface]
    public class BaseSettings : JsonSettings
    {
        public BaseSettings() : base(CharacterSettingsDirectory + "/Magitek/BaseSettings.json") { }

        private static BaseSettings _instance;
        public static BaseSettings Instance => _instance ?? (_instance = new BaseSettings());

        [Setting]
        [DefaultValue(ModifierKeys.None)]
        public ModifierKeys UseOpenersModkey { get; set; }

        [Setting]
        [DefaultValue(Keys.None)]
        public Keys UseOpenersKey { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AllowGambits { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOpeners { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ResetOpeners { get; set; } 

        [Setting]
        public string ContributorKey { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double SettingsWindowPosX { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double SettingsWindowPosY { get; set; }

        [Setting]
        [DefaultValue(10)]
        public double FontSize { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ZoomHack { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool SettingsWindowTopMost { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DebugPlayerCasting { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DebugEnemyInfo { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DebugHealingLists { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DebugSpellCastHistory { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EnemySpellCastHistory { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        public bool EnemyAuraHistory { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        public bool PartyMemberAuraHistory { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EnemyTargetHistory { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DebugCastingCallerMemberName { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DebugCastingCallerMemberNameIncludePath { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double OverlayPosX { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double OverlayPosY { get; set; }

        [Setting]
        [DefaultValue(140)]
        public double OverlayHeight { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseOverlay { get; set; }

        [Setting]
        [DefaultValue(150)]
        public double OverlayWidth { get; set; }

        [Setting]
        [DefaultValue(0.8)]
        public double OverlayOpacity { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UsePositionalOverlay { get; set; }

        [Setting]
        [DefaultValue(100)]
        public double PositionalOverlayWidth { get; set; }

        [Setting]
        [DefaultValue(0.8)]
        public double PositionalOverlayOpacity { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double PositionalOverlayPosX { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double PositionalOverlayPosY { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseCastOrQueue { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SummonChocobo { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ChocoboStanceDance { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float ChocoboStanceDanceHealthPercent { get; set; }

        [Setting]
        [DefaultValue(CompanionStance.Free)]
        public CompanionStance ChocoboStance { get; set; }

        [Setting]
        [DefaultValue(50)]
        public int UserLatencyOffset { get; set; }
    }
}