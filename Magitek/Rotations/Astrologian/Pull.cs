using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Models.Astrologian;
using Magitek.Utilities;

namespace Magitek.Rotations.Astrologian
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
                if (Globals.InParty)
                {
                    if (!AstrologianSettings.Instance.DoDamage)
                        return false;
                }
            }
            
            if (Core.Me.InCombat)
                return false;

            return await Combat.Execute();
        }
    }
}
