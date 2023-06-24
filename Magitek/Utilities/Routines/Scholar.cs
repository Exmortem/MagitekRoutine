using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Scholar
    {
        public static List<Character> AlliancePhysickOnly = new List<Character>();

        public static double SeraphTimeRemaining()
        {
            return Math.Max(Spells.SummonSeraph.Cooldown.TotalSeconds - 98, 0);
        }

        public static bool NeedToInterruptCast()
        {
            if (Casting.CastingSpell != Spells.Resurrection && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Cast: Unit Died");
                return true;
            }

            if (Casting.CastingSpell == Spells.Resurrection && (Casting.SpellTarget?.HasAura(Auras.Raise) == true || Casting.SpellTarget?.CurrentHealth > 0))
            {
                Logger.Error($@"Stopped Resurrection: Unit has raise aura");
                return true;
            }

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell == Spells.Succor || Casting.CastingSpell == Spells.Adloquium)
                return false;

            if (ScholarSettings.Instance.InterruptHealing && Casting.DoHealthChecks && Casting.SpellTarget?.CurrentHealthPercent >= ScholarSettings.Instance.InterruptHealingPercent)
            {
                Logger.Error($@"Stopped Healing: Target's Health Too High");
                return true;
            }

            if (ScholarSettings.Instance.StopCastingIfBelowHealthPercent && Globals.InParty)
            {
                if (Casting.CastingSpell == Spells.Broil ||
                    Casting.CastingSpell == Spells.Broil2 ||
                    Casting.CastingSpell == Spells.Broil3 ||
                    Casting.CastingSpell == Spells.Ruin
                    )
                {
                    if (Group.CastableAlliesWithin30.Any(c => c?.CurrentHealthPercent < ScholarSettings.Instance.DamageOnlyIfAboveHealthPercent && c.IsAlive))
                    {
                        Logger.Error($@"Stopped Cast: Ally below {ScholarSettings.Instance.DamageOnlyIfAboveHealthPercent}% Health");
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
            Group.UpdateAlliance(
                ScholarSettings.Instance.IgnoreAlliance,
                ScholarSettings.Instance.HealAllianceDps,
                ScholarSettings.Instance.HealAllianceHealers,
                ScholarSettings.Instance.HealAllianceTanks,
                ScholarSettings.Instance.ResAllianceDps,
                ScholarSettings.Instance.ResAllianceHealers,
                ScholarSettings.Instance.ResAllianceTanks
            );
        }

        public static int EnemiesInCone;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(8);
           
        }
    }
}
