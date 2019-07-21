using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Logic.Machinist
{
    internal static class SingleTarget
    {
        public static async Task<bool> SplitShot()
        {
            return await Spells.SplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SlugShot()
        {
            if (ActionManager.LastSpell != Spells.SplitShot)
                return false;

            return await Spells.SlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CleanShot()
        {
            if (ActionManager.LastSpell != Spells.SlugShot)
                return false;

            return await Spells.CleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HotShot()
        {
            return await Spells.HotShot.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> GaussRound()
        {
            return await Spells.GaussRound.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {
            return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if(ActionManager.CanCast(Spells.Reassemble, Core.Me))
                if (await Spells.Reassemble.CastAura(Core.Me, Auras.Reassembled))
                    return await Spells.Drill.Cast(Core.Me.CurrentTarget);

            if(Casting.LastSpell == Spells.Reassemble)
                return await Spells.Drill.Cast(Core.Me.CurrentTarget);

            if(Spells.Reassemble.Cooldown.TotalMilliseconds > 20000 && Spells.AirAnchor.Cooldown.TotalMilliseconds < Spells.Reassemble.Cooldown.TotalMilliseconds)
                return await Spells.Drill.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> AirAnchor()
        {
            if(ActionManager.CanCast(Spells.Reassemble, Core.Me))
                if (await Spells.Reassemble.CastAura(Core.Me, Auras.Reassembled))
                    return await Spells.AirAnchor.Cast(Core.Me.CurrentTarget);

            if(Casting.LastSpell == Spells.Reassemble)
                return await Spells.AirAnchor.Cast(Core.Me.CurrentTarget);

            if(Spells.Reassemble.Cooldown.TotalMilliseconds > 40000 && Spells.Drill.Cooldown.TotalMilliseconds < Spells.Reassemble.Cooldown.TotalMilliseconds)
                return await Spells.AirAnchor.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}
