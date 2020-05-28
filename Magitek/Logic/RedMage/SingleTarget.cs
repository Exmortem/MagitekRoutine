using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using RedMageRoutines = Magitek.Utilities.Routines.RedMage;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class SingleTarget
    {
        private static bool InMeleeRange => Core.Me.CurrentTarget.Distance(Core.Me) <= (4 + Core.Me.CurrentTarget.CombatReach);

        //If we're further out than this, we don't want to zoom in, because we might run into some AoE
        private static bool InSafeCorpsACorpsRange => Core.Me.CurrentTarget.Distance(Core.Me) <= (2 + Core.Me.CurrentTarget.CombatReach);

        public static async Task<bool> Jolt()
        {
            if (Core.Me.ClassLevel < 4)
                return await Spells.Jolt.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;
            
            return await Spells.Jolt.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Scorch()
        {
            if (Core.Me.ClassLevel < Spells.Scorch.LevelAcquired)
                return false;

            if (ActionManager.LastSpell != Spells.Verholy && ActionManager.LastSpell != Spells.Verflare)
                return false;

            else
                return await Spells.Scorch.Cast(Core.Me.CurrentTarget);
        }

        private static List<SpellData> ComboInProgressSpells = new List<SpellData>() { Spells.Riposte, Spells.Zwerchhau, Spells.EnchantedRedoublement, Spells.Verflare, Spells.Verholy };
        //Sometimes, after casting Riposte, the ActionManager still reports the spell *before* Riposte as the LastSpell, so we need to check the Casting class as well, just to be sure
        public static bool ComboInProgress => ComboInProgressSpells.Any(spell =>    spell.Id == ActionManager.LastSpellId
                                                                                 || spell.Id == Casting.LastSpell?.Id);

        //We should cast Veraero if we're holding for Veraero, OR if we have less white mana and we're not holding for Verthunder
        private static bool ShouldCastVeraero =>
               HoldForVeraero
            || (   WhiteMana <= BlackMana
                && !HoldForVerthunder);

        //We should cast Verthunder if we're holding for Verthunder, OR if we have less black mana and we're not holding for Veraero
        private static bool ShouldCastVerthunder =>
               HoldForVerthunder
            || (   BlackMana <= WhiteMana
                && !HoldForVeraero);

        //We want to cast Veraero even if there's more white mana if:
        //  1. We've already procced Verfire
        //  2. We haven't procced Verstone
        //  3. This won't put the mana difference over 30
        private static bool HoldForVeraero =>
               Core.Me.HasAura(Auras.VerfireReady)
            && !Core.Me.HasAura(Auras.VerstoneReady)
            && WhiteMana + 11 <= BlackMana + 30
            && Core.Me.ClassLevel >= Spells.Veraero.LevelAcquired;

        //We want to cast Verthunder even if there's more black mana if:
        //  1. We've already procced Verstone
        //  2. We haven't procced Verfire
        //  3. This won't put the mana difference over 30
        private static bool HoldForVerthunder =>
               Core.Me.HasAura(Auras.VerstoneReady)
            && !Core.Me.HasAura(Auras.VerfireReady)
            && BlackMana + 11 <= WhiteMana + 30
            && Core.Me.ClassLevel >= Spells.Verthunder.LevelAcquired;

        public static async Task<bool> Veraero()
        {
            if (!Core.Me.HasAura(Auras.Dualcast))
            {
                //TODO: The Balance says we should hold this for when we're moving around
                if (RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                {
                    if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                        return false;

                    if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                        return false;

                    if (!ShouldCastVeraero)
                        return false;

                    if (Core.Me.HasAura(Auras.VerstoneReady))
                        return false;

                    //Don't swiftcast right before a combo, it'll mess up our procs, or in a combo, it'll mess up the combo
                    if (ReadyForCombo() || ComboInProgress)
                        return false;

                    if (!RedMageRoutines.CanWeave)
                        return false;

                    if (await Spells.Swiftcast.Cast(Core.Me))
                    {
                        await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                        await Coroutine.Wait(2000, () => ActionManager.CanCast(Spells.Veraero, Core.Me.CurrentTarget));
                        return await Spells.Veraero.Cast(Core.Me.CurrentTarget);
                    }
                }
                else
                    return false;
            }

            //Check if prepping would make us ready for the combo
            if (ReadyForCombo(BlackMana, WhiteMana + 11))
            {
                if (PrepForComboWithVeraero())
                    return await Spells.Veraero.Cast(Core.Me.CurrentTarget);
            }

            //Check if we need to clean up our procs before the combo
            //TODO: We also need to prep if we're going to manify right after this
            if (ReadyForCombo())
            {
                if (PrepForComboWithVeraero())
                    return await Spells.Veraero.Cast(Core.Me.CurrentTarget);
                else
                    return false;
            }

            if (!ShouldCastVeraero)
                return false;

            else
                return await Spells.Veraero.Cast(Core.Me.CurrentTarget);
        }


        //Check if we need to cast Verthunder to get our procs set up properly to begin a melee combo
        private static bool PrepForComboWithVerthunder()
        {
            //If we're ready to combo and both procs are up, we're stuck, so cast Verthunder if there isn't more BlackMana.
            //One of the procs is getting overwritten regardless, so we may as well try to keep the mana close.
            if (Core.Me.HasAura(Auras.VerfireReady) && Core.Me.HasAura(Auras.VerstoneReady) && BlackMana <= WhiteMana)
                return true;
            //If we're ready to combo and neither proc is up, we should cast Verthunder if there's less BlackMana and the
            //cast would make it pass WhiteMana, or if Veraero wouldn't be enough to let WhiteMana catch up. Either way,
            //Verholy will proc Verstone.
            if (!Core.Me.HasAura(Auras.VerfireReady) && !Core.Me.HasAura(Auras.VerstoneReady) &&
                ((BlackMana < WhiteMana && BlackMana + 11 > WhiteMana) || (WhiteMana + 11 <= BlackMana)))
                return true;
            //If Verfire's proc is up and Verstone's isn't, and casting Verthunder would let BlackMana pass WhiteMana, cast
            //Verthunder so that Verholy will proc Verstone
            if (Core.Me.HasAura(Auras.VerfireReady) && !Core.Me.HasAura(Auras.VerstoneReady) && BlackMana + 11 > WhiteMana)
                return true;
            //If Verfire's proc is not up and Verstone's is, but casting Veraero won't be enough for WhiteMana to pass BlackMana, cast
            //Verthunder. Verholy will overwrite Verstone regardless, so we may as well try for a Verthunder proc now
            if (!Core.Me.HasAura(Auras.VerfireReady) && Core.Me.HasAura(Auras.VerstoneReady) && WhiteMana + 11 <= BlackMana)
                return true;

            return false;
        }

        private static bool PrepForComboWithVeraero()
        {
            //If Verthunder's not the right spell to prep with, Veraero's the only other choice!
            return !PrepForComboWithVerthunder();
        }

        public static async Task<bool> Verthunder()
        {
            if (!Core.Me.HasAura(Auras.Dualcast))
            {
                //TODO: The Balance says we should hold this for when we're moving around
                if (RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                {
                    if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                        return false;

                    if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                        return false;

                    if (!ShouldCastVerthunder)
                        return false;

                    if (Core.Me.HasAura(Auras.VerfireReady))
                        return false;

                    //Don't swiftcast right before a combo, it'll mess up our procs, or in a combo, it'll mess up the combo
                    if (ReadyForCombo() || ComboInProgress)
                        return false;

                    if (!RedMageRoutines.CanWeave)
                        return false;

                    if (await Spells.Swiftcast.Cast(Core.Me))
                    {
                        await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                        await Coroutine.Wait(2000, () => ActionManager.CanCast(Spells.Verthunder, Core.Me.CurrentTarget));
                        return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
                    }
                }
                else
                    return false;
            }

            if (Core.Me.ClassLevel < 10)
                return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);

            //Check if prepping would make us ready for the combo
            if (ReadyForCombo(BlackMana + 11, WhiteMana))
            {
                if (PrepForComboWithVerthunder())
                    return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
            }

            //Check if we need to clean up our procs before the combo
            //TODO: We also need to prep if we're going to manify right after this
            if (ReadyForCombo())
            {
                if (PrepForComboWithVerthunder())
                    return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
                else
                    return false;
            }

            if (!ShouldCastVerthunder)
                return false;

            else
                return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verfire()
        {
            if (!Core.Me.HasAura(Auras.VerfireReady))
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (BlackMana > WhiteMana)
                return false;
            else
                return await Spells.Verfire.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verstone()
        {
            if (!Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (BlackMana < WhiteMana)
                return false;
            else
                return await Spells.Verstone.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verflare()
        {
            if (Core.Me.ClassLevel < Spells.Verflare.LevelAcquired)
                return false;

            //If we don't have Verholy yet, cast Verflare even if there's more black mana
            //If mana's equal, cast Verflare unless it has its proc already and Verholy doesn't
            if (   (   BlackMana > WhiteMana
                    || (   BlackMana == WhiteMana
                        && !Core.Me.HasAura(Auras.VerstoneReady)
                        && Core.Me.HasAura(Auras.VerfireReady)))
                && Core.Me.ClassLevel >= Spells.Verholy.LevelAcquired)
                return false;

            if (ActionManager.LastSpell != Spells.EnchantedRedoublement)
                return false;

            else
                return await Spells.Verflare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verholy()
        {
            if (Core.Me.ClassLevel < Spells.Verholy.LevelAcquired)
                return false;

            //If mana's equal, cast Verholy unless it has its proc already and Verflare doesn't
            if (   WhiteMana > BlackMana
                || (   WhiteMana == BlackMana
                    && !Core.Me.HasAura(Auras.VerfireReady)
                    && Core.Me.HasAura(Auras.VerstoneReady)))
                return false;

            if (ActionManager.LastSpell != Spells.EnchantedRedoublement)
                return false;

            else
                return await Spells.Verholy.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fleche()
        {
            if (!RedMageSettings.Instance.Fleche)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            else
                return await Spells.Fleche.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Displacement()
        {
            if (!RedMageSettings.Instance.Displacement)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (BlackMana > 24 && WhiteMana > 24)
                return false;

            if (!ComboInProgress)
                return false;

            else
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Engagement()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (!RedMageSettings.Instance.Engagement)
                return false;

            if (BlackMana > 24 && WhiteMana > 24)
                return false;

            else
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);
        }

        public static bool ReadyForCombo()
        {
            return ReadyForCombo(BlackMana, WhiteMana);
        }

        public static bool ReadyForCombo(int bm, int wm)
        {
            return (Core.Me.ClassLevel < 35 && (bm >= 30 && wm >= 30))
                   || (Core.Me.ClassLevel < 50 && (bm >= 55 && wm >= 55))
                   || (bm >= 80 && wm >= 80);
        }

        public static bool ReadyForManaficationComboSoon =>
               Core.Me.ClassLevel >= Spells.Manafication.LevelAcquired
            && WhiteMana + 9 >= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana
            && BlackMana + 9 >= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana;

        public static async Task<bool> CorpsACorps()
        {
            if (!RedMageSettings.Instance.CorpsACorps)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (!RedMageRoutines.CanWeave)
                return false;

            //We want to skip Corps-a-corps if:
            //  1. The user has selected to only use it in melee range, and they're out of melee range.
            //         When in melee-range-only mode, it will be used whenever possible, rather than
            //         to open a combo. This can be useful in a boss fight when you don't want
            //         unexpected movement but still want the DPS when you're close enough. The user
            //         will need to approach the boss manually to get off a combo
            // 2. The user has selected to use it outside of melee range, and the combo isn't up yet.
            //         When in Corps-a-corps-anywhere mode, it will be used only to open a combo. Great
            //         for getting into melee range quickly to get off a combo, but is dangerous for a
            //         lot of fights
            if (   (RedMageSettings.Instance.CorpsACorpsInMeleeRangeOnly && !InSafeCorpsACorpsRange)
                || (!RedMageSettings.Instance.CorpsACorpsInMeleeRangeOnly && !ReadyForCombo()))
                return false;
            else
                return await Spells.CorpsACorps.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Zwerchhau()
        {
            if (ActionManager.LastSpell != Spells.Riposte)
                return false;

            if (!InMeleeRange)
                return false;

            if (BlackMana < 25 || WhiteMana < 25)
                return false;

            else
                return await Spells.Zwerchhau.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Redoublement()
        {
            if (ActionManager.LastSpell != Spells.Zwerchhau)
                return false;

            if (!InMeleeRange)
                return false;

            if (BlackMana < 25 || WhiteMana < 25)
                return false;

            else
                return await Spells.Redoublement.Cast(Core.Me.CurrentTarget);
        }

        //TODO: Handle case where you have verflare but not yet verholy
        private static bool DumpVerfireBeforeCombo =>
            BlackMana < WhiteMana && Core.Me.HasAura(Auras.VerfireReady) && (Math.Max(BlackMana + 9, 100) - 100) + (Math.Max(WhiteMana + 11, 100) - 100) <= 8;

        private static bool DumpVerstoneBeforeCombo =>
            WhiteMana < BlackMana && Core.Me.HasAura(Auras.VerstoneReady) && (Math.Max(WhiteMana + 9, 100) - 100) + (Math.Max(BlackMana + 11, 100) - 100) <= 8;

        private static bool FixEqualManaBeforeCombo =>
               WhiteMana == BlackMana
            && (   (!Core.Me.HasAura(Auras.VerstoneReady) && !Core.Me.HasAura(Auras.VerfireReady) && WhiteMana <= 94)
                || (!Core.Me.HasAura(Auras.VerstoneReady) && Core.Me.HasAura(Auras.VerfireReady) && WhiteMana <= 88)
                || (Core.Me.HasAura(Auras.VerstoneReady) && !Core.Me.HasAura(Auras.VerfireReady) && WhiteMana <= 88));

        public static async Task<bool> Riposte()
        {
            if (Core.Me.ClassLevel < 2)
            {
                return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
            }

            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (!ReadyForCombo())
                return false;

            if (RedMageSettings.Instance.MeleeComboBossesOnly && !Core.Me.CurrentTarget.IsBoss())
                return false;

            if (DumpVerfireBeforeCombo)
                return false;

            if (DumpVerstoneBeforeCombo)
                return false;

            if (FixEqualManaBeforeCombo)
                return false;

            return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
        }

        private const double RepriseRange = 25.0;

        public static async Task<bool> Reprise()
        {
            if (!RedMageSettings.Instance.UseReprise)
                return false;

            if (BlackMana < 5 || WhiteMana < 5)
                return false;

            if (!MovementManager.IsMoving)
                return false;

            if (ComboInProgress)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > RepriseRange + Core.Me.CurrentTarget.CombatReach + Core.Me.CombatReach)
                return false;

            if (!Combat.Enemies.Any(e => e.IsBoss()))
                return false;

            return await Spells.Reprise.Cast(Core.Me.CurrentTarget);
        }
    }
}