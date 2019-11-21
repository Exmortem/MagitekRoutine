using System;
using System.Collections.Generic;
using System.Linq;
using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class Astrologian
    {
        public static bool OnGcd => Spells.Malefic.Cooldown.TotalMilliseconds > 100;

        public static HashSet<string> DontBenefic = new HashSet<string>();
        public static HashSet<string> DontBenefic2 = new HashSet<string>();
        public static HashSet<string> DontDiurnalBenefic = new HashSet<string>();
        public static HashSet<string> DontNocturnalBenefic = new HashSet<string>();
        public static HashSet<string> DontEssentialDignity = new HashSet<string>();
        public static HashSet<string> DontCelestialIntersection = new HashSet<string>();

        public static List<Character> AllianceBeneficOnly = new List<Character>();

        private static bool NeedBenefic2TankBuster
        {
            get
            {
                var a = Combat.Enemies
                    .FirstOrDefault(r => r.IsCasting && TankBusterManager.Benefic2List.Contains(r.CastingSpellId))
                    ?.TargetCharacter;

                if (a == null)
                    return false;

                return Casting.LastSpell != Spells.Benefic2 || Casting.LastSpellTarget != a ||
                       DateTime.Now >= Casting.LastTankBusterTime.AddSeconds(5);
            }
        }

        private static bool NeedHeliosTankBuster
        {
            get
            {
                if (Casting.LastSpell == Spells.Helios && Casting.LastSpellTarget == Core.Me &&
                    DateTime.Now < Casting.LastTankBusterTime.AddSeconds(5))
                    return false;

                return Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.HeliosList.Contains(r.CastingSpellId));
            }
        }

        private static bool NeedAspectedHeliosTankBuster
        {
            get
            {
                var enemyCasting = Combat.Enemies.Any(r => r.IsCasting &&
                                                           TankBusterManager.AspectedHeliosList.Contains(r
                                                               .CastingSpellId));

                switch (Core.Me.Sect())
                {
                    case AstrologianSect.Diurnal:
                        return enemyCasting &&
                               Group.CastableAlliesWithin15.Count(r => !r.HasAura(Auras.AspectedHelios)) <
                               AstrologianSettings.Instance.DiurnalHeliosAllies;
                    case AstrologianSect.Nocturnal:
                        return enemyCasting &&
                               Group.CastableAlliesWithin15.Count(r => !r.HasAura(Auras.NocturnalField)) <
                               AstrologianSettings.Instance.NocturnalHeliosAllies;
                    case AstrologianSect.None:
                        return false;
                    default:
                        return false;
                }
            }
        }

        private static bool NeedAspectedBeneficTankBuster
        {
            get
            {
                var a = Combat.Enemies
                    .FirstOrDefault(
                        r => r.IsCasting && TankBusterManager.AspectedBeneficList.Contains(r.CastingSpellId))
                    ?.TargetCharacter;

                if (a == null)
                    return false;

                return Casting.LastSpell != Spells.AspectedBenefic || Casting.LastSpellTarget != a ||
                       DateTime.Now >= Casting.LastTankBusterTime.AddSeconds(5);
            }
        }

        public static bool NeedToInterruptCast()
        {
            if (Casting.CastingTankBuster)
                return false;

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell != Spells.Ascend && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Resurrection: Unit Died");
                return true;
            }

            if (AstrologianSettings.Instance.InterruptHealing && Casting.DoHealthChecks &&
                Casting.SpellTarget?.CurrentHealthPercent >= AstrologianSettings.Instance.InterruptHealingHealthPercent)
            {
                if (Casting.CastingSpell == Spells.Helios && PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                        r.CurrentHealth > 0 && r.Distance(Core.Me) <= Spells.Helios.Radius && r.CurrentHealthPercent <=
                        AstrologianSettings.Instance.HeliosHealthPercent) <
                    AstrologianSettings.Instance.HeliosAllies)
                {
                    Logger.Error($@"Stopped Healing: Party's Health Too High");
                    return true;
                }
                if (Casting.CastingSpell == Spells.AspectedHelios)
                {
                    if (Core.Me.Sect() == AstrologianSect.Nocturnal && PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                            r.CurrentHealth > 0 &&
                            r.Distance(Core.Me) <= Spells.AspectedHelios.Radius &&
                            r.CurrentHealthPercent <=
                            AstrologianSettings.Instance.NocturnalHeliosHealthPercent &&
                            !r.HasAura(Auras.NocturnalField, true)) <
                        AstrologianSettings.Instance.NocturnalHeliosAllies)
                    {
                        Logger.Error($@"Stopped Healing: Party's Health Too High");
                        return true;
                    }
                    if (Core.Me.Sect() == AstrologianSect.Diurnal && PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                            r.CurrentHealth > 0 &&
                            r.Distance(Core.Me) <= Spells.AspectedHelios.Radius &&
                            r.CurrentHealthPercent <=
                            AstrologianSettings.Instance.DiurnalHeliosHealthPercent &&
                            !r.HasAura(Auras.AspectedHelios, true)) < AstrologianSettings.Instance.DiurnalHeliosAllies)
                    {
                        Logger.Error($@"Stopped Healing: Party's Health Too High");
                        return true;
                    }
                }
                else
                {
                    Logger.Error($@"Stopped Healing: Target's Health Too High");
                    return true;
                }
            }

            if (!AstrologianSettings.Instance.UseTankBusters || !AstrologianSettings.Instance.PrioritizeTankBusters)
                return false;
    
            if (AstrologianSettings.Instance.InterruptDamageToHeal && !Core.Me.HasAura(1495))
            {
                if (Casting.CastingSpell == Spells.Malefic || Casting.CastingSpell == Spells.Malefic2 ||
                    Casting.CastingSpell == Spells.Malefic3 || Casting.CastingSpell == Spells.PvpMalefic3 ||
                    Casting.CastingSpell == Spells.Gravity)
                {

                    var lowestHealthToInterruptList = new[]
                    {
                        AstrologianSettings.Instance.Benefic ? AstrologianSettings.Instance.BeneficHealthPercent -10 : 0,
                        AstrologianSettings.Instance.Benefic2 ? AstrologianSettings.Instance.Benefic2HealthPercent -10 : 0
                    };

                    var lowestHealthToInterrupt = lowestHealthToInterruptList.Max();
                    
                    if (PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Any(r => r.CurrentHealth > 0 &&
                        r.CurrentHealthPercent <= lowestHealthToInterrupt && r.Distance() < 30 && r.InLineOfSight()))
                    {
                        Logger.Error($@"Stopping Cast: Need To Heal Someone In The Party");
                        return true;
                    }
                }
            }

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            if (Core.Me.CurrentManaPercent < AstrologianSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (!NeedBenefic2TankBuster && !NeedHeliosTankBuster && !NeedAspectedHeliosTankBuster &&
                !NeedAspectedBeneficTankBuster)
                return false;

            Logger.Error($@"Stopping Cast: Need To Use A Tank Buster");
            return true;
        }

        public static void GroupExtension()
        {
            // Should we be ignoring our alliance?
            if (!AstrologianSettings.Instance.IgnoreAlliance && (Globals.InActiveDuty || WorldManager.InPvP))
            {
                // Create a list of alliance members that we need to check
                if (AstrologianSettings.Instance.HealAllianceDps || AstrologianSettings.Instance.HealAllianceHealers || AstrologianSettings.Instance.HealAllianceTanks)
                {
                    var allianceToHeal =
                        Group.AllianceMembers.Where(a => !a.CanAttack && !a.HasAura(Auras.MountedPvp) && (AstrologianSettings.Instance.HealAllianceDps && a.IsDps() ||
                                                         AstrologianSettings.Instance.HealAllianceTanks && a.IsTank() ||
                                                         AstrologianSettings.Instance.HealAllianceHealers && a.IsHealer()));

                    //Logging.Write("Found {0} Dps, {1} Tanks, {2} Healers", allianceToHeal.Count(a => a.IsDps()),allianceToHeal.Count(a => a.IsTank()), allianceToHeal.Count(a => a.IsHealer()));
                    // If all we're going to do with the alliance is Physick them, then simply use this list
                    if (AstrologianSettings.Instance.HealAllianceOnlyBenefic)
                    {
                        AllianceBeneficOnly = allianceToHeal.ToList();
                    }
                    else
                    {
                        // If not, then sort the alliance members into the appropriate lists
                        foreach (var ally in allianceToHeal)
                        {
                            var distance = ally.Distance(Core.Me);

                            if (distance <= 30)
                            {
                                Group.CastableAlliesWithin30.Add(ally);
                            }

                            if (distance <= 15)
                            {
                                Group.CastableAlliesWithin15.Add(ally);
                            }

                            if (distance <= 10)
                            {
                                Group.CastableAlliesWithin10.Add(ally);
                            }
                        }
                        //Logging.Write("CastableAlliesWithin30 is now {0}",Group.CastableAlliesWithin30.Count);
                    }
                }

                if (AstrologianSettings.Instance.ResAllianceDps || AstrologianSettings.Instance.ResAllianceHealers ||
                    AstrologianSettings.Instance.ResAllianceTanks)
                {
                    var allianceToRes = Group.AllianceMembers.Where(a => a.CurrentHealth <= 0 &&
                                                                         (AstrologianSettings.Instance.ResAllianceDps &&
                                                                          a.IsDps() ||
                                                                          AstrologianSettings.Instance
                                                                              .ResAllianceTanks && a.IsTank() ||
                                                                          AstrologianSettings.Instance
                                                                              .ResAllianceHealers && a.IsDps()));

                    foreach (var ally in allianceToRes)
                    {
                        Group.DeadAllies.Add(ally);
                    }
                }
            }

            // Heal Pets
            if (!AstrologianSettings.Instance.HealPartyMembersPets)
                return;

            var pets = AstrologianSettings.Instance.HealPartyMembersPetsTitanOnly
                ? Group.Pets.Where(r => r.EnglishName.Contains("Titan")).ToArray()
                : Group.Pets.ToArray();
            Group.CastableAlliesWithin30.AddRange(pets);
        }

        public static readonly uint[] ShieldAuraList = {
            Auras.NocturnalField,
            Auras.Galvanize
        };
        
        public static Vector3 EarthlyStarLocation { get; set; }

    }
}
