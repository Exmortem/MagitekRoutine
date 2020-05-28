using ff14bot.Helpers;
using Magitek.Enumerations;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.RedMage
{
    [AddINotifyPropertyChangedInterface]
    public class RedMageSettings : JsonSettings, IRoutineSettings
    {
        public RedMageSettings() : base(CharacterSettingsDirectory + "/Magitek/RedMage/RedMageSettings.json") { }

        public static RedMageSettings Instance { get; set; } = new RedMageSettings();

        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMelee { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Scatter { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int ScatterEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastScatter { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Acceleration { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Displacement { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Engagement { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CorpsACorps { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool CorpsACorpsInMeleeRangeOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Fleche { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool MeleeComboBossesOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Moulinet { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int MoulinetEnemies { get; set; }

        [Setting]
        [DefaultValue(40)]
        public int EmboldenFinisherPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseContreSixte { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int ContreSixteEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ver2 { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int Ver2Enemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Embolden { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EmboldenOnlyWithAnotherBuff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Manafication { get; set; }

        [Setting]
        [DefaultValue(40)]
        public int ManaficationMinimumBlackAndWhiteMana { get; set; }

        [Setting]
        [DefaultValue(60)]
        public int ManaficationMaximumBlackAndWhiteMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float LucidDreamingManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Vercure { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VercureOnlyDualCast { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float VercureHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VercureTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VercureHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VercureDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VercureSelf { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Verraise { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VerraiseTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VerraiseHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool VerraiseDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastVerthunderVeraero { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseReprise { get; set; }

        [Setting]
        [DefaultValue(RedMageOpenerStrategy.AlwaysUseOpener)]
        public RedMageOpenerStrategy RedMageOpenerStrategy { get; set; }

        public void CycleOpenerStrategy()
        {
            switch (RedMageOpenerStrategy)
            {
                case RedMageOpenerStrategy.NeverOpener:
                    RedMageOpenerStrategy = RedMageOpenerStrategy.OpenerOnlyBosses;
                    break;
                case RedMageOpenerStrategy.OpenerOnlyBosses:
                    RedMageOpenerStrategy = RedMageOpenerStrategy.AlwaysUseOpener;
                    break;
                case RedMageOpenerStrategy.AlwaysUseOpener:
                    RedMageOpenerStrategy = RedMageOpenerStrategy.NeverOpener;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
