using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Machinist
{
    internal static class Pvp
    {
        public static async Task<bool> BlastedCharge()
        {
            if (!Spells.BlastChargePvp.CanCast())
                return false;

            return await Spells.BlastChargePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {
            if (!Spells.BlastChargePvp.CanCast())
                return false;

            return await Spells.HeatBlastPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WildFire()
        {
            if(!Spells.WildfirePvp.CanCast())
                return false;

            return await Spells.WildfirePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Analysis()
        {
            if (!Spells.AnalysisPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Analysis))
                return false;

            if (!MachinistSettings.Instance.Pvp_UsedAnalysisOnDrill && Core.Me.HasAura(Auras.DrillPrimed))
                return false;

            if (!MachinistSettings.Instance.Pvp_UsedAnalysisOnBio && Core.Me.HasAura(Auras.BioPrimed))
                return false;

            if (!MachinistSettings.Instance.Pvp_UsedAnalysisOnAA && Core.Me.HasAura(Auras.AirAnchorPrimed))
                return false;

            if (!MachinistSettings.Instance.Pvp_UsedAnalysisOnChainSaw && Core.Me.HasAura(Auras.ChainSawPrimed))
                return false;

            return await Spells.AnalysisPvp.Cast(Core.Me);
        }

        public static async Task<bool> Drill()
        {
            if (!Spells.DrillPvp.CanCast())
                return false;

            return await Spells.DrillPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BioBlaster()
        {
            if (!Spells.BioblasterPvp.CanCast())
                return false;

            return await Spells.BioblasterPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AirAnchor()
        {
            if (!Spells.AirAnchorPvp.CanCast())
                return false;

            return await Spells.AirAnchorPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ChainSaw()
        {
            if (!Spells.ChainSawPvp.CanCast())
                return false;

            return await Spells.ChainSawPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BishopAutoturret()
        {
            if (!Spells.BishopAutoturretPvp.CanCast())
                return false;

            return await Spells.BishopAutoturretPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MarksmansSpite()
        {
            if (!MachinistSettings.Instance.Pvp_UseMarksmansSpite)
                return false;

            if (!Spells.MarksmansSpitePvp.CanCast())
                return false;

            return await Spells.MarksmansSpitePvp.Cast(Core.Me.CurrentTarget);
        }

        
    }
}
