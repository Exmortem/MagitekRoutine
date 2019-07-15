using System.Threading.Tasks;
using Magitek.Logic.Paladin;

namespace Magitek.Rotations.Paladin
{
    internal static class CombatBuff
    {
        public static async Task<bool> Execute()
        {
            return await Buff.Oath();
        }
    }
}