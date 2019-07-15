using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Rotations.Scholar
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 4)
                return false;

            await Spells.Physick.Heal(Core.Me);
            return true;
        }
    }
}
