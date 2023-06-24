using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class WhiteMage
    {
        public static bool OnGcd => Spells.Stone.Cooldown.TotalMilliseconds > 100;

        public static HashSet<string> DontCure = new HashSet<string>();
        public static HashSet<string> DontCure2 = new HashSet<string>();
        public static HashSet<string> DontRegen = new HashSet<string>();
        public static HashSet<string> DontTetraGrammaton = new HashSet<string>();
        public static HashSet<string> DontBenediction = new HashSet<string>();
        public static HashSet<string> DontAfflatusSolace = new HashSet<string>();
        public static List<Character> AllianceCureOnly = new List<Character>();

        private static HashSet<uint> DamageSpells = new HashSet<uint>()
        {
            Spells.Stone.Id,
            Spells.Stone2.Id,
            Spells.Stone3.Id,
            Spells.Stone4.Id,
            Spells.Holy.Id,
            Spells.Stone4.Id,
        };

        public static bool NeedToInterruptCast()
        {
            if (Casting.CastingSpell != Spells.Raise && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Cast: Unit Died");
                return true;
            }

            if (Casting.CastingSpell == Spells.Raise && (Casting.SpellTarget?.HasAura(Auras.Raise) == true || Casting.SpellTarget?.CurrentHealth > 0))
            {
                Logger.Error($@"Stopped Resurrection: Unit has raise aura");
                return true;
            }

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (WhiteMageSettings.Instance.InterruptHealing && Casting.DoHealthChecks && Casting.SpellTarget?.CurrentHealthPercent >= WhiteMageSettings.Instance.InterruptHealingHealthPercent)
            {
                Logger.Error($@"Stopped Healing: Target's Health Too High");
                return true;
            }

            if (WhiteMageSettings.Instance.StopDpsIfPartyMemberBelow && DamageSpells.Contains(Core.Me.CastingSpellId))
            {
                if (Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < WhiteMageSettings.Instance.StopDpsIfPartyMemberBelowHealthPercent))
                {
                    Logger.Error($@"Stopped Casting Damage Spell: Ally Below Setting Health");
                    return true;
                }
            }

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            //Logger.Error($@"Stopping Cast: Need To Use A Tank Buster");
            return false;
        }

        public static void GroupExtension()
        {
            Group.UpdateAlliance(
                WhiteMageSettings.Instance.IgnoreAlliance,
                WhiteMageSettings.Instance.HealAllianceDps,
                WhiteMageSettings.Instance.HealAllianceHealers,
                WhiteMageSettings.Instance.HealAllianceTanks,
                WhiteMageSettings.Instance.ResAllianceDps,
                WhiteMageSettings.Instance.ResAllianceHealers,
                WhiteMageSettings.Instance.ResAllianceTanks
            );
        }
    }
}
