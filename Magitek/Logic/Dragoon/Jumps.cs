using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using System.Threading.Tasks;
using DragoonRoutine = Magitek.Utilities.Routines.Dragoon;

namespace Magitek.Logic.Dragoon
{
    internal static class Jumps
    {
        public static async Task<bool> Execute()
        {
            if (!DragoonSettings.Instance.UseJumps)
                return false;

            if (DragoonSettings.Instance.SafeJumpLogic)
            {
                if (!Core.Me.CurrentTarget.InView())
                    return false;
            }

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (DragoonRoutine.JumpsList.Contains(Casting.LastSpell))
                return false;

            if (await HighJump()) return true;
            if (await Stardiver()) return true;
            if (await DragonfireDive()) return true;
            return await SpineshatterDive();
        }


        /***************************************************************************
         *                                Single Target
         * *************************************************************************/
        public static async Task<bool> HighJump()
        {
            if (!DragoonSettings.Instance.UseHighJump)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            return await DragoonRoutine.HighJump.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MirageDive()
        {
            if (!DragoonSettings.Instance.UseMirageDive)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (!Core.Me.HasAura(Auras.DiveReady))
                return false;

            // Don't mirage dive if at 2 eyes.
            if (ActionResourceManager.Dragoon.DragonGaze == 2)
                return false;

            return await Spells.MirageDive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SpineshatterDive()
        {
            if (!DragoonSettings.Instance.UseSpineshatterDive)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (Spells.SpineshatterDive.Charges <= 1 && Spells.LanceCharge.IsKnownAndReady(10000))
                return false;

            return await Spells.SpineshatterDive.Cast(Core.Me.CurrentTarget);
        }



        /***************************************************************************
         *                           AOE
         * *************************************************************************/
        public static async Task<bool> DragonfireDive()
        {
            if (!DragoonSettings.Instance.UseDragonfireDive)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (!Core.Me.HasAura(Auras.LanceCharge))
                return false;

            return await Spells.DragonfireDive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Stardiver()
        {
            if (!DragoonSettings.Instance.UseStardiver)
                return false;

            return await Spells.Stardiver.Cast(Core.Me.CurrentTarget);
        }
    }
}
