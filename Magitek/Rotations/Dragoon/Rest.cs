using System.Threading.Tasks;
using ff14bot;
using Magitek.Models.Dragoon;

namespace Magitek.Rotations.Dragoon
{
    internal static class Rest
    {
        public static async Task<bool> Execute()
        {
            return Core.Me.CurrentHealthPercent < DragoonSettings.Instance.RestHealthPercent;
        }
    }
}