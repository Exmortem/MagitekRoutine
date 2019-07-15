using System.Linq;
using ff14bot.Objects;
using Magitek.Utilities.Managers;

namespace Magitek.Extensions
{
    internal static class CharacterExtensions
    {
        public static bool NeedsDispel(this Character unit, bool highPriority = false)
        {
            return unit.CharacterAuras.Select(r => r.Id).Intersect(DispelManager.HighPriorityDispels.Union(DispelManager.NormalDispels)).Any();
        }

        public static bool HasAnyDispellableAura(this Character unit)
        {
            return unit.CharacterAuras.Any(r => r.IsDispellable);
        }
    }
}
