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
            return await Spells.CascadePvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fountain()
        {
            if (!Spells.FountainPvp.CanCast())
                return false;

            return await Spells.FountainPvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> ReverseCascade()
        {
            if (!Spells.ReverseCascadePvp.CanCast())
                return false;

            return await Spells.ReverseCascadePvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SaberDance()
        {
            if (!Spells.SaberDancePvp.CanCast())
                return false;

            return await Spells.SaberDancePvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> FountainFall()
        {
            if (!Spells.FountainFallPvp.CanCast())
                return false;

            return await Spells.FountainFallPvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
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

        public static async Task<bool> HoningDance()
        {
            if (!DancerSettings.Instance.Pvp_UseHoningDance)
                return false;
            
            if (!Spells.HoningDancePvp.CanCast())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < DancerSettings.Instance.Pvp_HoningDanceMinimumEnemies)
                return false;

            return await Spells.HoningDancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Contradance()
        {
            if (!DancerSettings.Instance.Pvp_UseContradance)
                return false;

            if (!Spells.ContradancePvp.CanCast())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 15 + x.CombatReach) < DancerSettings.Instance.Pvp_ContradanceMinimumEnemies)
                return false;

            return await Spells.ContradancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CuringWaltz()
        {
            if (!DancerSettings.Instance.Pvp_UseCuringWaltz)
                return false;
            
            if (!Spells.CuringWaltzPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            var cureTargets = Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.Pvp_CuringWaltzHP && x.Distance(Core.Me) < 5);

            if (Core.Me.HasAura(Auras.ClosedPosition))
            {
                var DancePartner = Group.CastableParty.FirstOrDefault(x => x.HasMyAura(Auras.DancePartner));

                if (DancePartner != null)
                    cureTargets += Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.Pvp_CuringWaltzHP && x.Distance(DancePartner) < 5);
            }

            if (cureTargets < 1)
                return false;

            return await Spells.CuringWaltzPvp.Cast(Core.Me.CurrentTarget);
        }
    }
}
