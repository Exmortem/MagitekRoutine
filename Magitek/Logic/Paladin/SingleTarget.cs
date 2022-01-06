using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;


namespace Magitek.Logic.Paladin
{
    internal static class SingleTarget
    {
        public static async Task<bool> ShieldLob()
        {
            if (PaladinSettings.Instance.ShieldLobToPullExtraEnemies && !BotManager.Current.IsAutonomous)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                {
                    return await Spells.ShieldLob.Cast(pullTarget);
                }
            }

            if (!PaladinSettings.Instance.UseShieldLob)
                return false;

            return await Spells.ShieldLob.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShieldLobLostAggro()
        {
            if (Globals.OnPvpMap)
                return false;

            if (!PaladinSettings.Instance.ShieldLobLostAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var shieldLobTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) > 5 + r.CombatReach && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.Distance(Core.Me) <= 15 + r.CombatReach && r.TargetGameObject != Core.Me);

            if (shieldLobTarget == null)
                return false;

            if (shieldLobTarget.TargetGameObject == null)
                return false;

            if (!await Spells.ShieldLob.Cast(shieldLobTarget))
                return false;

            Logger.Write($@"[Magitek] Shield Lob On {shieldLobTarget.Name} To Pull Aggro");
            return true;
        }

        public static async Task<bool> SpiritsWithin()
        {
            if (!PaladinSettings.Instance.SpiritsWithin)
                return false;

            if (Casting.LastSpell == Spells.FightorFlight)
                return false;

            if (Spells.FightorFlight.Cooldown.Seconds <= 8 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 8000))
            {
                //Right we want to check if we want to hold CoS.
                if (Casting.LastSpell == Spells.FastBlade)
                    return false;

                if (Casting.LastSpell == Spells.RiotBlade)
                    return false;

                if (Casting.LastSpell == Spells.Confiteor)
                    return false;

                return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);
            }

            return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HolySpirit()
        {
            if (!PaladinRoutine.ToggleAndSpellCheck(PaladinSettings.Instance.HolySpirit, Spells.HolySpirit))
                return false;

            if (PaladinRoutine.RequiescatStackCount <= 1)
                return false;

            // Before 6.0 PLD would hold Requiestcat until starting the magic combo,
            // but now it's often put up early during physical combo because it lasts
            // a long time. It's not sufficient to only check if we have Requiestcat stacks
            // now, because we could have them but still be in the middle of our phys combo.
            // We also don't just check if we have FoF because it could be wearing off before
            // we actually finish the physical combo.

            if (ActionManager.LastSpell == Spells.FastBlade
                || ActionManager.LastSpell == Spells.RiotBlade
                || Core.Me.HasAura(Auras.FightOrFight, true, 2000))
            {
                return false;
            }

            // TODO:optimization(wildchill)
            // There was a mana check here, but mana is changed in endwalker for paladin magic combo
            // to have more flexibility. We could check if you have stacks of Requiescat but not enough
            // mana to finish the confiteor combo and early cast confiteor.

            return await Spells.HolySpirit.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Intervene()
        {
            if (Core.Me.ClassLevel < 74)
                return false;

            if (!PaladinSettings.Instance.Intervene)
                return false;

            if (Core.Me.HasAura(Auras.SwordOath) && Core.Me.HasAura(Auras.FightOrFight))
            {
                // We only want to use this during Sword Oath as a OGCD, one we go into this phase and once after using Atonement
                if (Casting.LastSpell == Spells.RoyalAuthority)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);

                if (Casting.LastSpell == Spells.CircleofScorn)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);

                if (Casting.LastSpell == Spells.Atonement)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> Atonement()
        {
            // This logic is to accomodate dropping an unbuffed attonement usage,
            // which puts paladin into a 61 second "standard loop" recommended by
            // the balance (if you use the 3rd unbuffed attonement it's a 64 second
            // loop).
            // TODO:options(wildchill) Maybe add a toggle for 61/64 loop

            // Don't attonement during combo, including when goring blade was last
            // because without goring blade check we'd use a left over attonement
            // that we're trying to drop from the unbuffed segment of the rotation
            if (ActionManager.LastSpell == Spells.FastBlade
                || ActionManager.LastSpell == Spells.RiotBlade
                || ActionManager.LastSpell == Spells.GoringBlade)
            {
                return false;
            }

            // If we have a single charge (HasAuraCharge looks for single stack ONLY)
            // and we're not under FoF then don't use attonement.
            if (!Core.Me.HasAura(Auras.FightOrFight) && Core.Me.HasAuraCharge(Auras.SwordOath))
                return false;

            return await Spells.Atonement.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Confiteor()
        {
            if (!PaladinRoutine.ToggleAndSpellCheck(PaladinSettings.Instance.UseConfiteor, Spells.Confiteor))
                return false;

            if (ActionManager.CanCast(Spells.BladeOfFaith.Id, Core.Me.CurrentTarget))
                return await Spells.BladeOfFaith.Cast(Core.Me.CurrentTarget);

            if (ActionManager.CanCast(Spells.BladeOfTruth.Id, Core.Me.CurrentTarget))
                return await Spells.BladeOfTruth.Cast(Core.Me.CurrentTarget);

            if (ActionManager.CanCast(Spells.BladeOfValor.Id, Core.Me.CurrentTarget))
                return await Spells.BladeOfValor.Cast(Core.Me.CurrentTarget);

            // We want to Confit with our last stack, but if the req buff is
            // about to fall off, and this is our last action, use confit

            var reqAura = Core.Me.GetAuraById(Auras.Requiescat);

            if (reqAura == null)
                return false;

            // In testing with Pohky we saw cases where we properly see Requiescat
            // stack count, but the aura TimeLeft property is a negative value (-30).
            // This works around that, which Pohky regularly could reproduce. Not sure
            // if a ping related problem or something systematic we need to look at
            // in more detail.
            if (PaladinRoutine.RequiescatStackCount > 1
                && (reqAura.TimeLeft > 3.0f || reqAura.TimeLeft < 0.0f))
            {
                return false;
            }

            var ret = await Spells.Confiteor.Cast(Core.Me.CurrentTarget);

            if (ret && PaladinRoutine.RequiescatStackCount > 1)
            {
                Logger.Write($"[PLD DEBUG] Setting: {PaladinSettings.Instance.HolySpirit} Known: {Spells.HolySpirit.IsKnown()} "); ;
                Logger.Write($"[PLD DEBUG] Stacks: {PaladinRoutine.RequiescatStackCount}");
                Logger.Write($"[PLD DEBUG] LastSpell: {ActionManager.LastSpell.Name}");

                foreach (var aura in Core.Me.Auras)
                {
                    Logger.Write($"[PLD DEBUG] Aura: {aura.Name} {aura.TimeLeft}");
                }
            }

            return ret;
        }

        public static async Task<bool> Interrupt()
        {
            List<SpellData> extraStun = new List<SpellData>();

            if (PaladinSettings.Instance.ShieldBash)
            {
                extraStun.Add(Spells.ShieldBash);
            }

            return await Tank.Interrupt(PaladinSettings.Instance, extraStuns: extraStun);
        }
    }
}