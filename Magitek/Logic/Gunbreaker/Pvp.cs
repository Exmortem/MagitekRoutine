using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;


namespace Magitek.Logic.Gunbreaker
{
    internal static class Pvp
    {
        public static async Task<bool> KeenEdgePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.KeenEdgePvp.CanCast())
                return false;

            return await Spells.KeenEdgePvp.CastPvpCombo(Spells.SolidBarrelPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> BrutalShelPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BrutalShelPvp.CanCast())
                return false;

            return await Spells.BrutalShelPvp.CastPvpCombo(Spells.SolidBarrelPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SolidBarrelPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SolidBarrelPvp.CanCast())
                return false;

            return await Spells.SolidBarrelPvp.CastPvpCombo(Spells.SolidBarrelPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> GnashingFangPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.GnashingFangPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_GnashingFangCombo)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.GnashingFangPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SavageClawPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SavageClawPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_GnashingFangCombo)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.SavageClawPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WickedTalonPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.WickedTalonPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_GnashingFangCombo)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.WickedTalonPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ContinuationPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ContinuationPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_Continuation)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.ContinuationPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DoubleDownPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.DoubleDownPvp.CanCast())
                return false;

            if(!GunbreakerSettings.Instance.Pvp_DoubleDown)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.DoubleDownPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BurstStrikePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BurstStrikePvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_BurstStrike)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.BurstStrikePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RoughDividePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RoughDividePvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_BurstStrike)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            if (Core.Me.HasAura(Auras.PvpNoMercy))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (GunbreakerSettings.Instance.Pvp_SafeRoughDivide && Core.Me.CurrentTarget.Distance(Core.Me) > 3)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.RoughDividePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DrawandJunctionPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.DrawandJunctionPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_DrawandJunction)
                return false;

            if(!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (!GunbreakerSettings.Instance.Pvp_BlastingZone && Core.Me.CurrentTarget.IsDps())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_Nebula && Core.Me.CurrentTarget.IsTank())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_Aurora && Core.Me.CurrentTarget.IsHealer())
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.DrawandJunctionPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlastingZonePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BlastingZonePvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_BlastingZone)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.BlastingZonePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> NebulaPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.NebulaPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_Nebula)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.NebulaPvp.Cast(Core.Me);
        }

        public static async Task<bool> AuroraPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.AuroraPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_Aurora)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            return await Spells.AuroraPvp.Cast(Core.Me);
        }

        public static async Task<bool> RelentlessRushPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RelentlessRushPvp.CanCast())
                return false;

            if (!GunbreakerSettings.Instance.Pvp_RelentlessRush)
                return false;

            if (Core.Me.HasAura(Auras.PvpRelentlessRush))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < GunbreakerSettings.Instance.Pvp_RelentlessRushEnemyCount)
                return false;

            return await Spells.RelentlessRushPvp.Cast(Core.Me);
        }
    }
}
