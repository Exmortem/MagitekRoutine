using System.ComponentModel;
using System.Configuration;
using System.IO;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

namespace Magitek.Models.Scholar
{
    [AddINotifyPropertyChangedInterface]
    public class ScholarSettings : HealerSettings, IRoutineSettings
    {
        public ScholarSettings() : base(CharacterSettingsDirectory + "/Magitek/Scholar/ScholarSettings.json") { }

        public static ScholarSettings Instance { get; set; } = new ScholarSettings();

        #region Buffs
        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float LucidDreamingManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreamingOnlyWhenNoAetherFlow { get; set; }

        [Setting]
        [DefaultValue(ChainStrategemStrategemStrategy.OnlyBosses)]
        public ChainStrategemStrategemStrategy ChainStrategemsStrategy { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Recitation { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RecitationOnlyNoAetherflow { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RecitationWithIndomitability { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RecitationWithLustrate { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RecitationWithExcog { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool RecitationWithAdlo { get; set; }
        #endregion

        #region Healing
        [Setting]
        [DefaultValue(88.0f)]
        public float InterruptHealingPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterruptHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        
        public bool ResOutOfCombat { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool SlowcastRes { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastRes { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Physick { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float PhysickHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Adloquium { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float AdloquiumHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AdloquiumOnlyTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool AdloquiumOnlyHealer { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool AdloquiumTankForBuff { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Lustrate { get; set; }

        [Setting]
        [DefaultValue(45.0f)]
        public float LustrateHpPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool LustrateOnlyTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool LustrateOnlyHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Succor { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float SuccorHpPercent { get; set; }
        
        [Setting]
        [DefaultValue(4)]
        public int SuccorNeedHealing { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool SacredSoil { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float SacredSoilHpPercent { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int SacredSoilNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Indomitability { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float IndomitabilityHpPercent { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int IndomitabilityNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EmergencyTactics { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EmergencyTacticsAdloquium { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EmergencyTacticsSuccor { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float EmergencyTacticsAdloquiumHealthPercent { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float EmergencyTacticsSuccorHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DeploymentTactics { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int DeploymentTacticsAllyInRange { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Excogitation { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ExcogitationOnlyHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ExcogitationOnlyTank { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float ExcogitationHpPercent { get; set; }
        #endregion

        #region Dispelling
        [Setting]
        [DefaultValue(true)]
        public bool Dispel { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DispelOnlyAbove { get; set; }

        [Setting]
        [DefaultValue(85.0f)]
        public float DispelOnlyAboveHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DispelPet { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AutomaticallyDispelAnythingThatsDispellable { get; set; }
        #endregion

        #region Combat
        [Setting]
        [DefaultValue(true)]
        public bool DoDamage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EnergyDrain { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float EnergyDrainManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool StopCastingIfBelowHealthPercent { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float DamageOnlyIfAboveHealthPercent { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float MinimumManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bio { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BioMultipleTargets { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int BioRefreshSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BioUseTimeTillDeath { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int BioDontIfEnemyDyingWithinSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool RuinBroil { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ruin2 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ArtOfWar { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int ArtOfWarEnemies { get; set; }
        #endregion

        #region Pet
        [Setting]
        [DefaultValue(ScholarPets.Eos)]
        public ScholarPets SelectedPet { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool WhisperingDawn { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool WhisperingDawnOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int WhisperingDawnNeedHealing { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float WhisperingDawnHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FeyIllumination { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float FeyIlluminationHpPercent { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int FeyIlluminationNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FeyIlluminationOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool Consolation { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float ConsolationHpPercent { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int ConsolationNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ConsolationOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FeyBlessing { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int FeyBlessingMinimumFairieGauge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FeyBlessingOnlyWithTank { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float FeyBlessingHpPercent { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int FeyBlessingNeedHealing { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SummonSeraph { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float SummonSeraphHpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Aetherpact { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int AetherpactEnemies { get; set; }

        [Setting]
        [DefaultValue(60)]
        public int AetherpactMinimumFairieGauge { get; set; }

        [Setting]
        [DefaultValue(75.0f)]
        public float AetherpactHealthPercent { get; set; }

        [Setting]
        [DefaultValue(100.0f)]
        public float BreakAetherpactHp { get; set; }

        [Setting]
        [DefaultValue(45.0f)]
        public float AetherpactPhysickHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AetherpactUseAdloquium { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float AetherpactAdloquiumHealthPercent { get; set; }
        #endregion

        #region Alliance
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
        public bool HealAllianceOnlyPhysick { get; set; }
        #endregion

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;
            

            LoadFrom(path);
        }
    }
}
