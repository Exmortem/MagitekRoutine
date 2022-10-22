using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Dancer
{
    internal static class Pvp
    {
        public static async Task<bool> Cascade()
        {
            if (!Spells.CascadePvp.CanCast())
                return false;

            if(Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.CascadePvp.Cast(Core.Me.CurrentTarget); ;
        }

        public static async Task<bool> Fountain()
        {

            if (!Spells.FountainPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.FountainPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> StarfallDance()
        {

            if (!Spells.StarfallDancePvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.StarfallDancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FanDance()
        {

            if (!Spells.FanDancePvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.FanDancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CuringWaltzPvp()
        {
            if (!Spells.CuringWaltzPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!DancerSettings.Instance.UseCuringWaltzPvp)
                return false;

            var cureTargets = Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.CuringWaltzHPPvp && x.Distance(Core.Me) < 5);

            if (Core.Me.HasAura(Auras.ClosedPosition))
            {
                var DancePartner = Group.CastableParty.FirstOrDefault(x => x.HasMyAura(Auras.DancePartner));

                if (DancePartner != null)
                    cureTargets += Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.CuringWaltzHPPvp && x.Distance(DancePartner) < 5);
            }

            if (cureTargets < 1)
                return false;

            return await Spells.CuringWaltzPvp.Cast(Core.Me.CurrentTarget);
        }
    }
}
