using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;

namespace Magitek.Logic.Paladin
{
    internal static class Buff
    {
        public static async Task<bool> Oath()
        {
            if (PaladinSettings.Instance.IronWill && !Core.Me.HasAura(Auras.IronWill))
            {
                return await Spells.IronWill.Cast(Core.Me);
            }


            if (!PaladinSettings.Instance.IronWill && Core.Me.HasAura(Auras.IronWill))
            {
                return await Spells.IronWill.Cast(Core.Me);
            }


            return false;

        }
        public static async Task<bool> Requiescat()
        {
            if (!PaladinRoutine.ToggleAndSpellCheck(PaladinSettings.Instance.Requiescat, Spells.Requiescat))
                return false;

            // In 6.0 PLD casts Req early, while FoF is still up, because the req buff lasts a long time,
            // and early use can be beneficial in some fight lengths.
            // The 6.0 balance standard rotation has it about 3 gcds/7.5~ seconds after FoF usage, but we
            // default to 1 and expose the setting for users to adjust to what works for their situation.
            if (Core.Me.HasAura(Auras.FightOrFight, true,
                PaladinSettings.Instance.RequiescatWithFofSecondsRemaining * 1000))
            {
                return false;
            }

            // If we're in an aoe situation we want to Req even if our
            // target doesn't have goring. This is typically in dungeons.
            var enemyCount = Combat.Enemies.Count(r =>
                r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach);

            if (Spells.FightorFlight.IsKnown()
                && Spells.FightorFlight.Cooldown != TimeSpan.Zero
                && Casting.LastSpell != Spells.FightorFlight
                && enemyCount >= PaladinSettings.Instance.TotalEclipseEnemies)
            {
                return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);
            }

            // Even if we're doing Req first, which is non-standard, we still
            // want to get goring up before starting req combo.
            if (!Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 1900))
                return false;

            if (PaladinSettings.Instance.FoFFirst
                && Spells.FightorFlight.Cooldown.Seconds < 8
                && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 10000))
                return false;


            return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FightOrFlight()
        {
            if (!PaladinSettings.Instance.UseFightOrFlight)
                return false;

            if (!PaladinSettings.Instance.FoFFirst
                && Spells.Requiescat.IsKnownAndReady(10000))
            {
                return false;
            }

            if (Core.Me.HasAura(Auras.Requiescat))
                return false;

            if (ActionManager.CanCast(Spells.BladeOfFaith.Id, Core.Me.CurrentTarget))
                return false;

            if (ActionManager.CanCast(Spells.BladeOfTruth.Id, Core.Me.CurrentTarget))
                return false;

            if (ActionManager.CanCast(Spells.BladeOfValor.Id, Core.Me.CurrentTarget))
                return false;

            // If we're in an aoe situation we want to FoF even if our
            // target doesn't have goring. This is typically in dungeons.
            var enemyCount = Combat.Enemies.Count(r =>
                r.ValidAttackUnit() && r.Distance(Core.Me) <= 5 + r.CombatReach);

            if (enemyCount > PaladinSettings.Instance.TotalEclipseEnemies)
            {
                return await Spells.FightorFlight.Cast(Core.Me);
            }

            if (Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 13000))
                return false;

            var lastSpellToCheck = (PaladinSettings.Instance.FoFFastBlade
                ? Spells.FastBlade
                : Spells.RiotBlade);

            if (ActionManager.LastSpell != lastSpellToCheck)
                return false;

            if (Spells.FastBlade.Cooldown.TotalMilliseconds > (650 + BaseSettings.Instance.UserLatencyOffset))
                return false;

            return await Spells.FightorFlight.Cast(Core.Me);

        }

        public static async Task<bool> DivineVeil()
        {
            if (!PaladinSettings.Instance.DivineVeil)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Group.CastableAlliesWithin15.Any(r => r.CurrentHealthPercent < PaladinSettings.Instance.DivineVeilHp))
                return false;

            if (!Group.CastableAlliesWithin30.Any(r => r.IsHealer()))
                return false;

            return await Spells.DivineVeil.Cast(Core.Me);
        }

        public static async Task<bool> Sheltron()
        {
            if (!PaladinSettings.Instance.Sheltron)
                return false;

            if (ActionResourceManager.Paladin.Oath < 50)
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.SheltronHp)
                return false;

            if (!ActionManager.HasSpell(Spells.Sheltron.Id))
                return false;

            if (!ActionManager.CanCast(Spells.Sheltron.Id, Core.Me))
                return false;

            if (!Core.Me.HasTarget)
                return false;

            var targetAsCharacter = Core.Me.CurrentTarget as Character;

            if (targetAsCharacter == null)
                return false;

            if (!targetAsCharacter.HasTarget)
                return false;

            if (targetAsCharacter.TargetGameObject != Core.Me)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5 + Core.Me.CurrentTarget.CombatReach)
                return false;

            return await Spells.Sheltron.Cast(Core.Me);
        }

        public static async Task<bool> Cover()
        {
            if (!PaladinSettings.Instance.UseCover)
                return false;

            if (!Globals.InParty)
                return false;

            if (Spells.Cover.Cooldown > TimeSpan.Zero)
                return false;

            if (PaladinSettings.Instance.UseCoverHealer)
            {
                GameObject coverTarget = Group.CastableAlliesWithin10.FirstOrDefault(r => r.IsHealer() && r.CurrentHealthPercent < PaladinSettings.Instance.UseCoverHealerHp);

                if (coverTarget != null)
                {
                    return await Spells.Cover.Cast(coverTarget);
                }
            }

            if (!PaladinSettings.Instance.UseCoverDps)
                return false;

            var coverDpsTarget = Group.CastableAlliesWithin10.FirstOrDefault(r => r.IsDps() && r.CurrentHealthPercent < PaladinSettings.Instance.UseCoverDpsHp);

            if (coverDpsTarget == null)
                return false;

            return await Spells.Cover.Cast(coverDpsTarget);
        }

        public static async Task<bool> Intervention()
        {
            if (!PaladinSettings.Instance.InterventionOnNearbyPartyMember)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Spells.Intervention.Cooldown > TimeSpan.Zero)
                return false;

            if (PaladinSettings.Instance.InterventionPartyAlwaysWithCD)
            {
                if (Core.Me.HasAura(Auras.Sentinel) || Core.Me.HasAura(Auras.Rampart))
                {
                    var interventionTarget = Group.CastableAlliesWithin30.OrderBy(r => r.CurrentHealthPercent).FirstOrDefault();

                    if (interventionTarget != null)
                    {
                        return await Spells.Intervention.Cast(interventionTarget);
                    }
                }

            }

            if (PaladinSettings.Instance.InterventionPartyAlwaysWOCD)
            {
                if (!Core.Me.HasAura(Auras.Sentinel) && !Core.Me.HasAura(Auras.Rampart))
                    return false;
            }

            var interventionTarget2 = Group.CastableAlliesWithin30.OrderBy(r => r.CurrentHealthPercent).FirstOrDefault(r => r.CurrentHealthPercent <= PaladinSettings.Instance.InterventionOnNearbyPartyMemberHealth);

            if (interventionTarget2 == null)
                return false;

            return await Spells.Intervention.Cast(interventionTarget2);
        }
    }
}