using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Gambits.Conditions;

namespace Magitek.Gambits.Actions
{
    public class UseItemOnSelfAction : GambitAction
    {
        public UseItemOnSelfAction() : base(GambitActionTypes.UseItemOnSelf)
        {
        }

        public string ItemName { get; set; }
        public bool AnyQuality { get; set; }
        public bool NormalQualityOnly { get; set; }
        public bool HighQualityOnly { get; set; }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            var bagSlot = InventoryManager.FilledSlots.FirstOrDefault(r =>
                string.Equals(r.Name, ItemName, StringComparison.CurrentCultureIgnoreCase) &&
                (AnyQuality || HighQualityOnly && r.IsHighQuality || NormalQualityOnly && !r.IsHighQuality));

            if (bagSlot == null)
                return false;

            if (conditions.Any(condition => !condition.Check(Core.Me)))
                return false;

            if (!bagSlot.CanUse(Core.Me))
                return false;

            // id 846 for backingaction if medicated
            // id 845 for backingaction if well fed

            if (bagSlot.Item.BackingAction != null)
            {
                if (bagSlot.Item.BackingAction.Cooldown.TotalMilliseconds > 0)
                    return false;

                if (bagSlot.Item.BackingAction.Id == 846)
                {
                    while (!Core.Me.HasAura("Medicated"))
                    {
                        bagSlot.UseItem(Core.Me);
                        await Coroutine.Yield();
                    }

                    return true;
                }

                if (bagSlot.Item.BackingAction.Id == 845)
                {
                    while (!Core.Me.HasAura("Well Fed"))
                    {
                        bagSlot.UseItem(Core.Me);
                        await Coroutine.Yield();
                    }

                    return true;
                }
            }
         
            bagSlot.UseItem(Core.Me);
            return true;
        }
    }
}