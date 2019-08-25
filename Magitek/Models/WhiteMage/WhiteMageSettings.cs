using System.ComponentModel;
using System.Configuration;
using System.IO;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.WhiteMage
{
    [AddINotifyPropertyChangedInterface]
    public class WhiteMageSettings : HealerSettings, IRoutineSettings
    {
        public WhiteMageSettings() : base(CharacterSettingsDirectory + "/Magitek/WhiteMage/WhiteMageSettings.json") { }

        public static WhiteMageSettings Instance { get; set; } = new WhiteMageSettings();

        [Setting]
        [DefaultValue(true)]
        public bool Stone { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterruptHealing { get; set; }

        [Setting]
        [DefaultValue(90.0f)]
        public float InterruptHealingHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Raise { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RaiseSwiftcast { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Asylum { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int AsylumAllies { get; set; }

        [Setting]
        [DefaultValue(85.0f)]
        public float AsylumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Assize { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool AssizeHealOnly { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AssizeAllies { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float AssizeHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Tetragrammaton { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TetragrammatonTankOnly { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float TetragrammatonHealthPercent { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Cure3 { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float Cure3HealthPercent { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int Cure3Allies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benediction { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BenedictionTankOnly { get; set; }

        [Setting]
        [DefaultValue(20.0f)]
        public float BenedictionHealthPercent { get; set; }


        [Setting]
        [DefaultValue(true)]
        public bool AfflatusSolace { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool AfflatusSolaceTankOnly { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float AfflatusSolaceHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Medica { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int MedicaAllies { get; set; }

        [Setting]
        [DefaultValue(75)]
        public float MedicaHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Medica2 { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int Medica2Allies { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float Medica2HealthPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int TemperanceAllies { get; set; }

        [Setting]
        [DefaultValue(45)]
        public float TemperanceHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AfflatusRapture { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AfflatusRaptureAllies { get; set; }

        [Setting]
        [DefaultValue(75)]
        public float AfflatusRaptureHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Regen { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RegenOnTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OnlyRegenWhileMoving { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RegenOnHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RegenOnDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RegenKeepUpOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RegenKeepUpOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RegenKeepUpOnDps { get; set; }

        [Setting]
        [DefaultValue(90.0f)]
        public float RegenHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RegenDontCureUnlessUnderTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RegenDontCureUnlessUnderHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RegenDontCureUnlessUnderDps { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float RegenDontCureUnlessUnderHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Cure { get; set; }

        [Setting]
        [DefaultValue(85.0f)]
        public float CureHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Cure2 { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float Cure2HealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AssizeForMana { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float AssizeManaPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DontBuffIfYouHaveOneAlready { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PresenceOfMind { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float PresenceOfMindHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PresenceOfMindTankOnly { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int PresenceOfMindNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Dispel { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DispelOnlyAbove { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float DispelOnlyAboveHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float LucidDreamingManaPercent { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float MinimumManaPercentToDoDamage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DoDamage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AssizeDamage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AssizeOnlyBelow90Mana { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int AssizeEnemies { get; set; }

        [Setting]
        [DefaultValue(20000)]
        public int DotHealthMinimum { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float DotHealthMinimumPercent { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int DotRefreshSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Aero { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Holy { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int HolyEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FluidAura { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTimeTillDeathForDots { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int DontDotIfEnemyDyingWithin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool IgnoreAlliance { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealAllianceHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealAllianceTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealAllianceDps { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ResAllianceHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ResAllianceTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ResAllianceDps { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealAllianceOnlyCure { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        public bool HealPartyMembersPets { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealPartyMembersPetsTitanOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ThinAir { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ThinAirBeforeSwiftcastRaise { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ThinAirBeforeHoly { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool PresenceOfMindBeforeHoly { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ThinAirBeforeCure3 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AutomaticallyDispelAnythingThatsDispellable { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DivineBenison { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Temperance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PlenaryIndulgence { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int PlenaryIndulgenceConfessions { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int PlenaryIndulgenceAllies { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float PlenaryIndulgenceHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool StopDpsIfPartyMemberBelow { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float StopDpsIfPartyMemberBelowHealthPercent { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float CurePvpHealthPercent { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float Cure2PvpHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BenedictionPvp { get; set; }

        [Setting]
        [DefaultValue(20.0f)]
        public float BenedictionPvpHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool StonePvp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RegenPvp { get; set; }

        [Setting]
        [DefaultValue(90.0f)]
        public float RegenPvpHealthPercent { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float DivineBenisonPvpHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DivineBenisonPvp { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int DivineBenisonPvpMinimumLillies { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float AssizePvpHealthPercent { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int AssizePvpAllies { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int AssizePvpMinimumLillies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AssizePvp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AssizePvpForMana { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float AssizePvpMinimumManaToAssize { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float ThinAirManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FluidAuraPvp { get; set; }

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;

            Instance.LoadFrom(path);
        }
    }
}
