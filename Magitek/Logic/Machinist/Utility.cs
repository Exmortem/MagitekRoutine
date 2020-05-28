using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Machinist
{
    class Utility
    {
        public static async Task<bool> Tactician()
        {

            if (!MachinistSettings.Instance.ForceTactician)
                return false;

            if (!await Spells.Tactician.Cast(Core.Me)) return false;
            MachinistSettings.Instance.ForceTactician = false;
            TogglesManager.ResetToggles();
            return true;

        }

    }
}
