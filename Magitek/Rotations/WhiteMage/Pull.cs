using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;

namespace Magitek.Rotations.WhiteMage
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
                if (!WhiteMageSettings.Instance.DoDamage)
                    return false;
            }
            
            if (Core.Me.InCombat)
                return false;

            return await Combat.Execute();
        }
    }
}
