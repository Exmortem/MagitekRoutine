using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Models.Reaper;
using static ff14bot.Managers.ActionResourceManager.Reaper;

namespace Magitek.Logic.Reaper
{
    internal static class Utility
    {

        public static async Task<bool> TrueNorth()
        {
            if (ReaperSettings.Instance.EnemyIsOmni) return false;

            return false;
        }

    }
}