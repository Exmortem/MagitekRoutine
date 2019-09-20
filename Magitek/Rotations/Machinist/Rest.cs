using System.Threading.Tasks;
using ff14bot;
using Magitek.Models.Machinist;

namespace Magitek.Rotations.Machinist
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            return Core.Me.CurrentHealthPercent < 75;
        }
    }
}