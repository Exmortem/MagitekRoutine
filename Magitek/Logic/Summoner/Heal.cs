using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;

namespace Magitek.Logic.Summoner
{
    internal static class Heal
    {
        public static async Task<bool> Physick()
        {
            if (Core.Me.ClassLevel < 4) return false;

            //return await Spells.Physick.Heal(Core.Me);
            return await Task.FromResult(false);
        }
    }
}