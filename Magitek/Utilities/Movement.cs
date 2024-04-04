using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using Magitek.Extensions;
using System.Threading.Tasks;
using BaseSettings = Magitek.Models.Account.BaseSettings;

namespace Magitek.Utilities
{
    internal static class Movement
    {

        public static void NavigateToUnitLos(GameObject unit, float distance)
        {
            if (!BaseSettings.Instance.MagitekMovement)
                return;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && MovementManager.IsMoving)
                return;

            if (unit == null)
                return;

            if (AvoidanceManager.IsRunningOutOfAvoid)
                return;

            //if (!MovementManager.IsMoving && !unit.InView() && !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing))
            //   Core.Me.Face(Core.Me.CurrentTarget);

            if (unit.Distance(Core.Me) > distance)
            {
                Navigator.MoveTo(new MoveToParameters(unit.Location));
            }

            if (Core.Me.Distance(unit.Location) <= distance && unit.InView() && unit.InLineOfSight())
            {
                if (MovementManager.IsMoving)
                {
                    Navigator.PlayerMover.MoveStop();
                }
            }
            else
            {
                Navigator.MoveTo(new MoveToParameters(unit.Location));
            }
        }

        public static async Task<bool> Dismount()
        {
            if (!Core.Me.IsMounted)
                return false;

            while (Core.Me.IsMounted)
            {
                ActionManager.Mount();
                await Coroutine.Yield();
            }

            return true;
        }
    }
}
