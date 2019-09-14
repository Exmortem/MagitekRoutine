using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class SingleTarget
    {
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
              
            return await Spells.Scorch.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Veraero()
        {
            if (!Core.Me.HasAura(Auras.Dualcast))
            {
                if (RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                {
                    if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.CorpsACorps || s.Spell == Spells.Riposte))
                        return false;

                    if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                        return false;

                    if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                        return false;

                    if (WhiteMana > BlackMana)
                        return false;

                    if (await Spells.Swiftcast.Cast(Core.Me))
                    {
                        await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                        await Coroutine.Wait(2000,
                            () => ActionManager.CanCast(Spells.Veraero, Core.Me.CurrentTarget));
                        return await Spells.Veraero.Cast(Core.Me.CurrentTarget);

                    }
                }
                else
                    return false;
            }

            if (WhiteMana > BlackMana)
                return false;

            return await Spells.Veraero.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verthunder()
        {
            if (!Core.Me.HasAura(Auras.Dualcast))
            {
                if (RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                {
                    if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.CorpsACorps || s.Spell == Spells.Riposte))
                        return false;
                    
                    if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                        return false;

                    if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                        return false;

                    if (BlackMana > WhiteMana)
                        return false;

                    if (await Spells.Swiftcast.Cast(Core.Me))
                    {
                        await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                        await Coroutine.Wait(2000,
                            () => ActionManager.CanCast(Spells.Verthunder, Core.Me.CurrentTarget));
                        return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
                        
                    }
                }
                else
                    return false;
            }

            if (Core.Me.ClassLevel < 10)
                return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);

            if (BlackMana > WhiteMana)
                return false;

            return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verfire()
        {
            return await Spells.Verfire.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verstone()
        {
            return await Spells.Verstone.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verflare()
        {
            if (BlackMana <= WhiteMana)
                return await Spells.Verflare.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Verholy()
        {
            if (WhiteMana > BlackMana)
                return false;

            return await Spells.Verholy.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fleche()
        {
            if (!RedMageSettings.Instance.Fleche)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

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

            var inMeleeCombo = Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Riposte 
                                                                 || s.Spell == Spells.Zwerchhau
                                                                 || s.Spell == Spells.Redoublement);
            if (!inMeleeCombo)
                return false;

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

            return await Spells.Engagement.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CorpsACorps()
        {
            if (!RedMageSettings.Instance.CorpsACorps)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (BlackMana < 90 || WhiteMana < 90)
                return false;

            return await Spells.CorpsACorps.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Zwerchhau()
        {
            if (ActionManager.LastSpell != Spells.Riposte)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 4 + Core.Me.CurrentTarget.CombatReach)
                return false;

            if (BlackMana < 25 || WhiteMana < 25)
                return false;

            return await Spells.Zwerchhau.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Redoublement()
        {
            if (ActionManager.LastSpell != Spells.Zwerchhau)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 4 + Core.Me.CurrentTarget.CombatReach)
                return false;

            if (BlackMana < 25 || WhiteMana < 25)
                return false;

            return await Spells.Redoublement.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Riposte()
        {
            if (Core.Me.CurrentTarget.Distance(Core.Me) > 4 + Core.Me.CurrentTarget.CombatReach)
                return false;

            if (Core.Me.ClassLevel < 2)
            {
                return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
            }

            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (Core.Me.ClassLevel < 35)
            {
                if (BlackMana < 55 || WhiteMana < 55)
                    return false;
            }

            if (Core.Me.ClassLevel > 49)
            {
                if (BlackMana < 80 || WhiteMana < 80)
                    return false;
            }

            return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
        }
    }
}
