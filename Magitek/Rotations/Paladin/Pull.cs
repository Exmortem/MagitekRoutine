using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Utilities;

namespace Magitek.Rotations.Paladin
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 4);
            }

            return await Combat.Execute();
        }
    }
}