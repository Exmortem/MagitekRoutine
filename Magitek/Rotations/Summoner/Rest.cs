using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Rotations.Summoner
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 4)
                return false;
            
            return await Spells.Physick.Heal(Core.Me);
        }
    }
}