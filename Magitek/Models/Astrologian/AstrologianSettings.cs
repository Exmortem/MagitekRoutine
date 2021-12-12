using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Astrologian
{
    [AddINotifyPropertyChangedInterface]
    public class AstrologianSettings : HealerSettings, IRoutineSettings
    {
        public AstrologianSettings() : base(CharacterSettingsDirectory + "/Magitek/Astrologian/AstrologianSettings.json") { }

        public static AstrologianSettings Instance { get; set; } = new AstrologianSettings();

        #region Combat

        [Setting]
        [DefaultValue(true)]
        public bool Malefic { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterruptHealing { get; set; }

        [Setting]
        [DefaultValue(90.0f)]
        public float InterruptHealingHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterruptDamageToHeal { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DoDamage { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Combust { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool CombustMultipleTargets { get; set; }

        [Setting]
        [DefaultValue(3050)]
        public int CombustRefreshMSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTTDForCombust { get; set; }

        [Setting]
        [DefaultValue(21)]
        public int DontCombustIfEnemyDyingWithin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Gravity { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int GravityEnemies { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float MinimumManaPercentToDoDamage { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int DoDamageIfTimeLeftLessThan { get; set; }

        #endregion

        #region Buffs

        [Setting]
        [DefaultValue(true)]
        public bool Lightspeed { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float LightspeedManaPercent { get; set; }

        //Work in Progress
        [Setting]
        [DefaultValue(true)]
        public bool LightspeedWhileMoving { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float LightspeedHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LightspeedTankOnly { get; set; }

        [Setting]
        [DefaultValue(AstrologianSect.Nocturnal)]
        public AstrologianSect SectWithNoPairing { get; set; }

        [Setting]
        [DefaultValue(AstrologianSect.Nocturnal)]
        public AstrologianSect SectWhenPairedWithWhm { get; set; }

        [Setting]
        [DefaultValue(AstrologianSect.Diurnal)]
        public AstrologianSect SectWhenPairedWithSch { get; set; }

        [Setting]
        [DefaultValue(AstrologianSectWithOpposite.Opposite)]
        public AstrologianSectWithOpposite SectWhenPairedWithAst { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float LucidDreamingManaPercent { get; set; }

        #endregion

        #region Heals

        [Setting]
        [DefaultValue(true)]
        public bool Synastry { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float SynastryHealthPercent { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int SynastryAmountOfPeople { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SynastryTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NeutralSect { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int NeutralSectAllies { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float NeutralSectHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ascend { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AscendSwiftcast { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EssentialDignity { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EssentialDignityTankOnly { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float EssentialDignityHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Helios { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int HeliosAllies { get; set; }

        [Setting]
        [DefaultValue(60)]
        public float HeliosHealthPercent { get; set; }

        [Setting]
        [DefaultValue(40)]
        public float HeliosMinManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalHelios { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int DiurnalHeliosAllies { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float DiurnalHeliosHealthPercent { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float DiurnalHeliosMinManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalHelios { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int NocturnalHeliosAllies { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float NocturnalHeliosHealthPercent { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float NocturnalHeliosMinManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LadyOfCrowns { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float LadyOfCrownsHealthPercent { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int LadyOfCrownsAllies { get; set;}

        [Setting]
        [DefaultValue(3)]
        public int LordOfCrownsEnemies { get; set;}

        [Setting]
        [DefaultValue(true)]
        public bool Horoscope { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int HoroscopeAllies { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float HoroscopeHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CelestialOpposition { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int CelestialOppositionAllies { get; set; }

        [Setting]
        [DefaultValue(75)]
        public float CelestialOppositionHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBenefic { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float DiurnalBeneficMinMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBeneficOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficOnDps { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficKeepUpOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficKeepUpOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficKeepUpOnDps { get; set; }

        [Setting]
        [DefaultValue(85.0f)]
        public float DiurnalBeneficHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBeneficWhileMoving { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float DiurnalBeneficWhileMovingMinMana { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficDontBeneficUnlessUnderTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBeneficDontBeneficUnlessUnderHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBeneficDontBeneficUnlessUnderDps { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float DiurnalBeneficDontBeneficUnlessUnderHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBenefic { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float NocturnalBeneficMinMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBeneficOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool NocturnalBeneficOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool NocturnalBeneficOnDps { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool NocturnalBeneficKeepUpOnTanks { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool NocturnalBeneficKeepUpOnHealers { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool NocturnalBeneficKeepUpOnDps { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float NocturnalBeneficHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBeneficWhileMoving { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float NocturnalBeneficWhileMovingMinMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic { get; set; }

        [Setting]
        [DefaultValue(55.0f)]
        public float BeneficHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic2 { get; set; }

        
        [Setting]
        [DefaultValue(true)]
        public bool NoBeneficIfBenefic2Available { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float Benefic2HealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic2AlwaysWithEnhancedBenefic2 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CelestialIntersection { get; set; }

        [Setting]
        [DefaultValue(85.0f)]
        public float CelestialIntersectionHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CelestialIntersectionTankOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CollectiveUnconscious { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int CollectiveUnconsciousAllies { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float CollectiveUnconsciousHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EarthlyStar { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int EarthlyStarEnemiesNearTarget { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int EarthlyStarPartyMembersNearTarget { get; set; }

        [Setting]
        [DefaultValue(95)]
        public float EarthlyStarPartyMembersNearTargetHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool StellarDetonation { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int EarthlyDominanceCount { get; set; }

        [Setting]
        [DefaultValue(70)]
        public float EarthlyDominanceHealthPercent { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int GiantDominanceCount { get; set; }

        [Setting]
        [DefaultValue(60)]
        public float GiantDominanceHealthPercent { get; set; }


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

        #region AlliancesAndPets

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
        public bool HealAllianceOnlyBenefic { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool HealPartyMembersPets { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool HealPartyMembersPetsTitanOnly { get; set; }

        #endregion

        #region Cards

        [Setting]
        [DefaultValue(true)]
        public bool UseDraw { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseReDraw { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Play { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Divination { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AstroDyne { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int DivinationAllies { get; set; }

        [Setting]
        [DefaultValue(25)]
        public int DontPlayWhenCombatTimeIsLessThan { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CardRuleDefaultToMinorArcana { get; set; }
        #endregion

        #region Card Weights
        [Setting]
        [DefaultValue(1)]
        public int MnkCardWeight { get; set; }
        [Setting]
        [DefaultValue(2)]
        public int BlmCardWeight { get; set; }
        [Setting]
        [DefaultValue(3)]
        public int DrgCardWeight { get; set; }
        [Setting]
        [DefaultValue(4)]
        public int SamCardWeight { get; set; }
        [Setting]
        [DefaultValue(5)]
        public int MchCardWeight { get; set; }
        [Setting]
        [DefaultValue(6)]
        public int SmnCardWeight { get; set; }
        [Setting]
        [DefaultValue(7)]
        public int BrdCardWeight { get; set; }
        [Setting]
        [DefaultValue(8)]
        public int NinCardWeight { get; set; }
        [Setting]
        [DefaultValue(9)]
        public int RdmCardWeight { get; set; }
        [Setting]
        [DefaultValue(10)]
        public int DncCardWeight { get; set; }
        [Setting]
        [DefaultValue(12)]
        public int PldCardWeight { get; set; }
        [Setting]
        [DefaultValue(13)]
        public int WarCardWeight { get; set; }
        [Setting]
        [DefaultValue(14)]
        public int DrkCardWeight { get; set; }
        [Setting]
        [DefaultValue(15)]
        public int GnbCardWeight { get; set; }
        [Setting]
        [DefaultValue(11)]
        public int WhmCardWeight { get; set; }
        [Setting]
        [DefaultValue(16)]
        public int SchCardWeight { get; set; }
        [Setting]
        [DefaultValue(17)]
        public int AstCardWeight { get; set; }
        #endregion

        #region PVP

        [Setting]
        [DefaultValue(true)]
        public bool Disable { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PvpMalefic { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PvpEssentialDignity { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool PvpEssentialDignityTankOnly { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float PvpEssentialDignityHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Purify { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AutomaticallyPurifyAnythingThatsDispellable { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PurifyOnlyAbove { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float PurifyOnlyAboveHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Muse { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PvpLightspeed { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool PvpLightspeedTankOnly { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float PvpLightspeedHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PvpSynastry { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool PvpSynastryTankOnly { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PvpSynastryHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Deorbit { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float DeorbitHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DeborbitOnlyIfTargetedByHostile { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EmpyreanRain { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float EmpyreanRainHealthPercent { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int EmpyreanRainAllies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Recuperate { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float RecuperateHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PvpBenefic { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float PvpBeneficHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PvpBenefic2 { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float PvpBenefic2HealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic2AlwaysWithAbridged { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Concentrate { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int ConcentrateEnemiesTargeting { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float ConcentrateHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Safeguard { get; set; }

        [Setting]
        [DefaultValue(1)]
        public int SafeguardEnemiesTargeting { get; set; }

        #endregion

    }
}
