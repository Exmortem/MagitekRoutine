using System.Collections.Generic;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Roles;
using Magitek.Utilities;
using System.Threading.Tasks;


namespace Magitek.Logic.Roles
{
    internal static class MagicDps
    {
        public static async Task<bool> Interrupt(MagicDpsSettings settings)
        {
            List<SpellData> stuns = new List<SpellData>();
            List<SpellData> interrupts = new List<SpellData>();

            if (Core.Me.IsBlueMage())
            {
                interrupts.Add(Spells.FlyingSardine);
            }

            return await InterruptAndStunLogic.StunOrInterrupt(stuns, interrupts, settings.Strategy);
        }

    }
}
