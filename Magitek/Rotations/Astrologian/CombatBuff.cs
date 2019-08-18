using System.Threading.Tasks;
using Magitek.Logic.Astrologian;

namespace Magitek.Rotations.Astrologian
{
    internal static class CombatBuff
    {
        public static async Task<bool> Execute()
        {
            return await Cards.PlayCards();
        }
    }
}
