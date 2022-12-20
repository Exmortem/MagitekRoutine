using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class Pvp
    {
        public static async Task<bool> SpinningEdgePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SpinningEdgePvp.CanCast())
                return false;

            return await Spells.SpinningEdgePvp.CastPvpCombo(Spells.AeolianEdgePvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> GustSlashPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.GustSlashPvp.CanCast())
                return false;

            return await Spells.GustSlashPvp.CastPvpCombo(Spells.AeolianEdgePvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> AeolianEdgePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.AeolianEdgePvp.CanCast())
                return false;

            return await Spells.AeolianEdgePvp.CastPvpCombo(Spells.AeolianEdgePvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> AssassinatePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.AssassinatePvp.CanCast())
                return false;

            if(!NinjaSettings.Instance.Pvp_Assassinate)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.AssassinatePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MugPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Spells.MugPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_Mug)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.MugPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BunshinPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Spells.BunshinPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_Bunshin)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.BunshinPvp.Cast(Core.Me);
        }

        public static async Task<bool> FumaShurikenPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Spells.FumaShurikenPvp.CanCast() || Spells.FumaShurikenPvp.IsKnownAndReady())
                return false;

            if (!NinjaSettings.Instance.Pvp_FumaShuriken)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.FumaShurikenPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShukuchiPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Spells.ShukuchiPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_Shukuchi)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.ShukuchiPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SeitonTenchuPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Spells.SeitonTenchuPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_SeitonTenchu)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (Core.Me.CurrentTarget.CurrentHealthPercent > NinjaSettings.Instance.Pvp_SeitonTenchuHealthPercent)
                return false;

            return await Spells.SeitonTenchuPvp.Cast(Core.Me.CurrentTarget);
        }

        #region Three Mudra

        public static async Task<bool> ThreeMudraPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden) || Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.ThreeMudraPvp.CanCast())
                return false;

            if (NinjaSettings.Instance.Pvp_DoNotUseThreeMudra)
                return false;

            return await Spells.ThreeMudraPvp.Cast(Core.Me);
        }

        public static async Task<bool> HyoshoRanryuPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.HyoshoRanryuPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_HyoshoRanryu)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.HyoshoRanryuPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GokaMekkyakuPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.GokaMekkyakuPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_GokaMekkyaku)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me.CurrentTarget) <= 5 + x.CombatReach) < NinjaSettings.Instance.Pvp_GokaMekkyakuMinEnemies)
                return false;

            return await Spells.GokaMekkyakuPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DotonPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.DotonPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_Doton)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < NinjaSettings.Instance.Pvp_GokaMekkyakuMinEnemies)
                return false;

            return await Spells.DotonPvp.Cast(Core.Me);
        }

        public static async Task<bool> HutonPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.HutonPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_Huton)
                return false;

            if(Core.Me.CurrentHealthPercent > NinjaSettings.Instance.Pvp_HutonHealthPercent)
                 return false;

            return await Spells.HutonPvp.Cast(Core.Me);
        }

        public static async Task<bool> MeisuiPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.MeisuiPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_Meisui)
                return false;

            if (Core.Me.CurrentHealthPercent > NinjaSettings.Instance.Pvp_MeisuiHealthPercent)
                return false;

            return await Spells.MeisuiPvp.Cast(Core.Me);
        }

        public static async Task<bool> ForkedRaijuPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.PvpThreeMudra))
                return false;

            if (!Spells.ForkedRaijuPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_ForkedRaiju)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.ForkedRaijuPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FleetingRaijuPvp()
        {
            if (Core.Me.HasAura(Auras.Guard) || Core.Me.HasAura(Auras.PvpHidden))
                return false;

            if (!Core.Me.HasAura(Auras.FleetingRaijuReady))
                return false;

            if (!Spells.FleetingRaijuPvp.CanCast())
                return false;

            if (!NinjaSettings.Instance.Pvp_FleetingRaiju)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.FleetingRaijuPvp.Cast(Core.Me.CurrentTarget);
        }

        #endregion

    }
}
