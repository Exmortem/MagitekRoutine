using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using static Magitek.Extensions.GameObjectExtensions;

namespace Magitek.Logic.Astrologian
{
    internal static class Heals
    {

        #region Single Target No Regen Heals
        public static async Task<bool> Benefic()
        {
            if (!AstrologianSettings.Instance.Benefic)
                return false;

            if (Globals.InParty)
            {
                foreach (var ally in Group.CastableAlliesWithin30)
                {
                    if (Utilities.Routines.Astrologian.DontBenefic.Contains(ally.Name))
                        continue;

                    if (ally.CheckTankImmunity() == TankImmunityCheck.DontHealThem)
                        continue;

                    if (ally.CurrentHealthPercent > AstrologianSettings.Instance.BeneficHealthPercent
                        || ally.CurrentHealth <= 0)
                        continue;

                    if (!ally.HasAura(Auras.AspectedBenefic))
                        return await CastBenefic(ally);

                    if (!AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderDps
                        && ally.IsDps())
                        return await CastBenefic(ally);

                    if (!AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderHealer
                        && ally.IsHealer())
                        return await CastBenefic(ally);

                    if (!AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderTank
                        && ally.IsTank())
                        return await CastBenefic(ally);

                    if (AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderDps
                        && ally.IsDps()
                        && ally.CurrentHealthPercent < AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderHealth)
                        return await CastBenefic(ally);

                    if (AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderHealer && ally.IsHealer()
                        && ally.CurrentHealthPercent < AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderHealth)
                        return await CastBenefic(ally);

                    if (AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderTank && ally.IsTank()
                        && ally.CurrentHealthPercent < AstrologianSettings.Instance.DiurnalBeneficDontBeneficUnlessUnderHealth)
                        return await CastBenefic(ally);
                }

                return false;
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.BeneficHealthPercent)
                    return false;

                if (Spells.Benefic2.IsKnownAndReady())
                {
                    if (Core.Me.HasAura(Auras.EnhancedBenefic2)
                        && AstrologianSettings.Instance.Benefic2AlwaysWithEnhancedBenefic2
                        && Core.Me.CurrentManaPercent >= Spells.Benefic2.Cost)
                        return await Spells.Benefic2.Heal(Core.Me);

                    if (Core.Me.CurrentHealthPercent <= AstrologianSettings.Instance.Benefic2HealthPercent
                        && Core.Me.CurrentManaPercent >= Spells.Benefic2.Cost)
                        return await Spells.Benefic2.Heal(Core.Me);
                }

                return await Spells.Benefic.Heal(Core.Me);
            }

            async Task<bool> CastBenefic(GameObject ally)
            {
                if (AstrologianSettings.Instance.NoBeneficIfBenefic2Available)
                    if (Core.Me.ClassLevel >= Spells.Benefic2.LevelAcquired && AstrologianSettings.Instance.Benefic2)
                        return await Spells.Benefic2.Heal(ally);

                return await Spells.Benefic.Heal(ally);
            }
        }

        public static async Task<bool> Benefic2()
        {
            if (!AstrologianSettings.Instance.Benefic2)
                return false;

            if (!Spells.Benefic2.IsKnownAndReady())
                return false;

            var shouldBenefic2WithEnhancedBenefic2 = AstrologianSettings.Instance.Benefic2AlwaysWithEnhancedBenefic2
                && Core.Me.CurrentManaPercent >= Spells.Benefic2.Cost;

            if (Globals.InParty)
            {
                // Added this to test (Exmortem)
                if (Casting.LastSpell == Spells.Benefic2)
                {
                    if (Casting.LastSpellTarget != Globals.HealTarget)
                    {
                        if (Core.Me.HasAura(Auras.EnhancedBenefic2)
                            && Globals.HealTarget?.CurrentHealthPercent <= AstrologianSettings.Instance.BeneficHealthPercent
                            && shouldBenefic2WithEnhancedBenefic2)
                        {
                            return await Spells.Benefic2.Heal(Globals.HealTarget);
                        }
                    }
                }

                if (Core.Me.CurrentHealthPercent < AstrologianSettings.Instance.Benefic2HealthPercent)
                    return await Spells.Benefic2.Heal(Core.Me);

                var benefic2Target = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontBenefic2.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.Benefic2HealthPercent
                && r.CheckTankImmunity() == TankImmunityCheck.HealThem);

                if (benefic2Target == null)
                    return false;

                // Added this to test (Exmortem)
                if (Casting.LastSpell == Spells.Benefic2)
                {
                    if (Casting.LastSpellTarget == benefic2Target)
                        return false;
                }

                return await Spells.Benefic2.Heal(benefic2Target);
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.Benefic2HealthPercent)
                    return false;

