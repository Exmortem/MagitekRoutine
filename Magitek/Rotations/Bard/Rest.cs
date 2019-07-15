using System.Threading.Tasks;
using ff14bot;
using Magitek.Models.Bard;

namespace Magitek.Rotations.Bard
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            return Core.Me.CurrentHealthPercent < BardSettings.Instance.RestHealthPercent;
        }
    }
}