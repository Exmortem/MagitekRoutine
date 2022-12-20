using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using System.Linq;
using System.Threading.Tasks;
using SummonerRoutine = Magitek.Utilities.Routines.Summoner;

namespace Magitek.Logic.Summoner
{
    internal static class Pvp
    {
        public static async Task<bool> RuinIIIPvp()
        {

            if(!Spells.RuinIIIPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if(Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.RuinIIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SlipstreamPvp()
        {

            if (!Spells.SlipstreamPvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.Pvp_UsedSlipstream)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.SlipstreamPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MountainBusterPvp()
        {

            if (!Spells.MountainBusterPvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.Pvp_UsedMountainBuster)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.MountainBusterPvp.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> FesterPvp()
        {

            if (!Spells.FesterPvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.CurrentHealthPercent > 50)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.FesterPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RadiantAegisPvp()
        {

            if (!Spells.RadiantAegisPvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.Pvp_UsedRadiantAegis)
                return false;

            if (Core.Me.CurrentHealthPercent > SummonerSettings.Instance.Pvp_UseRadiantAegisHealthPercent)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.RadiantAegisPvp.Cast(Core.Me);
        }


        public static async Task<bool> CrimsonStrikePvp()
        {

            if (!Spells.CrimsonStrikePvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.Pvp_UsedCrimsonStrike)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.CrimsonStrikePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnkindleBahamutPvp()
        {

            if (!Spells.EnkindleBahamutPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.EnkindleBahamutPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnkindlePhoenixPvp()
        {

            if (!Spells.EnkindlePhoenixPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.EnkindlePhoenixPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SummonBahamutPvp()
        {

            if (!Spells.SummonBahamutPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!SummonerSettings.Instance.Pvp_Summon || !SummonerSettings.Instance.Pvp_SummonBahamut)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 30)
                return false;

            return await Spells.SummonBahamutPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SummonPhoenixPvp()
        {

            if (!Spells.SummonPhoenixPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!SummonerSettings.Instance.Pvp_Summon || !SummonerSettings.Instance.Pvp_SummonPhoenix)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 30)
                return false;

            return await Spells.SummonPhoenixPvp.Cast(Core.Me.CurrentTarget);
        }

    }
}
