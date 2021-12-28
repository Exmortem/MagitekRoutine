using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Logic.Dragoon
{
    internal static class Utility
    {

        public static async Task<bool> TrueNorth()
        {
            if (DragoonSettings.Instance.EnemyIsOmni || !DragoonSettings.Instance.UseTrueNorth) 
                return false;

            if (Casting.LastSpell == Spells.TrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.LastSpell != Spells.FullThrust && Casting.LastSpell != Spells.ChaosThrust && Casting.LastSpell != Spells.WheelingThrust && Casting.LastSpell != Spells.FangAndClaw)
                return false;

            return await Spells.TrueNorth.Cast(Core.Me);
        }

    }
}