using Magitek.Logic.Monk;
using System.Threading.Tasks;

namespace Magitek.Rotations.Monk
{
    internal static class CombatBuff
    {
        public static async Task<bool> Execute()
        {
            if (await Buff.FormShift()) return true;
            if (await Buff.Meditate()) return true;
            return false;
        }
    }
}