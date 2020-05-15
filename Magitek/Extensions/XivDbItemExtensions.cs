using Magitek.Models.Debugging;
using Magitek.Models.WebResources;
using System.Linq;

namespace Magitek.Extensions
{
    internal static class XivDbItemExtensions
    {
        public static double GetIcon(this XivDbItem item, bool isStatus = false)
        {
            return isStatus ? GetStatusIcon(item.Id) : GetActionIcon(item.Id);
        }

        public static double GetIcon(this EnemySpellCastInfo spell)
        {
            return GetActionIcon(spell.Id);
        }

        public static double GetIcon(this TargetAuraInfo aura)
        {
            return GetStatusIcon(aura.Id);
        }

        private static double GetStatusIcon(uint id)
        {
            var status = Utilities.XivDataHelper.XivDbStatuses.FirstOrDefault(r => r.Id == id);
            return status?.Icon ?? 0;
        }

        private static double GetActionIcon(uint id)
        {
            var action = Utilities.XivDataHelper.XivDbActions.FirstOrDefault(r => r.Id == id);
            return action?.Icon ?? 0;
        }
    }
}
