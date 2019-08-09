using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using Clio.Utilities.Collections;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

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
        [DefaultValue(80.0f)]
        public float InterruptHealingHealthPercent { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool InterruptDamageToHeal { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool DoDamage { get; set; }

        [Setting]
        [DefaultValue(20000)]
        public int DotHealthMinimum { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float DotHealthMinimumPercent { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int DotRefreshSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseTimeTillDeathForDots { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int DontDotIfEnemyDyingWithin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Combust { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Gravity { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int GravityEnemies { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float MinimumManaPercentToDoDamage { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int DoDamageIfTimeLeftLessThan { get; set; }

        #endregion

        #region Buffs

        [Setting]
        [DefaultValue(true)]
        public bool Lightspeed { get; set; }

        [Setting]
        [DefaultValue(40.0f)]
        public float LightspeedHealthPercent { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool LightspeedTankOnly { get; set; }

        [Setting]
        [DefaultValue(AstrologianSect.Diurnal)]
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
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float LucidDreamingManaPercent { get; set; }
        
        #endregion

        #region BuffExtenders

        [Setting]
        [DefaultValue(true)]
        public bool CelestialOpposition { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CelestialOppositionAfterCollectiveUnconscious { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool LucidDreamingBeforeCelestialOpposition { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool DiurnalHeliosBeforeCelestialOpposition { get; set; }
        
        [Setting]
        [DefaultValue(60f)]
        public float DiurnalHeliosBeforeCelestialOppositionManaPercent { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CelestialOppositionAfterAoeCard { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CelestialOppositionBalance { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CelestialOppositionBole { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CelestialOppositionArrow { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CelestialOppositionSpear { get; set; }
        
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
        [DefaultValue(40.0f)]
        public float EssentialDignityHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Helios { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int HeliosAllies { get; set; }

        [Setting]
        [DefaultValue(70)]
        public float HeliosHealthPercent { get; set; }
        
        [Setting]
        [DefaultValue(50)]
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
        [DefaultValue(50.0f)]
        public float DiurnalHeliosMinManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalHelios { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int NocturnalHeliosAllies { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float NocturnalHeliosHealthPercent { get; set; }
        
        [Setting]
        [DefaultValue(50.0f)]
        public float NocturnalHeliosMinManaPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBenefic { get; set; }
        
        [Setting]
        [DefaultValue(70.0f)]
        public float DiurnalBeneficMinMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBeneficOnTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
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
        [DefaultValue(60.0f)]
        public float DiurnalBeneficWhileMovingMinMana { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficDontBeneficUnlessUnderTank { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool DiurnalBeneficDontBeneficUnlessUnderHealer { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DiurnalBeneficDontBeneficUnlessUnderDps { get; set; }

        [Setting]
        [DefaultValue(65.0f)]
        public float DiurnalBeneficDontBeneficUnlessUnderHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBenefic { get; set; }
        
        [Setting]
        [DefaultValue(70.0f)]
        public float NocturnalBeneficMinMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBeneficOnTanks { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBeneficOnHealers { get; set; }

        [Setting]
        [DefaultValue(true)]
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
        [DefaultValue(85.0f)]
        public float NocturnalBeneficHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool NocturnalBeneficWhileMoving { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float NocturnalBeneficWhileMovingMinMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float BeneficHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic2 { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float Benefic2HealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Benefic2AlwaysWithEnhancedBenefic2 { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool CollectiveUnconscious { get; set; }

        [Setting]
        [DefaultValue(3)]
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
        [DefaultValue(3)]
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
        [DefaultValue(70.0f)]
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
        public bool Draw { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Play { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PrepCardsOutOfCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool PrepCardsOutOfCombatOnlyWhenPartied { get; set; }

        [Setting]
        [DefaultValue(18)]
        public int DontDrawWhenCombatTimeIs { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CardRuleDefaultToMinorArcana { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool CardRuleDefaultToUndraw { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool LordofCrowns { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool LadyofCrowns { get; set; }
        
        [Setting]
        [DefaultValue(60.0f)]
        public float LadyofCrownsHealthPercent { get; set; }

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

        #region CardRules

        [Setting]
        public ObservableCollection<CardRule> CardRules { get; set; }

        #endregion

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;

            Instance.LoadFrom(path);

        }
    }

    [AddINotifyPropertyChangedInterface]
    public class CardRule
    {
        public int CardPriority { get; set; }
        public CardLogicType LogicType { get; set; }
        public CardPlayType PlayType { get; set; }
        public ActionResourceManager.Astrologian.AstrologianCard Card { get; set; }
        public Conditions Conditions { get; set; }
        public CardAction Action { get; set; }
        public CardTarget Target { get; set; }
        public TargetConditions TargetConditions { get; set; }

        public bool HasConditions => Conditions != null;
    }

    //TODO: Add MinLevel 
    public class Conditions
    {
        public bool? InCombat { get; set; }
        public ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard> HasHeldCard { get; set; } = new AsyncObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>();
        public ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard> DoesntHaveHeldCard { get; set; } = new AsyncObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>();
        public bool? CanSpread { get; set; }
        public ObservableCollection<ClassJobType> JobsNotInParty { get; set; } = new AsyncObservableCollection<ClassJobType>();
        public ObservableCollection<CardRole> RolesNotInParty { get; set; } = new AsyncObservableCollection<CardRole>();
    }

    public class TargetConditions
    {
        public bool? HasTarget { get; set; }
        public float? HpLessThan { get; set; } = 100;
        public float? MpLessThan { get; set; } = 100;
        public float? TpLessThan { get; set; } = 100;
        public ObservableCollection<CardRole> IsRole { get; set; } = new AsyncObservableCollection<CardRole>();
        public ObservableCollection<ClassJobType> JobOrder { get; set; } = new AsyncObservableCollection<ClassJobType>();
        public ObservableCollection<ClassJobType> IsJob { get; set; } = new AsyncObservableCollection<ClassJobType>();
        public CardChoiceType? Choice { get; set; }
        public string PlayerName { get; set; }
        public int? WithAlliesNearbyMoreThan { get; set; } = 0;
    }

    public enum CardLogicType
    {
        Solo = 0,
        Party = 1,
        LargeParty = 2,
        Pvp = 3
    }

    public enum CardPlayType
    {
        Drawn = 0,
        Held = 1
    }

    public enum CardAction
    {
        Play = 0,
        Spread = 1,
        MinorArcana = 2,
        Redraw = 3,
        Undraw = 4,
        UndrawSpread = 5,
        StopLogic = 6
    }

    public enum CardTarget
    {
        Me = 0,
        PartyMember = 1,
    }

    public enum CardRole
    {
        Healer = 0,
        Tank = 1,
        Dps = 2,
    }

    public enum CardChoiceType
    {
        Random = 0
    }
}
