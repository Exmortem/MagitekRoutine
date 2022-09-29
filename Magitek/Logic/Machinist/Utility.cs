using ff14bot;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Machinist;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Machinist
{
    internal static class Utility
    {
        public static async Task<bool> Tactician()
        {

            if (!MachinistSettings.Instance.ForceTactician)
                return false;

            if (!await Spells.Tactician.Cast(Core.Me)) 
                return false;

            MachinistSettings.Instance.ForceTactician = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.AirAnchor.IsKnown() && !Spells.AirAnchor.IsReady(2500))
                return false;

            if (Spells.Wildfire.IsKnown() && !Spells.Wildfire.IsReady(10000))
                return false;

            return await PhysicalDps.UsePotion(MachinistSettings.Instance);
        }

    }
}
