using System.Threading.Tasks;
using ff14bot;
using Magitek.Models.Paladin;

namespace Magitek.Rotations.Paladin
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            return Core.Me.CurrentHealthPercent < PaladinSettings.Instance.RestHealthPercent;
        }
    }
}