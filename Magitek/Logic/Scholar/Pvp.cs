using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using ScholarRoutine = Magitek.Utilities.Routines.Scholar;

namespace Magitek.Logic.Scholar
{
    internal static class Pvp
    {
        public static async Task<bool> BroilIVPvp()
        {

            if (!Spells.BroilIVPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.BroilIVPvp.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> BiolysisPvp()
        {

            if (!Spells.BiolysisPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_Biolysis)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.BiolysisPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MummificationPvp()
        {

            if (!Spells.MummificationPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_Mummification)
                return false;

            if (ScholarRoutine.EnemiesInCone < 1)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.MummificationPvp.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> AdloquiumPvp()
        {

            if (!Spells.AdloquiumPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_Adloquium)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (ScholarSettings.Instance.Pvp_HealSelfOnly)
            {
                if (Core.Me.HasAura(Auras.PvpCatalyze) || Core.Me.CurrentHealthPercent > ScholarSettings.Instance.Pvp_AdloquiumHealthPercent)
                    return false;

                return await Spells.AdloquiumPvp.Heal(Core.Me);
            }

            var Target = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 && r.CurrentHealthPercent <= ScholarSettings.Instance.Pvp_AdloquiumHealthPercent && !r.HasAura(Auras.PvpCatalyze));

            if (Target == null)
                if (Core.Me.CurrentHealthPercent <= ScholarSettings.Instance.Pvp_AdloquiumHealthPercent && !Core.Me.HasAura(Auras.PvpCatalyze))
                    return await Spells.AdloquiumPvp.Heal(Core.Me);
                else
                    return false;

            return await Spells.AdloquiumPvp.Heal(Target);

        }

        public static async Task<bool> DeploymentTacticsPvp()
        {

            if (!Spells.DeploymentTacticsPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_DeploymentTacticsOnSelf)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.HasAura(Auras.PvpCatalyze))
                 return false;

            if (!Core.Me.HasAura(Auras.PvpCatalyze, true, 14000))
                return false;

            return await Spells.DeploymentTacticsPvp.Cast(Core.Me);
        }

        public static async Task<bool> DeploymentTacticsAlliesPvp()
        {

            if (!Spells.DeploymentTacticsPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_DeploymentTacticsOnAllies)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            var deploymentTacticsTarget = Group.CastableAlliesWithin30.FirstOrDefault(r =>
                r.HasAura(Auras.PvpCatalyze, true)
                && Group.CastableAlliesWithin30.Count(x => x.Distance(r) <= 15 + x.CombatReach) >= 2);

            if (deploymentTacticsTarget == null)
                return false;

            if (!deploymentTacticsTarget.HasAura(Auras.PvpBiolytic, true, 14000))
                return false;

            return await Spells.DeploymentTacticsPvp.Cast(deploymentTacticsTarget);

        }

        public static async Task<bool> DeploymentTacticsEnemyPvp()
        {

            if (!Spells.DeploymentTacticsPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_DeploymentTacticsOnEnemy)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.PvpBiolytic))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.PvpBiolytic, true, 14000))
                return false;

            return await Spells.DeploymentTacticsPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ExpedientPvp()
        {

            if (!Spells.ExpedientPvp.CanCast())
                return false;

            if (!ScholarSettings.Instance.Pvp_Expedient)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.ExpedientPvp.Cast(Core.Me);
        }

        public static async Task<bool> SummonSeraphPvp()
        {
            if (!Spells.SummonSeraphPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Group.CastableAlliesWithin30.Count(x => x.IsValid && x.IsAlive) < ScholarSettings.Instance.Pvp_SummonSeraphNearbyAllies)
                return false;

            return await Spells.SummonSeraphPvp.Cast(Core.Me);
        }
    }
}