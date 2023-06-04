using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Reaper;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Reaper
{
    internal static class Pvp
    {
        public static async Task<bool> SlicePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SlicePvp.CanCast())
                return false;

            return await Spells.SlicePvp.CastPvpCombo(Spells.InfernalSlicePvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> WaxingSlicePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.WaxingSlicePvp.CanCast())
                return false;

            return await Spells.WaxingSlicePvp.CastPvpCombo(Spells.InfernalSlicePvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> InfernalSlicePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.InfernalSlicePvp.CanCast())
                return false;

            return await Spells.InfernalSlicePvp.CastPvpCombo(Spells.InfernalSlicePvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> GrimSwathePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.GrimSwathePvp.CanCast())
                return false;

            if(!ReaperSettings.Instance.Pvp_GrimSwathe)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 8)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.GrimSwathePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> LemureSlicePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.LemureSlicePvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_LemureSlice)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 8)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.LemureSlicePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ArcaneCrestPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ArcaneCrestPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_ArcaneCrest)
                return false;

            if (Group.CastableAlliesWithin15.Count(x => x.IsValid && x.IsAlive) < ReaperSettings.Instance.Pvp_ArcaneCrestNumberOfAllies)
                return false;

            return await Spells.ArcaneCrestPvp.Cast(Core.Me);
        }

        public static async Task<bool> DeathWarrantPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.DeathWarrantPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_DeathWarrant)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.DeathWarrantPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HarvestMoonPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HarvestMoonPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_HarvestMoon)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (Core.Me.CurrentTarget.CurrentHealthPercent > 50 && Core.Me.HasAura(Auras.PvpSoulsow, true, 2000))
                return false;

            return await Spells.HarvestMoonPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SoulSlicePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SoulSlicePvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_SoulSlice)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.SoulSlicePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PlentifulHarvestPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.PlentifulHarvestPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_PlentifulHarvest)
                return false;

            if (!Core.Me.HasAura(Auras.PvpImmortalSacrifice))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 15)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.PlentifulHarvestPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GuillotinePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.GuillotinePvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_Guillotine)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 8)
                return false;

            return await Spells.GuillotinePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> VoidReapingPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.VoidReapingPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_VoidReapingNCrossReaping)
                return false;

            if (!Core.Me.HasAura(Auras.PvpEnshrouded) || !Core.Me.HasAura(Auras.PvpEnshrouded, true, 3000))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (EnshroudedCount == 1)
                return false;

            if (await Spells.VoidReapingPvp.Cast(Core.Me.CurrentTarget))
            {
                EnshroudedCount -= 1;
                return true;
            }

            return false;
        }

        public static async Task<bool> CrossReapingePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.CrossReapingePvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_VoidReapingNCrossReaping)
                return false;

            if (!Core.Me.HasAura(Auras.PvpEnshrouded) || !Core.Me.HasAura(Auras.PvpEnshrouded,true,3000))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (EnshroudedCount == 1)
                return false;

            if (await Spells.CrossReapingePvp.Cast(Core.Me.CurrentTarget))
            {
                EnshroudedCount -= 1;
                return true;
            }

            return false;
        }

        public static async Task<bool> CommunioPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.CommunioPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_Communio)
                return false;

            if (!Core.Me.HasAura(Auras.PvpEnshrouded))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (EnshroudedCount != 1 && Core.Me.HasAura(Auras.PvpEnshrouded, true, 3000))
                return false;

            return await Spells.CommunioPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TenebraeLemurumPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.TenebraeLemurumPvp.CanCast())
                return false;

            if (!ReaperSettings.Instance.Pvp_TenebraeLemurum)
                return false;

            if (Core.Me.HasAura(Auras.PvpEnshrouded))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if(await Spells.TenebraeLemurumPvp.Cast(Core.Me.CurrentTarget))
            {
                EnshroudedCount = 5;
                return true;
            }

            return false;
        }

        public static int EnshroudedCount = 5;
    }
}
