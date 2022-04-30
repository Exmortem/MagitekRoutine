using ff14bot;
using ff14bot.Objects;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using Magitek.Models.Dragoon;
using Magitek.Toggles;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using DragoonRoutine = Magitek.Utilities.Routines.Dragoon;

namespace Magitek.Logic.Dragoon
{
    internal static class SingleTarget
    {

        public static async Task<bool> TrueThrust()
        {
            if (Core.Me.HasAura(Auras.SharperFangandClaw))
                return false;

            if (Core.Me.HasAura(Auras.EnhancedWheelingThrust))
                return false;

            if (Core.Me.HasAura(Auras.DraconianFire))
                return false;

            return await Spells.TrueThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RaidenThrust()
        {
            if (!Core.Me.HasAura(Auras.DraconianFire))
                return false;

            return await Spells.RaidenThrust.Cast(Core.Me.CurrentTarget);
        }

        /***************************************************************************
        *                           Combo 1
        * *************************************************************************/
        public static async Task<bool> Disembowel()
        {
            if (!DragoonRoutine.CanContinueComboAfter(Spells.TrueThrust) && !DragoonRoutine.CanContinueComboAfter(Spells.RaidenThrust))
                return false;

            return await Spells.Disembowel.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ChaoticSpring()
        {
            if (!DragoonRoutine.CanContinueComboAfter(Spells.Disembowel))
                return false;

            return await DragoonRoutine.ChaoticSpring.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WheelingThrust()
        {
            if (!Core.Me.HasAura(Auras.EnhancedWheelingThrust))
                return false;

            return await Spells.WheelingThrust.Cast(Core.Me.CurrentTarget);
        }

        /***************************************************************************
         *                           Combo 2
         * *************************************************************************/
        public static async Task<bool> VorpalThrust()
        {
            if (!DragoonRoutine.CanContinueComboAfter(Spells.TrueThrust) && !DragoonRoutine.CanContinueComboAfter(Spells.RaidenThrust))
                return false;

            if (!Core.Me.HasAura(Auras.PowerSurge))
                return false;

            Aura PowerSurgeAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.PowerSurge);
            if (Core.Me.HasAura(Auras.PowerSurge) && PowerSurgeAura.TimespanLeft.TotalMilliseconds <= 6000)
                return false;

            if (Spells.ChaoticSpring.IsKnown())
            {
                if (!Core.Me.CurrentTarget.HasAura(Auras.ChaoticSpring))
                    return false; 
                
                Aura ChaoticSpringAura = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Auras.ChaoticSpring);
                if (Core.Me.CurrentTarget.HasAura(Auras.ChaoticSpring) && ChaoticSpringAura.TimespanLeft.TotalMilliseconds <= 6000)
                    return false;
            } else
            {
                if (!Core.Me.CurrentTarget.HasAura(Auras.ChaosThrust))
                    return false; 
                
                Aura ChaosThrustAura = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Auras.ChaosThrust);
                if (Core.Me.CurrentTarget.HasAura(Auras.ChaosThrust) && ChaosThrustAura.TimespanLeft.TotalMilliseconds <= 6000)
                    return false;
            }

            return await Spells.VorpalThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeavensThrust()
        {
            if (!DragoonRoutine.CanContinueComboAfter(Spells.VorpalThrust))
                return false;

            return await DragoonRoutine.HeavensThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FangAndClaw()
        {
            if (!Core.Me.HasAura(Auras.SharperFangandClaw))
                return false;

            return await Spells.FangAndClaw.Cast(Core.Me.CurrentTarget);
        }
        public static bool ForceLimitBreak()
        {
            if (!DragoonSettings.Instance.ForceLimitBreak)
                return false;

            if (PartyManager.NumMembers == 8 && !Casting.SpellCastHistory.Any(s => s.Spell == Spells.DragonsongDive) && Spells.TrueThrust.Cooldown.TotalMilliseconds < 500)
            {

                ActionManager.DoAction(Spells.DragonsongDive, Core.Me.CurrentTarget);
                DragoonSettings.Instance.ForceLimitBreak = false;
                TogglesManager.ResetToggles();
                return true;

            }

            if (PartyManager.NumMembers == 4 && !Casting.SpellCastHistory.Any(s => s.Spell == Spells.Braver) && !Casting.SpellCastHistory.Any(s => s.Spell == Spells.Bladedance) && Spells.TrueThrust.Cooldown.TotalMilliseconds < 500)
            {
               
                if (!ActionManager.DoAction(Spells.Bladedance, Core.Me.CurrentTarget))
                    ActionManager.DoAction(Spells.Braver, Core.Me.CurrentTarget);
                DragoonSettings.Instance.ForceLimitBreak = false;
                TogglesManager.ResetToggles();
                return true;

            }

            return false;

        }

    }
}
