using ff14bot;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Paladin
{
    internal static class Pvp
    {

        public static async Task<bool> FastBladePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.FastBladePvp.CanCast())
                return false;

            return await Spells.FastBladePvp.CastPvpCombo(Spells.RoyalAuthorityPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> RiotBladePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RiotBladePvp.CanCast())
                return false;

            return await Spells.RiotBladePvp.CastPvpCombo(Spells.RoyalAuthorityPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> RoyalAuthorityPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RoyalAuthorityPvp.CanCast())
                return false;

            return await Spells.RoyalAuthorityPvp.CastPvpCombo(Spells.RoyalAuthorityPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> ConfiteorPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ConfiteorPvp.CanCast())
                return false;

            if (!PaladinSettings.Instance.Pvp_Confiteor)
                return false;

            return await Spells.ConfiteorPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AtonementPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.AtonementPvp.CanCast())
                return false;

            if (!PaladinSettings.Instance.Pvp_Atonement)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (!Core.Me.HasAura(Auras.PvpSwordOath))
                return false;

            return await Spells.AtonementPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShieldBashPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ShieldBashPvp.CanCast())
                return false;

            if (!PaladinSettings.Instance.Pvp_ShieldBash)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.ShieldBashPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HolySheltronPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HolySheltronPvp.CanCast())
                return false;

            if (!PaladinSettings.Instance.Pvp_HolySheltron)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.HolySheltronPvp.Cast(Core.Me);
        }

        public static async Task<bool> IntervenePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.IntervenePvp.CanCast())
                return false;

            if (!PaladinSettings.Instance.Pvp_Intervene)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (PaladinSettings.Instance.Pvp_SafeIntervene && Core.Me.CurrentTarget.Distance(Core.Me) > 3)
                return false;

            return await Spells.IntervenePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhalanxPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.PhalanxPvp.CanCast())
                return false;

            if (!PaladinSettings.Instance.Pvp_Phalanx)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.PhalanxPvp.Cast(Core.Me);
        }

        public static async Task<bool> BladeofFaithPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BladeofFaithPvp.CanCast())
                return false;

            return await Spells.BladeofFaithPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BladeofTruthPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BladeofTruthPvp.CanCast())
                return false;

            return await Spells.BladeofTruthPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BladeofValorPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BladeofValorPvp.CanCast())
                return false;

            return await Spells.BladeofValorPvp.Cast(Core.Me.CurrentTarget);
        }
    }
}