using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlueMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.BlueMage
{
    internal static class Dispel
    {
        public static async Task<bool> Exuviation()
        {
            if (!BlueMageSettings.Instance.Dispel || !ActionManager.HasSpell(Spells.Exuviation.Id))
                return false;

            if (!Core.Me.HasAnyDispellableAura())
                return false;

            return await Spells.Exuviation.Cast(Core.Me);
        }

    }
}
