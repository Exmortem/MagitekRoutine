using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Astrologian
{
    public class Heal
    {
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

                    if (ally.CurrentHealthPercent > AstrologianSettings.Instance.BeneficHealthPercent
                        || ally.CurrentHealth <= 0)
                        continue;

                    if (Core.Me.Sect() == AstrologianSect.Diurnal)
                    {
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
                    else
                    {
                        return await CastBenefic(ally);
                    }
                }

                return false;
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.BeneficHealthPercent)
                    return false;

                if (Core.Me.HasAura(Auras.EnhancedBenefic2)
                    && AstrologianSettings.Instance.Benefic2AlwaysWithEnhancedBenefic2
                    && Core.Me.CurrentManaPercent >= Spells.Benefic2.Cost)
                    return await Spells.Benefic2.Heal(Core.Me);

                if (Core.Me.CurrentHealthPercent <= AstrologianSettings.Instance.Benefic2HealthPercent
                    && Core.Me.CurrentManaPercent >= Spells.Benefic2.Cost)
                    return await Spells.Benefic2.Heal(Core.Me);

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

                var benefic2Target = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontBenefic2.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.Benefic2HealthPercent);

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


        public static async Task<bool> CelestialIntersection()
        {
            if (!AstrologianSettings.Instance.CelestialIntersection)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Spells.CelestialIntersection.Cooldown != TimeSpan.Zero)
                return false;
            
            if (AstrologianSettings.Instance.CelestialIntersectionTankOnly)
            {
                var celestialIntersectionTank = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontCelestialIntersection.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.CelestialIntersectionHealthPercent);

                if (celestialIntersectionTank == null)
                    return false;

                return await Spells.CelestialIntersection.Heal(celestialIntersectionTank, false);
            }

            var celestialIntersectionTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontCelestialIntersection.Contains(r.Name)
            && r.CurrentHealth > 0
            && r.CurrentHealthPercent <= AstrologianSettings.Instance.CelestialIntersectionHealthPercent);

            if (celestialIntersectionTarget == null)
                return false;

            return await Spells.CelestialIntersection.Cast(celestialIntersectionTarget);
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
                    && r.CurrentHealthPercent <= AstrologianSettings.Instance.EssentialDignityHealthPercent);

                    if (tar == null)
                        return false;

                    return await Spells.EssentialDignity.Heal(tar, false);
                }

                var essentialdignitytarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontEssentialDignity.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.EssentialDignityHealthPercent);

                if (essentialdignitytarget == null)
                    return false;

                return await Spells.EssentialDignity.Heal(essentialdignitytarget);
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.EssentialDignityHealthPercent)
                    return false;

                return await Spells.EssentialDignity.Heal(Core.Me, false);
            }
        }
        public static async Task<bool> CelestialOpposition()
        {
            if (!AstrologianSettings.Instance.CelestialOpposition)
                return false;

            if (Spells.CelestialOpposition.Cooldown != TimeSpan.Zero)
                return false;

            if (Casting.LastSpell == Spells.Helios)
                return false;

            if (Casting.LastSpell == Spells.AspectedHelios)
                return false;

            if (Casting.LastSpell == Spells.CelestialOpposition)
                return false;

            if (Casting.LastSpell == Spells.Horoscope)
                return false;

            var celestialOppositionCount = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0
            && r.Distance(Core.Me) <= 15
            && r.CurrentHealthPercent <= AstrologianSettings.Instance.CelestialOppositionHealthPercent);

            if (celestialOppositionCount < AstrologianSettings.Instance.CelestialOppositionAllies)
                return false;

            return await Spells.CelestialOpposition.Heal(Core.Me, false);
        }

        public static async Task<bool> LadyOfCrowns()
        {
            if (!Core.Me.HasAura(Auras.LadyOfCrownsDrawn))
                return false;

            if (!AstrologianSettings.Instance.LadyOfCrowns)
                return false;

            if (!Globals.InParty && Core.Me.CurrentHealthPercent <= AstrologianSettings.Instance.LadyOfCrownsHealthPercent)
                return await Spells.CrownPlay.Heal(Core.Me);

            if (Group.CastableAlliesWithin20.Count(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.LadyOfCrownsHealthPercent) <= AstrologianSettings.Instance.LadyOfCrownsAllies)
                return false; 

            return await Spells.CrownPlay.Heal(Core.Me);
        }
        public static async Task<bool> Horoscope()
        {
            if (!AstrologianSettings.Instance.Horoscope)
                return false;

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.HoroscopeHealthPercent) < AstrologianSettings.Instance.HoroscopeAllies)
                return false;

            if (await Spells.Horoscope.Cast(Core.Me))
                if (!await AspectedHelios())
                    return await Spells.Helios.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> HoroscopePop()
        {
            if (AstrologianSettings.Instance.Horoscope)
                return false;

            if (!Core.Me.HasAura(Auras.HoroscopeHelios))
                return false;

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.HoroscopeHealthPercent) < AstrologianSettings.Instance.HoroscopeAllies)
                return false;

            return await Spells.Horoscope.Cast(Core.Me);
        }
        public static async Task<bool> Helios()
        {
            if (!AstrologianSettings.Instance.Helios)
                return false;

            if (Casting.LastSpell == Spells.Helios)
                return false;

            if (Core.Me.CurrentManaPercent <= AstrologianSettings.Instance.HeliosMinManaPercent) return false;

            var heliosCount = PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r => r.CurrentHealth > 0
            && r.Distance(Core.Me) <= Spells.Helios.Radius
            && r.CurrentHealthPercent <= AstrologianSettings.Instance.HeliosHealthPercent);

            if (heliosCount < AstrologianSettings.Instance.HeliosAllies)
                return false;

            return await Spells.Helios.Heal(Core.Me, false);
        }

        public static async Task<bool> AspectedHelios()
        {           
            if (!AstrologianSettings.Instance.DiurnalHelios)
                return false;

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

            if (diurnalHeliosCount < AstrologianSettings.Instance.DiurnalHeliosAllies)
                return false;

            return await Spells.AspectedHelios.HealAura(Core.Me, Auras.AspectedHelios);
        }

        
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
                if (await DiurnalBeneficTanks())
                    return true;
                if (await AspectHeliosInsteadOfDiurnalBenefic())
                    return true;
                if (await DiurnalBeneficHealers())
                    return true;
                return await DiurnalBeneficDps();
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

        private static async Task<bool> DiurnalBeneficTanks()
        {
            if (!AstrologianSettings.Instance.DiurnalBeneficOnTanks)
                return false;

            var diurnalBeneficTarget = AstrologianSettings.Instance.DiurnalBeneficKeepUpOnTanks ?
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.HasAura(Auras.AspectedBenefic)
                && !r.HasMyRegen()) :
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.HasAura(Auras.AspectedBenefic)
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.DiurnalBeneficHealthPercent
                && !r.HasMyRegen());

            if (diurnalBeneficTarget == null)
                return false;

            return await Spells.AspectedBenefic.HealAura(diurnalBeneficTarget, Auras.AspectedBenefic);
        }

        private static async Task<bool> AspectHeliosInsteadOfDiurnalBenefic()
        {
            if (!AstrologianSettings.Instance.DiurnalHelios)
                return false;

            var heliosInsteadThreshold = Math.Round(Group.AllianceMembers.Count() * .6,0);

            var alliesNeedingRegen = Group.CastableAlliesWithin15.Where(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.HasAura(Auras.AspectedBenefic)
                && !r.HasAura(Auras.AspectedHelios)
                && !r.HasMyRegen()).ToList();

            if (alliesNeedingRegen.Count() < heliosInsteadThreshold)
                return false;

            return await Spells.AspectedHelios.HealAura(Core.Me, Auras.AspectedHelios);
        }

        private static async Task<bool> DiurnalBeneficHealers()
        {
            if (!AstrologianSettings.Instance.DiurnalBeneficOnHealers)
                return false;

            var diurnalBeneficTarget = AstrologianSettings.Instance.DiurnalBeneficKeepUpOnHealers
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.IsHealer()
                && !r.HasMyRegen())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && r.IsHealer()
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.DiurnalBeneficHealthPercent
                && !r.HasMyRegen());

            if (diurnalBeneficTarget == null)
                return false;

            return await Spells.AspectedBenefic.HealAura(diurnalBeneficTarget, Auras.AspectedBenefic);
        }

        private static async Task<bool> DiurnalBeneficDps()
        {
            if (!AstrologianSettings.Instance.DiurnalBeneficOnDps)
                return false;

            var diurnalBeneficTarget = AstrologianSettings.Instance.DiurnalBeneficKeepUpOnDps
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.IsTank()
                && !r.IsHealer()
                && !r.HasMyRegen())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontDiurnalBenefic.Contains(r.Name)
                && r.CurrentHealth > 0
                && !r.IsTank()
                && !r.IsHealer()
                && !r.HasMyRegen()
                && r.CurrentHealthPercent <= AstrologianSettings.Instance.DiurnalBeneficHealthPercent);

            if (diurnalBeneficTarget == null)
                return false;

            return await Spells.AspectedBenefic.HealAura(diurnalBeneficTarget, Auras.AspectedBenefic);
        }

        public static async Task<bool> Ascend()
        {
            if (!AstrologianSettings.Instance.Ascend)
                return false;

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentMana < Spells.Ascend.Cost)
                return false;

            /*
            if (Group.DeadAllies.Any())
            {
                Logger.WriteInfo(
                    @"========================================Dead Guy Logger========================================");
                var deadguycount = 0;
                foreach (var deadguy in Group.DeadAllies)
                {
                    deadguycount++;
                    Logger.WriteInfo($@"{deadguycount}" + "\t" + $@"{deadguy.Name}" + "\t" +
                                     $@"DoesntHaveRaiseAura: {!deadguy.HasAura(Auras.Raise)}" + "\t" +
                                     $@"Distance: {deadguy.Distance(Core.Me)}");
                    Logger.WriteInfo("\t" + $@"InLineOfSight: {deadguy.InLineOfSight()}" + "\t" +
                                     $@"IsTargetable: {deadguy.IsTargetable}" + "\t" +
                                     $@"IsVisible: {deadguy.IsVisible}");
                }
                Logger.WriteInfo(
                    @"========================================Dead Guy Logger========================================");
            }
            */
            var deadList = Group.DeadAllies.Where(u => !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30 &&
                                                       u.InLineOfSight() &&
                                                       u.IsTargetable &&
                                                       u.IsVisible)
                .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (!deadTarget.IsTargetable)
                return false;

            if (Core.Me.InCombat || Globals.OnPvpMap)
            {
                if (!ActionManager.HasSpell(Spells.Ascend.Id))
                    return false;

                if (!AstrologianSettings.Instance.AscendSwiftcast)
                    return false;

                if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                    return false;

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return false;

                if (await Buff.Swiftcast())
                {
                    while (Core.Me.HasAura(Auras.Swiftcast))
                    {
                        if (await Spells.Ascend.Cast(deadTarget))
                            return true;
                        await Coroutine.Yield();
                    }
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await Spells.Raise.CastAura(deadTarget, Auras.Raise);
        }

        public static async Task<bool> CollectiveUnconscious()
        {
            if (!AstrologianSettings.Instance.CollectiveUnconscious)
                return false;

            if (Group.CastableAlliesWithin10.Count(r => r.Distance() < 6 && r.IsAlive &&
                                                        r.CurrentHealthPercent <=
                                                        AstrologianSettings.Instance.CollectiveUnconsciousHealth) <
                AstrologianSettings.Instance.CollectiveUnconsciousAllies)
                return false;

            return await Spells.CollectiveUnconscious.HealAura(Core.Me, Auras.WheelOfFortune, false);
            //return await Spells.CollectiveUnconscious.CastAura(Core.Me, Auras.WheelOfFortune);
        }

        public static async Task<bool> EarthlyStar()
        {
            if (!ActionManager.HasSpell(Spells.EarthlyStar.Id))
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Combat.CombatTotalTimeLeft < 15)
                return false;

            if (Spells.EarthlyStar.Cooldown != TimeSpan.Zero)
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

            if (Combat.CombatTotalTimeLeft < 40)
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
    }
}
