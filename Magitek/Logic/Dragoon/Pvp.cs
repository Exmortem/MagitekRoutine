using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using DragoonRoutine = Magitek.Utilities.Routines.Dragoon;

namespace Magitek.Logic.Dragoon
{
    internal static class Pvp
    {
        public static async Task<bool> RaidenThrustPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RaidenThrustPvp.CanCast())
                return false;

            return await Spells.RaidenThrustPvp.CastPvpCombo(Spells.WheelingThrustPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> FangandClawPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.FangandClawPvp.CanCast())
                return false;

            return await Spells.FangandClawPvp.CastPvpCombo(Spells.WheelingThrustPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> WheelingThrustPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.WheelingThrustPvp.CanCast())
                return false;

            return await Spells.WheelingThrustPvp.CastPvpCombo(Spells.WheelingThrustPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeavensThrustPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HeavensThrustPvp.CanCast())
                return false;

            if (!Core.Me.HasAura(Auras.PvpHeavensent))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.HeavensThrustPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ChaoticSpringPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ChaoticSpringPvp.CanCast())
                return false;

            if(!DragoonSettings.Instance.Pvp_ChaoticSpring)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.ChaoticSpringPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GeirskogulPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.GeirskogulPvp.CanCast())
                return false;

            if (!DragoonSettings.Instance.Pvp_Geirskogul)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 15)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.GeirskogulPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> NastrondPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.NastrondPvp.CanCast())
                return false;

            if (!DragoonSettings.Instance.Pvp_Geirskogul)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 15)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (Core.Me.CurrentTarget.CurrentHealthPercent > 50 && Core.Me.HasAura(Auras.PvpLifeoftheDragon,true,2000))
                return false;

            return await Spells.NastrondPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HighJumpPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HighJumpPvp.CanCast())
                return false;

            if (!DragoonSettings.Instance.Pvp_HighJump)
                return false;

            if (DragoonSettings.Instance.Pvp_ElusiveJump && Spells.ElusiveJumpPvp.IsKnownAndReady(9000))
                return false;

            if (DragoonSettings.Instance.Pvp_ElusiveJump && Spells.WyrmwindThrustPvp.IsKnownAndReady())
                return false;

            if (Core.Me.HasAura(Auras.PvpFirstmindsFocus))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.HighJumpPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ElusiveJumpPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ElusiveJumpPvp.CanCast())
                return false;

            if (!DragoonSettings.Instance.Pvp_ElusiveJump)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.ElusiveJumpPvp.Cast(Core.Me);
        }

        public static async Task<bool> WyrmwindThrustPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.WyrmwindThrustPvp.CanCast())
                return false;

            if (!Core.Me.HasAura(Auras.PvpFirstmindsFocus))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.WyrmwindThrustPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HorridRoarPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HorridRoarPvp.CanCast())
                return false;

            if (!DragoonSettings.Instance.Pvp_HorridRoar)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.HorridRoarPvp.Cast(Core.Me);
        }

        public static async Task<bool> SkyHighPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SkyHighPvp.CanCast())
                return false;

            if (!DragoonSettings.Instance.Pvp_SkyHigh)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.SkyHighPvp.Cast(Core.Me);
        }
    }
}
