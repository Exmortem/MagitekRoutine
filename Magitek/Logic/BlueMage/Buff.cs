using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.BlueMage;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.BlueMage
{
    internal static class Buff
    {
        public static async Task<bool> OffGuard()
        {
            if(Spells.Surpanakha.Charges == 4)
            return await Spells.OffGuard.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}