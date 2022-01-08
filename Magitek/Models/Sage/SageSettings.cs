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

        #region Combat

        [Setting]
        [DefaultValue(true)]
        public bool Dosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterruptHealing { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float InterruptHealingHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterruptDamageToHeal { get; set; }

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
        [DefaultValue(21)]
        public int DontDotIfEnemyDyingWithin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AoE { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AoEEnemies { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float MinimumManaPercentToDoDamage { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int DoDamageIfTimeLeftLessThan { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ToxiconWhileMoving { get; set; }

        #endregion

        #region Buffs

        [Setting]
        [DefaultValue(true)]
        public bool Kardia { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool KardiaSwitchTargets { get; set; }

        [Setting]
        [DefaultValue(85)]
        public float KardiaSwitchTargetsHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KardiaTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool KardiaHealer { get; set; }

        [Setting]
        [DefaultValue(false)]
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
        [DefaultValue(60.0f)]
        public float KrasisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Soteria { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SoteriaTankOnly { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float SoteriaHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Kerachole { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int KeracholeNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KeracholeOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool KeracholeOnlyWithMainTank { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float KeracholeHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Egeiro { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EgeiroSwiftcast { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Rhizomata { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Zoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeDiagnosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeEukrasianDiagnosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoePrognosis { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeEukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float ZoeHealthPercent { get; set; }

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
        [DefaultValue(false)]
        public bool HolosMainTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Shield { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float ShieldHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldKeepUpOnTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldKeepUpOnHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldKeepUpOnDps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldOnTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldOnHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
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
        [DefaultValue(true)]
        public bool Diagnosis { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float DiagnosisHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Prognosis { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int PrognosisNeedHealing { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PrognosisHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int EukrasianPrognosisAllies { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int EukrasianPrognosisNeedHealing { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float EukrasianPrognosisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float EukrasianPrognosisMinManaPercent { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float ZoeDiagnosisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ZoeOnlyTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ZoeOnlyHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Eukrasia { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianDiagnosis { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float EukrasianDiagnosisHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianDiagnosisTankForBuff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EukrasianDiagnosisOnlyHealer { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EukrasianDiagnosisOnlyTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Physis { get; set; }

        [Setting]
        [DefaultValue(85.0f)]
        public float PhysisHpPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int PhysisNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Druochole { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float DruocholeHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ixochole { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float IxocholeHpPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int IxocholeNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pepsis { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PepsisHpPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int PepsisNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PepsisEukrasianPrognosis { get; set; }

        [Setting]
        [DefaultValue(55.0f)]
        public float PepsisEukrasianPrognosisHealthPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int PepsisEukrasianPrognosisNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Taurochole { get; set; }

        [Setting]
        [DefaultValue(45.0f)]
        public float TaurocholeHpPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool TaurocholeTankOnly { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool TaurocholeMainTankOnly { get; set; }

        [Setting]
        [DefaultValue(10.0f)]
        public float TaurocholeOthersHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Haima { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float HaimaHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HaimaTankForBuff { get; set; }

        [Setting]
        [DefaultValue(false)]
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
        [DefaultValue(false)]
        public bool PanhaimaOnlyWithMainTank { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int PanhaimaNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pneuma { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool OnlyZoePneuma { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PneumaHealOnly { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PneumaHpPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int PneumaNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastRes { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SlowcastRes { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResOutOfCombat { get; set; }

        #endregion

        #region Dispels

        [Setting]
        [DefaultValue(true)]
        public bool Dispel { get; set; }

        [Setting]
        [DefaultValue(true)]
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
        [DefaultValue(true)]
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
        [DefaultValue(false)]
        public bool HealAllianceOnlyDiagnosis { get; set; }

        #endregion



    }
}