                return await Spells.Benefic2.Heal(Core.Me);
            }
        }

        public static async Task<bool> DontLetTheDrkDie()
        {
            if (!AstrologianSettings.Instance.DontLetTheDRKDie)
                return false;

            if (!Globals.InParty)
                return false;
            
            if (!Globals.PartyInCombat)
                return false;

            var walkingDeadMan = Group.CastableTanks.FirstOrDefault(r =>
                !Utilities.Routines.Astrologian.DontBenefic2.Contains(r.Name)
                && r.HasAura(Auras.WalkingDead) 
                && r.CurrentHealthPercent < 100);

            if (walkingDeadMan == null)
                return false;

            return await Spells.Benefic2.Heal(walkingDeadMan);
        }

        public static async Task<bool> CelestialIntersection()
        {
            if (!AstrologianSettings.Instance.CelestialIntersection)
                return false;
            
            if (!Globals.PartyInCombat)
                return false;
            
            if (!Spells.CelestialIntersection.IsKnownAndReady())
                return false;

            if (Casting.LastSpell == Spells.CelestialIntersection)
                return false;

            if (AstrologianSettings.Instance.CelestialIntersectionTankOnly)
            {
                var celestialIntersectionTank = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontCelestialIntersection.Contains(r.Name)
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.CelestialIntersectionHealthPercent
                && Combat.Enemies.Any(x => x.TargetCharacter == r)
                && !r.HasAura(Auras.CelestialIntersection)
                && r.CheckTankImmunity() == TankImmunityCheck.HealThem);

                if (celestialIntersectionTank == null)
                    return false;
                
                return await Spells.CelestialIntersection.Heal(celestialIntersectionTank, false);
            }
            
            var celestialIntersectionTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontCelestialIntersection.Contains(r.Name)
            && r.CurrentHealthPercent <= AstrologianSettings.Instance.CelestialIntersectionHealthPercent
            && !r.HasAura(Auras.CelestialIntersection)
            && r.CheckTankImmunity() == TankImmunityCheck.HealThem);
            
            if (celestialIntersectionTarget == null)
                return false;

            return await Spells.CelestialIntersection.Heal(celestialIntersectionTarget);
        }

        public static async Task<bool> EssentialDignity()
        {
            if (!AstrologianSettings.Instance.EssentialDignity)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Globals.InParty)
            {
                if (AstrologianSettings.Instance.EssentialDignityTankOnly)
                {
                    var tar = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontEssentialDignity.Contains(r.Name)
                    && r.IsAlive
                    && r.CurrentHealthPercent <= AstrologianSettings.Instance.EssentialDignityHealthPercent
                    && r.CheckTankImmunity() == TankImmunityCheck.HealThem);

                    if (tar == null)
                        return false;

                    return await Spells.EssentialDignity.Heal(tar, false);
                }

                if (Core.Me.CurrentHealthPercent < AstrologianSettings.Instance.EssentialDignityHealthPercent)
                    return await Spells.EssentialDignity.Heal(Core.Me, false);

                var essentialDignityTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontEssentialDignity.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.EssentialDignityHealthPercent
                && r.CheckTankImmunity() == TankImmunityCheck.HealThem);

                if (essentialDignityTarget == null)
                    return false;

                return await Spells.EssentialDignity.Heal(essentialDignityTarget);
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.EssentialDignityHealthPercent)
                    return false;

                return await Spells.EssentialDignity.Heal(Core.Me, false);
            }
        }

        public static async Task<bool> Exaltation()
        {
            if (!AstrologianSettings.Instance.Exaltation)
                return false;

          
            if (!Globals.InParty)
                return false;

            if (!Spells.Exaltation.IsKnownAndReady())
                return false;

            if (AstrologianSettings.Instance.FightLogic_Exaltation)
            {

                var tankBusterOnPartyMember = FightLogic.EnemyIsCastingTankBuster();

                if (tankBusterOnPartyMember == null)
                    return false;

                return await FightLogic.DoAndBuffer(
                    Spells.Exaltation.HealAura(tankBusterOnPartyMember, Auras.Exaltation));
            }

            var tankToShieldAndHeal = Group.CastableTanks.FirstOrDefault(x =>
                x.CurrentHealthPercent < AstrologianSettings.Instance.ExaltationHealthPercent &&
                x.CheckTankImmunity() == TankImmunityCheck.HealThem);
            
            if (tankToShieldAndHeal == null)
                return false;

            return await Spells.Exaltation.HealAura(tankToShieldAndHeal, Auras.Exaltation);
        }

        #endregion

        #region AOE No Regen Heals

        public static async Task<bool> Helios()
        {
            if (!AstrologianSettings.Instance.Helios)
                return false;

            if (Core.Me.CurrentManaPercent <= AstrologianSettings.Instance.HeliosMinManaPercent)
                return false;

            var heliosCount = PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r => r.CurrentHealth > 0
            && r.Distance(Core.Me) < Spells.Helios.Radius
            && r.CurrentHealthPercent <= r.AdjustHealthThresholdByRegen(AstrologianSettings.Instance.HeliosHealthPercent));

            //if (heliosCount < AstrologianSettings.Instance.HeliosAllies)
            if (heliosCount <= AoeThreshold)
                return false;

            return await Spells.Helios.Heal(Core.Me, false);
        }

        public static async Task<bool> LadyOfCrowns()
        {
            if (!Core.Me.HasAura(Auras.LadyOfCrownsDrawn))
                return false;

            if (!AstrologianSettings.Instance.LadyOfCrowns)
                return false;

            if (!Globals.InParty && Core.Me.CurrentHealthPercent <= Core.Me.AdjustHealthThresholdByRegen(AstrologianSettings.Instance.LadyOfCrownsHealthPercent))
                return await Spells.CrownPlay.Heal(Core.Me);

            if (Group.CastableAlliesWithin20.Count(r => r.CurrentHealthPercent <= r.AdjustHealthThresholdByRegen(AstrologianSettings.Instance.LadyOfCrownsHealthPercent)) <= AstrologianSettings.Instance.LadyOfCrownsAllies)
                return false;

            return await Spells.CrownPlay.Heal(Core.Me);
        }

        #endregion

        #region Single Target Regen Heals

        public static async Task<bool> AspectedBenefic()
        {
            if (!AstrologianSettings.Instance.DiurnalBenefic)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (MovementManager.IsMoving)
            {
                if (!AstrologianSettings.Instance.DiurnalBeneficWhileMoving)
                    return false;

                if (Core.Me.CurrentManaPercent <= AstrologianSettings.Instance.DiurnalBeneficWhileMovingMinMana)
                    return false;
            }

            if (Core.Me.CurrentManaPercent < AstrologianSettings.Instance.DiurnalBeneficMinMana)
                return false;

            if (Globals.InParty)
            {
                if (await AspectedBeneficTanks())
                    return true;
                if (await AspectHeliosInsteadOfDiurnalBenefic())
                    return true;
                if (await AspectedBeneficHealers())
                    return true;
                return await AspectedBeneficDps();
            }
            else
            {
                if (Core.Me.HasAura(Auras.AspectedBenefic))
                    return false;

                if (!AstrologianSettings.Instance.DiurnalBeneficKeepUpOnHealers
                    && Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.DiurnalBeneficHealthPercent)
                    return false;

                return await Spells.AspectedBenefic.HealAura(Core.Me, Auras.AspectedBenefic);
            }
        }

        private static async Task<bool> AspectedBeneficTanks()
        {
            if (Core.Me.HasAllAuras(new List<uint>{Auras.NeutralSect,Auras.NeutralSectShield}))
            {
                var tankShieldTarget = Group.CastableTanks.FirstOrDefault(x => !x.HasAura(Auras.NeutralSectShield));

                if (tankShieldTarget != null)
                    return await Spells.AspectedBenefic.HealAura(tankShieldTarget,Auras.NeutralSectShield);
            }

            if (!AstrologianSettings.Instance.DiurnalBeneficOnTanks)
                return false;

            var diurnalBeneficTarget = AstrologianSettings.Instance.DiurnalBeneficKeepUpOnTanks ?
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.HasMyAura(Auras.AspectedBenefic)) :
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.HasAura(Auras.AspectedBenefic)
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.DiurnalBeneficHealthPercent);

            if (diurnalBeneficTarget == null)
                return false;

            return await Spells.AspectedBenefic.HealAura(diurnalBeneficTarget, Auras.AspectedBenefic);
        }

        private static async Task<bool> AspectHeliosInsteadOfDiurnalBenefic()
        {
            if (!AstrologianSettings.Instance.DiurnalHelios)
                return false;

            // Add check to ensure we don't double cast
            if (Casting.LastSpell == Spells.AspectedHelios)
                return false;

            if (!Spells.AspectedHelios.IsKnown())
                return false;

            var alliesNeedingRegen = Group.CastableAlliesWithin15.Where(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.HasMyAura(Auras.AspectedBenefic)
                && !r.HasMyAura(Auras.AspectedHelios)
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.DiurnalBeneficHealthPercent).ToList();

            if (alliesNeedingRegen.Count() <= AoeThreshold)
                return false;

            return await Spells.AspectedHelios.HealAura(Core.Me, Auras.AspectedHelios);
        }

        private static async Task<bool> AspectedBeneficHealers()
        {
            if (!AstrologianSettings.Instance.DiurnalBeneficOnHealers)
                return false;

            var aspectedBeneficTarget = AstrologianSettings.Instance.DiurnalBeneficKeepUpOnHealers
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.IsHealer()
                && !r.HasMyAura(Auras.AspectedBenefic))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.IsHealer()
                && !r.HasMyAura(Auras.AspectedBenefic)
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.DiurnalBeneficHealthPercent);

            if (aspectedBeneficTarget == null)
                return false;

            return await Spells.AspectedBenefic.HealAura(aspectedBeneficTarget, Auras.AspectedBenefic);
        }

        private static async Task<bool> AspectedBeneficDps()
        {
            if (!AstrologianSettings.Instance.DiurnalBeneficOnDps)
                return false;

            var aspectedBeneficTarget = AstrologianSettings.Instance.DiurnalBeneficKeepUpOnDps
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.IsTank()
                && !r.IsHealer()
                && !r.HasMyAura(Auras.AspectedBenefic))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.IsTank()
                && !r.IsHealer()
                && !r.HasMyAura(Auras.AspectedBenefic)
                && r.CurrentHealthPercent <= r.AdjustHealthThresholdByRegen(AstrologianSettings.Instance.DiurnalBeneficHealthPercent));

            if (aspectedBeneficTarget == null)
                return false;

            return await Spells.AspectedBenefic.HealAura(aspectedBeneficTarget, Auras.AspectedBenefic);
        }



        #endregion

        #region Aoe Regen Heals
        public static async Task<bool> AspectedHelios()
        {
            if (!AstrologianSettings.Instance.DiurnalHelios)
                return false;

            if (!Spells.AspectedHelios.IsKnownAndReady())
                return false;

            if (Core.Me.HasAura(Auras.NeutralSect) &&
                Group.CastableAlliesWithin15.Count(x => !x.HasAura(Auras.NeutralSectShield)) >= AoeThreshold && !Core.Me.HasAura(Auras.NeutralSectShield))
                return MovementManager.IsMoving
                    ? await SwiftCastAspectedHelios()
                    : await Spells.AspectedHelios.HealAura(Core.Me, Auras.NeutralSectShield, false);
            
            if (Casting.LastSpell == Spells.AspectedHelios)
                return false;

            if (Core.Me.CurrentManaPercent <= AstrologianSettings.Instance.DiurnalHeliosMinManaPercent)
                return false;
            
            var diurnalHeliosCount =
                PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r => r.CurrentHealth > 0 &&
                                                        r.Distance(Core.Me) <= Spells.AspectedHelios.Radius &&
                                                        r.CurrentHealthPercent <=
                                                        AstrologianSettings.Instance.DiurnalHeliosHealthPercent &&
                                                        !r.HasAura(Auras.AspectedHelios, true));

            if (diurnalHeliosCount >= AstrologianSettings.Instance.DiurnalHeliosAllies)
            {
                if (await SwiftCastAspectedHelios())
                    return true;

                return await Spells.AspectedHelios.HealAura(Core.Me, Auras.AspectedHelios);            
            }
            return false;
        }

        private static async Task<bool> SwiftCastAspectedHelios()
        {
            if (!Spells.Swiftcast.IsKnownAndReady())
                return false;

            if (!await Buff.Swiftcast())
                return false;

            while (Core.Me.HasAura(Auras.Swiftcast))
            {
                if (await Spells.AspectedHelios.HealAura(Core.Me, Auras.AspectedHelios, false))
                    return true;

                await Coroutine.Yield();
            }

            return false;
        }

        public static async Task<bool> CelestialOpposition()
        {
            if (!AstrologianSettings.Instance.CelestialOpposition)
                return false;

           if (!Spells.CelestialOpposition.IsKnownAndReady())
              return false;

            if (Casting.LastSpell == Spells.Horoscope)
                return false;

            var celestialOppositionCount = Group.CastableAlliesWithin15.Count(r => r.CurrentHealth > 0
            && r.CurrentHealthPercent <= AstrologianSettings.Instance.CelestialOppositionHealthPercent);

            if (celestialOppositionCount < AstrologianSettings.Instance.CelestialOppositionAllies)
                return false;

            return await Spells.CelestialOpposition.HealAura(Core.Me, Auras.Opposition, false);
        }

        public static async Task<bool> CollectiveUnconscious()
        {
            if (!AstrologianSettings.Instance.CollectiveUnconscious)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Spells.CollectiveUnconscious.IsKnownAndReady())
                return false;

            if (AstrologianSettings.Instance.FightLogic_CollectiveUnconscious && FightLogic.EnemyIsCastingAoe() &&
                Group.CastableAlliesWithin15.Count(x => x.WithinSpellRange(Spells.CollectiveUnconscious.Radius)) >
                AoeThreshold)
                return await FightLogic.DoAndBuffer(
                    Spells.CollectiveUnconscious.HealAura(Core.Me, Auras.CollectiveUnconsciousMitigation));


            if (Group.CastableAlliesWithin10.Count(r => r.Distance() < 6
                                                    && r.IsAlive
                                                    && r.CurrentHealthPercent <= AstrologianSettings.Instance.CollectiveUnconsciousHealth)
                                                    < AstrologianSettings.Instance.CollectiveUnconsciousAllies)
                return false;

            return await Spells.CollectiveUnconscious.HealAura(Core.Me, Auras.WheelOfFortune, false);
        }

        #endregion

        #region Delayed Heals
        public static async Task<bool> EarthlyStar()
        {
            if (!Spells.EarthlyStar.IsKnownAndReady())
                return false;

            if (!Core.Me.InCombat)
                return false;


            var earthlyStarLocation = Utilities.Routines.Astrologian.EarthlyStarLocation;

            var earthlyStarTargets = PartyManager.VisibleMembers.Select(r => r.BattleCharacter).ToList();

            if (Core.Me.HasAura(Auras.EarthlyDominance)
                && Utilities.Routines.Astrologian.EarthlyStarLocation != Vector3.Zero
                && AstrologianSettings.Instance.StellarDetonation)
            {
                if (earthlyStarTargets.Count(r => r.Distance(earthlyStarLocation) <= 30
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.EarthlyDominanceHealthPercent) > AstrologianSettings.Instance.EarthlyDominanceCount)
                    return await Spells.StellarDetonation.Heal(Core.Me);
            }

            if (Core.Me.HasAura(Auras.GiantDominance)
                && Utilities.Routines.Astrologian.EarthlyStarLocation != Vector3.Zero
                && AstrologianSettings.Instance.StellarDetonation)
            {
                if (earthlyStarTargets.Count(r => r.Distance(earthlyStarLocation) <= 30
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.GiantDominanceHealthPercent) > AstrologianSettings.Instance.GiantDominanceCount)
                    return await Spells.StellarDetonation.Heal(Core.Me);
            }

            if (!AstrologianSettings.Instance.EarthlyStar)
                return false;


            if (!Core.Me.HasTarget)
                return false;

            switch (Globals.InParty)
            {
                case true:
                    if (Core.Target.EnemiesNearby(30).Count() > AstrologianSettings.Instance.EarthlyStarEnemiesNearTarget
                        && earthlyStarTargets.Count(r => r.Distance(Core.Target) <= 30
                        && r.CurrentHealthPercent <= AstrologianSettings.Instance.EarthlyStarPartyMembersNearTargetHealthPercent) > AstrologianSettings.Instance.EarthlyStarPartyMembersNearTarget)
                        if (await Spells.EarthlyStar.Cast(Core.Target))
                        {
                            Utilities.Routines.Astrologian.EarthlyStarLocation = Core.Target.Location;
                            return true;
                        }
                    break;
                default:
                    if (Core.Target.EnemiesNearby(30).Count() > AstrologianSettings.Instance.EarthlyStarEnemiesNearTarget
                        && Core.Me.CurrentHealthPercent <= AstrologianSettings.Instance.EarthlyStarPartyMembersNearTargetHealthPercent
                        && Core.Target.Distance() <= 30)
                        if (await Spells.EarthlyStar.Cast(Core.Target))
                        {
                            Utilities.Routines.Astrologian.EarthlyStarLocation = Core.Target.Location;
                            return true;
                        }
                    break;
            }
            return false;
        }

        public static async Task<bool> Horoscope()
        {
            if (!AstrologianSettings.Instance.Horoscope)
                return false;

            if (Group.CastableAlliesWithin20.Count(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.HoroscopeHealthPercent) < AstrologianSettings.Instance.HoroscopeAllies)
                return false;

            if (await Spells.Horoscope.Cast(Core.Me))
                if (!await AspectedHelios())
                    return await Spells.Helios.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> HoroscopePop()
        {
            if (!AstrologianSettings.Instance.Horoscope)
                return false;
                        
            if (Group.CastableAlliesWithin20.Count(r => r.HasAura(Auras.HoroscopeHelios) && r.CurrentHealthPercent <= AstrologianSettings.Instance.HoroscopeHealthPercent) < AstrologianSettings.Instance.HoroscopeAllies)
                return false;

            return await Spells.Horoscope.Cast(Core.Me);
        }

        public static async Task<bool> Macrocosmos()
        {
            if (!AstrologianSettings.Instance.Macrocosmos)
                return false;
            
            if (!Core.Me.InCombat)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Spells.Macrocosmos.IsKnown())
                return false;

            if (Core.Me.HasMyAura(Auras.Macrocosmos))
                return await Microcosmos();

            if (!Spells.Macrocosmos.IsReady())
                return false;

            if (Group.CastableAlliesWithin20.Any(x => x.HasAura(Auras.Macrocosmos)))
                return false;

            if (AstrologianSettings.Instance.FightLogic_Macrocosmos && FightLogic.EnemyIsCastingBigAoe())
                return await FightLogic.DoAndBuffer(Spells.Macrocosmos.HealAura(Core.Me, Auras.Macrocosmos));

            var enemyCount = Combat.Enemies.Count();
            if (enemyCount == 0)
                return false;

            if (enemyCount > PartyManager.NumMembers)
                if (Combat.Enemies.All(x => x.WithinSpellRange(Spells.Macrocosmos.Radius) && Group.CastableAlliesWithin20.Count() == PartyManager.NumMembers))
                    return await Spells.Macrocosmos.HealAura(Core.Me, Auras.Macrocosmos);

            var isThereABoss = Combat.Enemies.Any(x => x.IsBoss());

            if (isThereABoss || !Group.CastableTanks.All(x =>
                    x.Distance() <= Spells.Macrocosmos.Radius && x.CurrentHealthPercent < 30f) ||
                enemyCount <= AoeThreshold) return false;

            return await Spells.Macrocosmos.HealAura(Core.Me, Auras.Macrocosmos);
        }

        private static async Task<bool> Microcosmos()
        {
            if (!Group.CastableAlliesWithin30.Any(x => x.HasMyAura(Auras.Macrocosmos)))
                return false;

            if (Group.CastableAlliesWithin30.Count(x => x.HasMyAura(Auras.Macrocosmos)
                    && x.CurrentHealthPercent < AstrologianSettings.Instance.MacrocosmosHealthPercent) <= AoeThreshold) return false;

            return await Spells.Microcosmos.Heal(Core.Me);

        }

        #endregion

        #region Raise

        public static async Task<bool> Ascend()
        {
            return await Roles.Healer.Raise(
                Spells.Ascend,
                AstrologianSettings.Instance.SwifcastRes,
                AstrologianSettings.Instance.SlowcastRes,
                AstrologianSettings.Instance.ResOutOfCombat
            );
        }

        #endregion

        public static int AoeThreshold => PartyManager.NumMembers == 4 ? 2 : 3;
    }
}
