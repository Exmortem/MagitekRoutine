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
using static Magitek.Logic.RedMage.Utility;
using Magitek.Models.QueueSpell;
using ff14bot.Objects;

namespace Magitek.Logic.RedMage
{
    internal class SingleTarget
    {
        public static async Task<bool> Riposte()
        {

            if (Core.Me.ClassLevel == 1)
                return await Spells.Riposte.Cast(Core.Me.CurrentTarget);
            
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration))
               return false;

            if (Core.Me.ClassLevel >= Spells.Embolden.LevelAcquired
                && Spells.Embolden.Cooldown.Seconds <= 10)
                return false;
                        
            if (Core.Me.ClassLevel >= 10)
            
            {

                if (BlackMana < 20
                    || WhiteMana < 20)
                    return false;
            }

            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel < 50)

            {

                if (BlackMana < 35
                    || WhiteMana < 35)
                    return false;

            }

            if (Core.Me.ClassLevel >= 50)

            {
                                 
                if (BlackMana < 50 || WhiteMana < 50)
                    return false;

            }

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
                if (Casting.LastSpell == Spells.Riposte)
                    return await Spells.Zwerchhau.Cast(Core.Me.CurrentTarget);

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
                if (Casting.LastSpell == Spells.Zwerchhau)
                    return await Spells.Redoublement.Cast(Core.Me.CurrentTarget);
                
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

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (BlackMana < 80 || WhiteMana < 80)
                return false;

            return await Spells.Reprise.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Jolt()
        {
            if (Core.Me.ClassLevel < Spells.Jolt.LevelAcquired)
                return false;
                    
            if (Core.Me.HasAura(Auras.Swiftcast))
                return false;

            if (Core.Me.ClassLevel < 4)
                if (MovementManager.IsMoving && !Core.Me.HasAura(Auras.Dualcast))
                    return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                && Core.Me.ClassLevel >= 4)
                return false;

            if (Core.Me.HasAura(Auras.Acceleration))
                return false;
            
            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.Jolt.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> ScorchResolutionCombo()
        {
            if (Core.Me.ClassLevel < 80)
                return false;

            if (!Spells.Scorch.CanCast())
                return false;

            if (Casting.SpellCastHistory.Take(6).Any(s => s.Spell == Spells.Verholy)
                || Casting.SpellCastHistory.Take(6).Any(s => s.Spell == Spells.Verflare))
            {
                if (Core.Me.ClassLevel >= 90)
                {
                   if (Casting.SpellCastHistory.Take(6).Count(s => s.Spell == Spells.Scorch || s.Spell == Spells.Jolt || s.Spell == Spells.Resolution) < 2)
                        return await Spells.Scorch.Cast(Core.Me.CurrentTarget);
                }
                if (!Casting.SpellCastHistory.Take(6).Any(s => s.Spell == Spells.Scorch || s.Spell == Spells.Jolt || s.Spell == Spells.Resolution))
                    return await Spells.Scorch.Cast(Core.Me.CurrentTarget);
            }        
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

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.Fleche.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> Verflare()
        {
            if (Core.Me.ClassLevel < Spells.Verflare.LevelAcquired)
                return false;
                        
         
            if (!ActionManager.CanCast(Spells.Verflare, Core.Me.CurrentTarget))
                return false;

            if (ManaStacks() < 3)
                return false;

            if (BlackMana > WhiteMana
                && Core.Me.ClassLevel >= Spells.Verholy.LevelAcquired)
                return false;
                        
            return await Spells.Verflare.Cast(Core.Me.CurrentTarget);     

        }
        public static async Task<bool> Verholy()
        {
            if (Core.Me.ClassLevel < Spells.Verholy.LevelAcquired)
                return false;

            if (InAoeCombo()
                && Casting.LastSpell != Spells.Moulinet)
                return false;
            
            if (!ActionManager.CanCast(Spells.Verholy, Core.Me.CurrentTarget))
                return false;

            if (ManaStacks() < 3)
                return false;

            if (WhiteMana > BlackMana)
                return false;

            return await Spells.Verholy.Cast(Core.Me.CurrentTarget); 

        }
        public static async Task<bool> Verthunder()
        {
            if (Core.Me.ClassLevel < Spells.Verthunder.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (Core.Me.HasAura(Auras.VerfireReady))
                if (!Core.Me.HasAura(Auras.VerstoneReady))
                    return false;
                else if (Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.VerstoneReady).TimespanLeft.TotalMilliseconds < Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.VerfireReady).TimespanLeft.TotalMilliseconds)
                    return false;

            if (Core.Me.ClassLevel >= 10)
            {
                if (WhiteMana <= BlackMana && !Core.Me.HasAura(Auras.VerstoneReady))
                    return false;
            }


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

        public static async Task<bool> Veraero()
        {
            if (Core.Me.ClassLevel < Spells.Veraero.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.VerstoneReady))
                if (!Core.Me.HasAura(Auras.VerfireReady))
                    return false;
                else if (Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.VerfireReady).TimespanLeft.TotalMilliseconds < Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.VerstoneReady).TimespanLeft.TotalMilliseconds)
                    return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (WhiteMana > BlackMana && !Core.Me.HasAura(Auras.VerfireReady))
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

            if (!Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Swiftcast))
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;
            
            if ((WhiteMana <= BlackMana) || Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.VerstoneReady).TimespanLeft.TotalMilliseconds < 4000)
                return await Spells.Verstone.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Verfire()
        {
            if (Core.Me.ClassLevel < Spells.Verfire.LevelAcquired)
                return false;

            if (!Core.Me.HasAura(Auras.VerfireReady))
                return false;

            if (MovementManager.IsMoving)
                return false;
             
            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Swiftcast))
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;
                        
            if (BlackMana <= WhiteMana || Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.VerfireReady).TimespanLeft.TotalMilliseconds < 4000)
                return await Spells.Verfire.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> CorpsACorps()
        {

            if (!RedMageSettings.Instance.CorpsACorps)
                return false;

            if (Core.Me.ClassLevel < Spells.CorpsACorps.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (Core.Me.CurrentTarget.Distance() >= (3 + Core.Me.CombatReach)
                && RedMageSettings.Instance.CorpsACorpsInMeleeRangeOnly)
                return false;          

            if (Spells.CorpsACorps.Charges <= 0 + RedMageSettings.Instance.SaveCorpsACorpsCharges)
                return false;

            return await Spells.CorpsACorps.Cast(Core.Me.CurrentTarget);

        }
        public static async Task<bool> Displacement()
        {
            if (!RedMageSettings.Instance.Displacement)
                return false;

            if (Core.Me.ClassLevel < Spells.Displacement.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget.Distance() > (3 + Core.Me.CurrentTarget.CombatReach))
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (Spells.Displacement.Charges >= 1)
                return await Spells.Displacement.Cast(Core.Me.CurrentTarget);


            return false;            
        }
        public static async Task<bool> Engagement()
        {
            if (!RedMageSettings.Instance.Engagement)
                return false;

            if (Core.Me.ClassLevel < Spells.Engagement.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget.Distance() > (3 + Core.Me.CurrentTarget.CombatReach))
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (Spells.Engagement.Charges >= 1)
                return await Spells.Engagement.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}
