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
        private static bool CheckBeforeExecuteJumps()
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

            return true;
        }


        /***************************************************************************
         *                                Single Target
         * *************************************************************************/
        public static async Task<bool> HighJump()
        {
            if (!DragoonSettings.Instance.UseHighJump)
                return false;
            

            if (!CheckBeforeExecuteJumps())
                return false;
            
            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            return await DragoonRoutine.HighJump.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MirageDive()
        {
            if (!DragoonSettings.Instance.UseMirageDive)
                return false;

            if (!CheckBeforeExecuteJumps())
                return false;

            if (!Core.Me.HasAura(Auras.DiveReady))
                return false;

            if (ActionResourceManager.Dragoon.DragonGaze == 2)
                return false;

            return await Spells.MirageDive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SpineshatterDive()
        {
            if (!DragoonSettings.Instance.UseSpineshatterDive)
                return false;

            if (!CheckBeforeExecuteJumps())
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

            if (!CheckBeforeExecuteJumps())
                return false;

            if (!Core.Me.HasAura(Auras.LanceCharge))
                return false;

            return await Spells.DragonfireDive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Stardiver()
        {
            if (!DragoonSettings.Instance.UseStardiver)
                return false;

            if (Spells.Geirskogul.IsReady())
                return false;

            if (!CheckBeforeExecuteJumps())
                return false;

            return await Spells.Stardiver.Cast(Core.Me.CurrentTarget);
        }
    }
}
