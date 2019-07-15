using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Models.Scholar;
using Magitek.Utilities;

namespace Magitek.Rotations.Scholar
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }   
            }
            else
            {
                if (!ScholarSettings.Instance.DoDamage)
                    return false;
            }

            return await Combat.Execute();
        }
    }
}
