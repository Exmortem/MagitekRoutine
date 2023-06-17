using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Utilities
{
    internal static class Potion
    {
        public static async Task<bool> UsePotion(int potionItemId)
        {
            if (potionItemId == 0)
                return false;

            var potionItem = InventoryManager.FilledSlots.FirstOrDefault(s => s.RawItemId == potionItemId);

            if (potionItem == null)
                return false;

            if (!potionItem.CanUse(Core.Me))
                return false;

            while(potionItem.CanUse())
            {
                Logger.WriteInfo($@"Use potion : {potionItem.Name}");
                potionItem.UseItem(Core.Me);
                await Coroutine.Wait(700, () => false);

                if (potionItem == null || !potionItem.CanUse())
                    return true;
            }
            return false;
        }
    }
}
