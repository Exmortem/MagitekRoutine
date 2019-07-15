using System.Threading.Tasks;
using ff14bot;

namespace Magitek.Rotations.Samurai
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            return Core.Me.CurrentHealthPercent < 75 || Core.Me.CurrentManaPercent < 50;
        }
    }
}