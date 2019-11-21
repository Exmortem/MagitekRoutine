using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class Scholar
    {
        public static List<Character> AlliancePhysickOnly = new List<Character>();

        private static bool NeedAdloquiumTankBuster
        {
            get
            {
                var castingTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting);

                if (castingTarget == null)
                    return false;

                var castingInfo = castingTarget.SpellCastInfo;

                if (!TankBusterManager.AdloquiumList.ContainsKey(castingInfo.ActionId))
                    return false;

                var adloquiumTankBuster = TankBusterManager.AdloquiumList.FirstOrDefault(r => r.Key == castingInfo.ActionId);

                if (castingInfo.CurrentCastTime.TotalMilliseconds < adloquiumTankBuster.Value)
                    return false;

                return !castingTarget.HasAura(Auras.Galvanize);
            }   
        }

        private static bool NeedExcogitationTankBuster
        {
            get
            {
                var excogitationTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.ExcogitationList.ContainsKey(r.CastingSpellId))?.TargetCharacter;

                if (excogitationTarget == null)
                    return false;

                return !excogitationTarget.HasAura(Auras.Exogitation);
            }
        }

        private static bool NeedSuccorTankBuster
        {
            get
            {
                var castingTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting);

                if (castingTarget == null)
                    return false;

                var castingInfo = castingTarget.SpellCastInfo;

                if (!TankBusterManager.SuccorList.ContainsKey(castingInfo.ActionId))
                    return false;

                var succorTankBuster = TankBusterManager.SuccorList.FirstOrDefault(r => r.Key == castingInfo.ActionId);

                if (castingInfo.CurrentCastTime.TotalMilliseconds < succorTankBuster.Value)
                    return false;

                return !Group.CastableAlliesWithin15.All(r => r.HasAura(Auras.Galvanize));
            }
        }

        public static bool NeedToInterruptCast()
        {
            if (Casting.CastingTankBuster)
                return false;

            if (Casting.CastingSpell != Spells.Resurrection && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Cast: Unit Died");
                return true;
            }

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell == Spells.Succor)
                return false;

            if (Core.Me.HasAura(Auras.EmergencyTactics))
                return false;

            if (ScholarSettings.Instance.InterruptHealing && Casting.DoHealthChecks && Casting.SpellTarget?.CurrentHealthPercent >= ScholarSettings.Instance.InterruptHealingPercent)
            {
                Logger.Error($@"Stopped Healing: Target's Health Too High");
                return true;
            }

            if (!ScholarSettings.Instance.UseTankBusters || !ScholarSettings.Instance.PrioritizeTankBusters)
                return false;

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            if (Core.Me.CurrentManaPercent < ScholarSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (!NeedAdloquiumTankBuster && !NeedSuccorTankBuster && !NeedExcogitationTankBuster)
                return false;

            Logger.Error($@"Stopping Cast: Need To Use A Tank Buster");
            return true;
        }

        public static void GroupExtension()
        {
            // Should we be ignoring our alliance?
            if (!ScholarSettings.Instance.IgnoreAlliance && (Globals.InActiveDuty || WorldManager.InPvP))
            {
                // Create a list of alliance members that we need to check
                if (ScholarSettings.Instance.HealAllianceDps || ScholarSettings.Instance.HealAllianceHealers || ScholarSettings.Instance.HealAllianceTanks)
                {
                    var allianceToHeal =  Group.AllianceMembers.Where(a => !a.CanAttack && !a.HasAura(Auras.MountedPvp) && (ScholarSettings.Instance.HealAllianceDps && a.IsDps() ||
                                                                           ScholarSettings.Instance.HealAllianceTanks && a.IsTank() ||
                                                                           ScholarSettings.Instance.HealAllianceHealers && a.IsDps()));

                    // If all we're going to do with the alliance is Physick them, then simply use this list
                    if (ScholarSettings.Instance.HealAllianceOnlyPhysick)
                    {
                        AlliancePhysickOnly = allianceToHeal.ToList();
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
                    }
                }

                if (ScholarSettings.Instance.ResAllianceDps || ScholarSettings.Instance.ResAllianceHealers || ScholarSettings.Instance.ResAllianceTanks)
                {
                    var allianceToRes = Group.AllianceMembers.Where(a => a.CurrentHealth <= 0 &&
                                                                   (ScholarSettings.Instance.ResAllianceDps && a.IsDps() ||
                                                                    ScholarSettings.Instance.ResAllianceTanks && a.IsTank() ||
                                                                    ScholarSettings.Instance.ResAllianceHealers && a.IsDps()));

                    foreach (var ally in allianceToRes)
                    {
                        Group.DeadAllies.Add(ally);
                    }
                }
            }
        }
    }
}
