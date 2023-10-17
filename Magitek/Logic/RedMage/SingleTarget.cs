using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System;
using System.Linq;
using Magitek.Models.RedMage;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal class SingleTarget
    {
        public static async Task<bool> Riposte()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (InAoeCombo())
                return false;

            //If embolden coming off cd soon, wait
            if (Core.Me.ClassLevel >= Spells.Embolden.LevelAcquired
                && Spells.Embolden.Cooldown.Seconds <= 10)
                return false;
          
            if (Core.Me.ClassLevel < 35)
            {
                if (BlackMana >= 20 
                    && WhiteMana >= 20)
                    return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
            }
            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel < 50
                && Casting.LastSpell != Spells.Riposte)
            {
                if (BlackMana >= 35
                    && WhiteMana >= 35)
                    return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
            }

            if (Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau)
                return false;

            if (BlackMana < 50 || WhiteMana < 50)
                return false;
            
            return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Zwerchhau()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (Core.Me.ClassLevel < Spells.Zwerchhau.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (BlackMana >= 15 && WhiteMana >= 15)
            {
                if (Casting.LastSpell == Spells.Riposte
                    && Casting.LastSpell != Spells.Zwerchhau)
                {
                    return await Spells.Zwerchhau.Cast(Core.Me.CurrentTarget);
                }
                return false;
            }
            return false;
        }
        public static async Task<bool> Redoublement()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (Core.Me.ClassLevel < Spells.Redoublement.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (BlackMana >= 15 && WhiteMana >= 15)
            {
                if (Casting.LastSpell == Spells.Zwerchhau
                    && Casting.LastSpell != Spells.Redoublement)
                {
                    return await Spells.Redoublement.Cast(Core.Me.CurrentTarget);
                }
                return false;
            }
            return false;
        }
        public static async Task<bool> Reprise()
        {
            if (!RedMageSettings.Instance.UseReprise)
                return false;

            if (Core.Me.ClassLevel < Spells.Reprise.LevelAcquired)
                return false;

            if (!MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (InCombo())
                return false;

            if (InAoeCombo())
                return false;

            if (BlackMana < 5 || WhiteMana < 5)
                return false;

            if (BlackMana == 50 || WhiteMana == 50)
                return false;

            return await Spells.Reprise.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Jolt()
        {
            if (Core.Me.ClassLevel < Spells.Jolt.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast))
                return false;

            if (Core.Me.HasAura(Auras.VerfireReady)
                && !Core.Me.HasAura(Auras.Dualcast))
                return await Spells.Verfire.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.VerstoneReady)
                && !Core.Me.HasAura(Auras.Dualcast))
                return await Spells.Verstone.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Dualcast)
                && Core.Me.ClassLevel >= 4)
                return false;

            if (Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (InCombo())
                return false;

            if (InAoeCombo())
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            return await Spells.Jolt.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Scorch()
        {
            if (Core.Me.ClassLevel < Spells.Scorch.LevelAcquired)
                return false;

            //if (!Spells.Scorch.CanCast())
            //    return false;

            if (!ActionManager.CanCast(Spells.Scorch, Core.Me.CurrentTarget))
                return false;

            if (InAoeCombo())
                return false;

            if (!Spells.Scorch.IsKnownAndReady())
                return false;

            if (Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Verflare)
                return await Spells.Scorch.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Resolution()
        {
            if (Core.Me.ClassLevel < Spells.Resolution.LevelAcquired)
                return false;

            //if (!Spells.Resolution.CanCast())
            //    return false;

            if (!ActionManager.CanCast(Spells.Resolution, Core.Me.CurrentTarget))
                return false;

            if (InAoeCombo())
                return false;

            if (!Spells.Resolution.IsKnownAndReady())
                return false;

            if (Casting.LastSpell == Spells.Scorch)
                return await Spells.Resolution.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Fleche()
        {
            if (!RedMageSettings.Instance.Fleche)
                return false;

            if (Core.Me.ClassLevel < Spells.Fleche.LevelAcquired)
                return false;

            if (Spells.Fleche.Cooldown != TimeSpan.Zero)
                return false;

            if (InCombo())
                return false;

            if (InAoeCombo())
                return false;

            return await Spells.Fleche.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Verthunder()
        {
            if (Core.Me.ClassLevel < Spells.Verthunder.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.HasAura(Auras.VerfireReady))
                return false;

            if (InCombo())
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            if (BlackMana + 6 - WhiteMana > 15
                || BlackMana > WhiteMana)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Swiftcast)
                && RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Acceleration)
                && !Core.Me.HasAura(Auras.VerfireReady))
                return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Verfire()
        {
            if (Core.Me.ClassLevel < Spells.Verfire.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Swiftcast))
                return false;

            if (!Core.Me.HasAura(Auras.VerfireReady))
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            if (InCombo())
                return false;

            if (BlackMana + 5 - WhiteMana < 15
                && BlackMana < WhiteMana)
                return await Spells.Verfire.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Verflare()
        {
            if (Core.Me.ClassLevel < Spells.Verflare.LevelAcquired)
                return false;

            if (InAoeCombo()
                && Casting.LastSpell != Spells.Moulinet)
                return false;

            //if (!Spells.Verflare.IsKnownAndReady())
            //    return false;

            //if (!Spells.Verflare.CanCast())
            //    return false;

            if (!ActionManager.CanCast(Spells.Verflare, Core.Me.CurrentTarget))
                return false;

            if (Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Scorch)
                return false;

            if (BlackMana > WhiteMana
                && Core.Me.ClassLevel >= Spells.Verholy.LevelAcquired)
                return false;

            if (Casting.SpellCastHistory.Take(3).All(x => x.Spell == Spells.Moulinet)
                && BlackMana <= WhiteMana)
                return await Spells.Verflare.Cast(Core.Me.CurrentTarget);

            //Trying this instead to be more flexible
            if (Casting.SpellCastHistory.Take(3).Any(s => s.Spell == Spells.Redoublement || s.Spell == Spells.Moulinet)
                && BlackMana <= WhiteMana)
                return await Spells.Verflare.Cast(Core.Me.CurrentTarget);

            /*if (Casting.LastSpell != Spells.Redoublement
                || Casting.LastSpell != Spells.Moulinet)
                return false;
            */
            return false;
        }
        public static async Task<bool> Veraero()
        {
            if (Core.Me.ClassLevel < Spells.Veraero.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            if (InCombo())
                return false;
            
            if (WhiteMana + 6 - BlackMana > 15
                || WhiteMana >= BlackMana)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return await Spells.Veraero.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Swiftcast)
                && RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                return await Spells.Veraero.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Acceleration)
                && !Core.Me.HasAura(Auras.VerstoneReady))
                return await Spells.Veraero.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Verstone()
        {
            if (Core.Me.ClassLevel < Spells.Verstone.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (!Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Swiftcast))
                return false;
            
            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            if (InCombo())
                return false;

            if (WhiteMana + 5 - BlackMana > 15
                && WhiteMana >= BlackMana)
                return false;

            return await Spells.Verstone.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Verholy()
        {
            if (Core.Me.ClassLevel < Spells.Verholy.LevelAcquired)
                return false;

            if (InAoeCombo()
                && Casting.LastSpell != Spells.Moulinet)
                return false;

            //if (!Spells.Verholy.IsKnownAndReady())
            //    return false;

            //if (!Spells.Verflare.CanCast())
            //    return false;

            if (!ActionManager.CanCast(Spells.Verholy, Core.Me.CurrentTarget))
                return false;

            if (Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Scorch)
                return false;

            if (WhiteMana > BlackMana)
                return false;

            if (Casting.SpellCastHistory.Take(3).All(x => x.Spell == Spells.Moulinet)
                && BlackMana > WhiteMana)
                return await Spells.Verholy.Cast(Core.Me.CurrentTarget);

            //Trying this instead to be more flexible
            if (Casting.SpellCastHistory.Take(3).Any(s => s.Spell == Spells.Redoublement || s.Spell == Spells.Moulinet)
                && BlackMana > WhiteMana)
                return await Spells.Verholy.Cast(Core.Me.CurrentTarget);

            /*if (Casting.LastSpell != Spells.Redoublement
                || Casting.LastSpell != Spells.Moulinet)
                return false;
            */
            return false;
        }
        public static async Task<bool> CorpsACorps()
        {
            if (!RedMageSettings.Instance.CorpsACorps)
                return false;

            if (Core.Me.ClassLevel < Spells.CorpsACorps.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.CurrentTarget.Distance() <= 3)
                return false;

            if (InCombo())
                return false;

            if (Spells.CorpsACorps.Cooldown != TimeSpan.Zero
                && Spells.CorpsACorps.Charges == 0)
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return await Spells.CorpsACorps.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Displacement()
        {
            if (!RedMageSettings.Instance.Displacement)
                return false;

            if (Core.Me.ClassLevel < Spells.Displacement.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.CurrentTarget.Distance() > 3)
                return false;

            if (InCombo())
                return false;

            if (Spells.Displacement.Cooldown != TimeSpan.Zero
                && Spells.Displacement.Charges == 0)
                return false;

            if (Core.Me.ClassLevel < 50
                && Casting.LastSpell == Spells.Zwerchhau)
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 50
                && Core.Me.ClassLevel < 68
                && Casting.LastSpell == Spells.Redoublement)
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 68
                && Core.Me.ClassLevel < 70
                && Casting.LastSpell == Spells.Verflare)
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 70
                && Core.Me.ClassLevel < 80
                && (Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Verholy))
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 80
                && Core.Me.ClassLevel < 90
                && Casting.LastSpell == Spells.Scorch)
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel == 90
                && Casting.LastSpell == Spells.Resolution)
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);

            return false;            
        }
        public static async Task<bool> Engagement()
        {
            if (!RedMageSettings.Instance.Engagement)
                return false;

            if (Core.Me.ClassLevel < Spells.Engagement.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.CurrentTarget.Distance() > 3)
                return false;

            if (InCombo())
                return false;

            if (Spells.Engagement.Cooldown != TimeSpan.Zero
                && Spells.Engagement.Charges == 0)
                return false;

            if (Core.Me.ClassLevel < 50
                && Casting.LastSpell == Spells.Zwerchhau)
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 50
                && Core.Me.ClassLevel < 68
                && Casting.LastSpell == Spells.Redoublement)
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 68
                && Core.Me.ClassLevel < 70
                && Casting.LastSpell == Spells.Verflare)
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 70
                && Core.Me.ClassLevel < 80
                && (Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Verholy))
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 80
                && Core.Me.ClassLevel < 90
                && Casting.LastSpell == Spells.Scorch)
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel == 90
                && Casting.LastSpell == Spells.Resolution)
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            return false;
        }
        private static bool InCombo()
        {
            if (Core.Me.ClassLevel >=6
                && Core.Me.ClassLevel < 35)
            {
                if (Casting.LastSpell == Spells.CorpsACorps)
                    return true;
            }
            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel < 50)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte)
                    return true;
            }
            if (Core.Me.ClassLevel >= 50
                && Core.Me.ClassLevel < 68)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau)
                    return true;
            }
            if (Core.Me.ClassLevel >= 68
                && Core.Me.ClassLevel < 80)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement)
                    return true;
            }
            if (Core.Me.ClassLevel >= 80
                && Core.Me.ClassLevel < 90)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Verflare)
                    return true;
            }
            
            if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Scorch)
                return true;

            return false;
        }
        public static bool InAoeCombo()
        {
            if (!RedMageSettings.Instance.UseAoe)
                return false;

            if (Core.Me.EnemiesNearby(10).Count() < RedMageSettings.Instance.AoeEnemies)
                return false;

            if (Casting.SpellCastHistory.Take(3).All(x => x.Spell == Spells.Moulinet))
                return true;

            if (WhiteMana >= 60
                    && BlackMana >= 60)
                return true;

            if (Casting.LastSpell == Spells.Moulinet)
                if (WhiteMana >= 20
                    && BlackMana >= 20)
                    return true;

            return false;
        }
    }
}
