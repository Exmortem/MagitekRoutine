using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using Magitek.Extensions;

namespace Magitek.Utilities
{
    internal static class Movement
    {
        public static void NavigateToUnitLos(GameObject unit, float distance)
        {
            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return;
            
            if (unit == null)
                return;  

            if (!MovementManager.IsMoving && !unit.InView())
                Core.Me.Face(Core.Me.CurrentTarget);

            if (AvoidanceManager.Avoids.Any(r => r.IsPointInAvoid(unit.Location)))
                return;

            if (unit.Distance(Core.Me) > distance)
            {
                Navigator.MoveTo(new MoveToParameters(unit.Location));
            }

            if (Core.Me.Distance(Core.Me.CurrentTarget.Location) <= distance && unit.InView() && unit.InLineOfSight())
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
