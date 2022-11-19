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
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BlastChargePvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            return await Spells.BlastChargePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HeatBlast.CanCast())
                return false;

            if (!Core.Me.HasAura(Auras.Overheated))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            return await Spells.HeatBlastPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WildFire()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!MachinistSettings.Instance.Pvp_Wildfire)
                return false;

            if (!Spells.WildfirePvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            return await Spells.WildfirePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Scattergun()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!MachinistSettings.Instance.Pvp_Scattergun)
                return false;

            if (!Spells.ScattergunPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 12)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) < 12) < 1)
                return false;

            return await Spells.ScattergunPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Analysis()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

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
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.DrillPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            return await Spells.DrillPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BioBlaster()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BioblasterPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 12)
                return false;

            return await Spells.BioblasterPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AirAnchor()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.AirAnchorPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            return await Spells.AirAnchorPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ChainSaw()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ChainSawPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            return await Spells.ChainSawPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BishopAutoturret()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!MachinistSettings.Instance.Pvp_BishopAutoturret)
                return false;

            if (!Spells.BishopAutoturretPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me.CurrentTarget) < 5 ) < MachinistSettings.Instance.Pvp_BishopAutoturretNumberOfEnemy)
                return false;

            return await Spells.BishopAutoturretPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MarksmansSpite()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!MachinistSettings.Instance.Pvp_UseMarksmansSpite)
                return false;

            if (!Spells.MarksmansSpitePvp.CanCast())
                return false;

            if(Core.Me.CurrentTarget.CurrentHealthPercent > MachinistSettings.Instance.Pvp_UseMarksmansSpiteHealthPercent)
                return false;

            return await Spells.MarksmansSpitePvp.Cast(Core.Me.CurrentTarget);
        }

        
    }
}
