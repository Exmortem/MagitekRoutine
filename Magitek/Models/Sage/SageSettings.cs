using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Sage
{
    [AddINotifyPropertyChangedInterface]
    public class SageSettings : HealerSettings, IRoutineSettings
    {
        public SageSettings() : base(CharacterSettingsDirectory + "/Magitek/Sage/SageSettings.json") { }

        public static SageSettings Instance { get; set; } = new SageSettings();

        #region ForcePowers
        [Setting]
        [DefaultValue(false)]
        public bool ForceEukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForcePanhaima { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceZoePneuma { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForcePepsisEukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceHaima { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceEukrasianDiagnosis { get; set; }
        #endregion

        #region FightLogic
        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_Haima { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_Taurochole { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_EukrasianDiagnosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_Panhaima { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_Holos { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_Kerachole { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FightLogic_EukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool FightLogic_RespectOnlyTank { get; set; }


        #endregion

        #region Combat

        [Setting]
        [DefaultValue(true)]
        public bool Dosis { get; set; }
        /*
        [Setting]
        [DefaultValue(false)]
        public bool InterruptHealing { get; set; }

        [Setting]
        [DefaultValue(81.0f)]
        public float InterruptHealingHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool InterruptDamageToHeal { get; set; }

        [Setting]
        [DefaultValue(79.0f)]
        public float InterruptDamageHealthPercent { get; set; }
        */
        [Setting]
        [DefaultValue(true)]
        public bool DoDamage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianDosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DotMultipleTargets { get; set; }

        [Setting]
        [DefaultValue(3050)]
        public int DotRefreshMSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTTDForDots { get; set; }

        [Setting]
        [DefaultValue(13)]
        public int DontDotIfEnemyDyingWithin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AoE { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoEEnemies { get; set; }

        [Setting]
        [DefaultValue(10.0f)]
        public float MinimumManaPercentToDoDamage { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int DoDamageIfTimeLeftLessThan { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ToxiconWhileMoving { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ToxiconOnFullAddersting { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ToxiconOnLowMana { get; set; }

        #endregion

        #region Buffs

        [Setting]
        [DefaultValue(true)]
        public bool Kardia { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaSwitchTargets { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float KardiaSwitchTargetsHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaSwitchTargetsCurrent { get; set; }

        [Setting]
        [DefaultValue(95.0f)]
        public float KardiaSwitchTargetsCurrentHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaMainTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float LucidDreamingManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Krasis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KrasisTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KrasisMainTankOnly { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float KrasisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Soteria { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool SoteriaTankOnly { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float SoteriaHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Kerachole { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KeracholeOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KeracholeOnlyWithMainTank { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float KeracholeHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Rhizomata { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Holos { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float HolosHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HolosTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HolosMainTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Shield { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float ShieldHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldKeepUpOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldKeepUpOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldKeepUpOnSelf { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldKeepUpOnDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldKeepUpUnlessAdderstingFull { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldKeepUpOnlyOutOfCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldOnSelf { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ShieldOnDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OnlyShieldWhileMoving { get; set; }

        #endregion

        #region Heals
        [Setting]
        [DefaultValue(true)]
        public bool WeaveOGCDHeals { get; set; }

        [Setting]
        [DefaultValue(20.0f)]
        public float WeaveOGCDHealsManaPercent { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int AoeNeedHealingLightParty { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoeNeedHealingFullParty { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DisableSingleHealWhenNeedAoeHealing { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float AoEHealHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HealingBuffsLimitAtOnce { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int HealingBuffsMaxAtOnce { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int HealingBuffsMaxUnderHp { get; set; }

        [Setting]
        [DefaultValue(3)]
        public float HealingBuffsMoreHpNeedHealing { get; set; }

        [Setting]
        [DefaultValue(45.0f)]
        public float HealingBuffsMoreHpHealthPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HealingBuffsOnlyMine { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Diagnosis { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float DiagnosisHpPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiagnosisOnlyBelowXAddersgall { get; set; }

        [Setting]
        [DefaultValue(0)]
        public int DiagnosisOnlyAddersgallValue { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Prognosis { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float PrognosisHpPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool PrognosisOnlyBelowXAddersgall { get; set; }

        [Setting]
        [DefaultValue(0)]
        public int PrognosisOnlyAddersgallValue { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(72.0f)]
        public float EukrasianPrognosisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Zoe { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ZoeEukrasianDiagnosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeEukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoePneuma { get; set; }

        [Setting]
        [DefaultValue(59.0f)]
        public float ZoeHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeMainTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ZoeHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Eukrasia { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianDiagnosis { get; set; }

        [Setting]
        [DefaultValue(69.0f)]
        public float EukrasianDiagnosisHpPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EukrasianDiagnosisOnlyHealer { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EukrasianDiagnosisOnlyTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EukrasianDiagnosisOnlyMainTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Physis { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float PhysisHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Druochole { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float DruocholeHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ixochole { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float IxocholeHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pepsis { get; set; }

        [Setting]
        [DefaultValue(46.0f)]
        public float PepsisHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PepsisEukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(55.0f)]
        public float PepsisEukrasianPrognosisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Taurochole { get; set; }

        [Setting]
        [DefaultValue(71.0f)]
        public float TaurocholeHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TaurocholeTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TaurocholeMainTankOnly { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float TaurocholeOthersHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Haima { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float HaimaHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HaimaTankForBuff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HaimaMainTankForBuff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Panhaima { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PanhaimaHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PanhaimaOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PanhaimaOnlyWithMainTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pneuma { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool OnlyZoePneuma { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool PneumaHealOnly { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PneumaHpPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public float PneumaNeedHealing { get; set; }

        #endregion

        #region Dispels

        [Setting]
        [DefaultValue(true)]
        public bool Dispel { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DispelOnlyAbove { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float DispelOnlyAboveHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AutomaticallyDispelAnythingThatsDispellable { get; set; }


        #endregion

        #region Alliance

        [Setting]
        [DefaultValue(true)]
        public bool IgnoreAlliance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HealAllianceHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HealAllianceTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealAllianceDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResAllianceHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResAllianceTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResAllianceDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HealAllianceOnlyDiagnosis { get; set; }

        #endregion

        #region PVP
        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Kardia { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Eukrasia { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Pneuma { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Toxikon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_PhlegmaIII { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Mesotes { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int Pvp_MesoteNearbyAllies { get; set; }
        #endregion

    }
}
