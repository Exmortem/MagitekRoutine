﻿using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Sage
    {
        public static bool OnGcd => Spells.Dosis.Cooldown.TotalMilliseconds > 100;

        public static HashSet<string> DontShield = new HashSet<string>();
        public static HashSet<string> DontEukrasianDiagnosis = new HashSet<string>();
        public static HashSet<string> DontEukrasianPrognosis = new HashSet<string>();

        public static List<Character> AllianceDiagnosisOnly = new List<Character>();

        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Sage, Spells.Diagnosis);

        public static bool NeedToInterruptCast()
        {
            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell != Spells.Egeiro && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Resurrection: Unit Died");
                return true;
            }

            if (SageSettings.Instance.InterruptHealing && Casting.DoHealthChecks &&
                Casting.SpellTarget?.CurrentHealthPercent >= SageSettings.Instance.InterruptHealingHealthPercent)
            {
                if (Casting.CastingSpell == Spells.Prognosis && PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                        r.CurrentHealth > 0 && r.Distance(Core.Me) <= Spells.Prognosis.Radius && r.CurrentHealthPercent <=
                        SageSettings.Instance.PrognosisHpPercent) <
                    SageSettings.Instance.PrognosisNeedHealing)
                {
                    Logger.Error($@"Stopped Healing: Party's Health Too High");
                    return true;
                }
                else
                {
                    Logger.Error($@"Stopped Healing: Target's Health Too High");
                    return true;
                }
            }

            if (SageSettings.Instance.InterruptDamageToHeal && !Core.Me.HasAura(1495))
            {
                if (Casting.CastingSpell == Spells.Dosis || Casting.CastingSpell == Spells.DosisII ||
                    Casting.CastingSpell == Spells.DosisIII || Casting.CastingSpell == Spells.Dyskrasia ||
                    Casting.CastingSpell == Spells.DyskrasiaII)
                {
                    if (PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Any(r => r.CurrentHealth > 0 &&
                        r.CurrentHealthPercent <= SageSettings.Instance.InterruptDamageHealthPercent && r.Distance() < 30 && r.InLineOfSight()))
                    {
                        Logger.Error($@"Stopping Cast: Need To Heal Someone In The Party");
                        return true;
                    }
                }
            }

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            return false;
        }
        public static void GroupExtension()
        {
            // Should we be ignoring our alliance?
            if (!SageSettings.Instance.IgnoreAlliance && (Globals.InActiveDuty || WorldManager.InPvP))
            {
                // Create a list of alliance members that we need to check
                if (SageSettings.Instance.HealAllianceDps || SageSettings.Instance.HealAllianceHealers || SageSettings.Instance.HealAllianceTanks)
                {
                    var allianceToHeal =
                        Group.AllianceMembers.Where(a => !a.CanAttack && !a.HasAura(Auras.MountedPvp) && (SageSettings.Instance.HealAllianceDps && a.IsDps() ||
                                                         SageSettings.Instance.HealAllianceTanks && a.IsTank() ||
                                                         SageSettings.Instance.HealAllianceHealers && a.IsHealer()));

                    if (SageSettings.Instance.HealAllianceOnlyDiagnosis)
                    {
                        AllianceDiagnosisOnly = allianceToHeal.ToList();
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
                if (SageSettings.Instance.ResAllianceDps || SageSettings.Instance.ResAllianceHealers ||
                        SageSettings.Instance.ResAllianceTanks)
                {
                    var allianceToRes = Group.AllianceMembers.Where(a => a.CurrentHealth <= 0 &&
                    (SageSettings.Instance.ResAllianceDps &&
                    a.IsDps() ||
                    SageSettings.Instance
                    .ResAllianceTanks && a.IsTank() ||
                    SageSettings.Instance
                    .ResAllianceHealers && a.IsDps()));
                    foreach (var ally in allianceToRes)
                    {
                        Group.DeadAllies.Add(ally);
                    }
                }
            }
        }

        public static readonly uint[] ShieldAuraList = {
            Auras.NocturnalField,
            Auras.Galvanize,
            Auras.EukrasianDiagnosis,
            Auras.EukrasianPrognosis
        };

    }
}
