using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class Aoe
    {
        
        public static async Task<bool> HellfrogMedium()
        {

            if (Core.Me.ClassLevel < 62)
                return false;

            if (!Spells.HellfrogMedium.IsKnown())
                return false;

            //dumping ninki before mug is missung
            if (ActionResourceManager.Ninja.NinkiGauge < 90)
                return false;

            //Smart Target Logic needs to be addded
            return await Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget);
        }

    }
}
