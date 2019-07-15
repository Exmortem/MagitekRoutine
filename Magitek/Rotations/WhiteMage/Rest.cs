using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Rotations.WhiteMage
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 2)
                return false;

            await Spells.Cure.Heal(Core.Me);
            return true;
        }
    }
}
