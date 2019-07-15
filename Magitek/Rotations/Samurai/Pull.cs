using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Logic.Samurai;
using Magitek.Utilities;

namespace Magitek.Rotations.Samurai
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

            return await SingleTarget.Hakaze();
        }
    }
}