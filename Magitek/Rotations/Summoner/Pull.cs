using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Rotations.Summoner
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
                
                return await Spells.SmnBio.CastAura(Core.Me.CurrentTarget, Auras.Bio);
            }

            return await Combat.Execute();
        }
    }
}