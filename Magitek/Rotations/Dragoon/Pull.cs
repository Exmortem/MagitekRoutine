using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Utilities;

namespace Magitek.Rotations.Dragoon
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 5 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat.Execute();
        }
    }
}