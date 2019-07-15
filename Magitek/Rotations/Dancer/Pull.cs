using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic.Dancer;
using Magitek.Utilities;

namespace Magitek.Rotations.Dancer
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            if (!BotManager.Current.IsAutonomous)
            {
                return await Combat.Execute();
            }

            Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);

            return await SingleTarget.Cascade();
        }
    }
}