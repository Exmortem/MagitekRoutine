using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Ninja
{
    internal static class Utility
    {

        public static async Task<bool> PrePullHide()
        {

            if (ActionResourceManager.Ninja.HutonTimer.TotalSeconds <= 0)
                return false;

            if ((uint)Spells.Ten.Charges == Spells.Ten.MaxCharges)
                return false;

            return await Spells.Hide.Cast(Core.Me);

        }
        
    }
}
