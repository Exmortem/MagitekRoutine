using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Utilities
{
    internal static class Ether
    {
        public static async Task<bool> UseEther(int etherItemId)
        {
            if (etherItemId == 0)
                return false;

            var etherItem = InventoryManager.FilledSlots.FirstOrDefault(s => s.RawItemId == etherItemId);

            if (etherItem == null)
                return false;

            if (!etherItem.CanUse(Core.Me))
                return false;

            while (etherItem.CanUse())
            {
                Logger.WriteInfo($@"Use ether : {etherItem.Name}");
                etherItem.UseItem(Core.Me);
                await Coroutine.Wait(700, () => false);

                if (etherItem == null || !etherItem.CanUse())
                    return true;
            }
            return false;
        }
    }
}
