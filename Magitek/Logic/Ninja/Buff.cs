using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Threading.Tasks;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;


namespace Magitek.Logic.Ninja
{
    internal static class Buff
    {
        
        public static async Task<bool> Kassatsu()
        {

            if (Core.Me.ClassLevel < 50)
                return false;

            if (!Spells.Kassatsu.IsKnown())
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || NinjaRoutine.)
                return false;

            return await Spells.Kassatsu.Cast(Core.Me);

        }

    }
}