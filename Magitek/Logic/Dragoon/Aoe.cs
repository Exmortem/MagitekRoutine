using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Logic.Dragoon
{
    internal static class Aoe
    {
        public static async Task<bool> Nastrond()
        {
            return await Spells.Nastrond.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DoomSpike()
        {
            if (!DragoonSettings.Instance.Aoe)
                return false;

            return await Spells.DoomSpike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SonicThrust()
        {
            if (!DragoonSettings.Instance.Aoe)
                return false;

            if (ActionManager.LastSpell != Spells.DoomSpike)
                return false;

            return await Spells.SonicThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CoethanTorment()
        {
            if (!DragoonSettings.Instance.Aoe)
                return false;

            if (ActionManager.LastSpell != Spells.SonicThrust)
                return false;

            return await Spells.CoerthanTorment.Cast(Core.Me.CurrentTarget);
        }
    }
}
