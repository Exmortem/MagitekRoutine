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

            return await Spells.RuinIIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SlipstreamPvp()
        {

            if (!Spells.SlipstreamPvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.UsedSlipstream)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.SlipstreamPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MountainBusterPvp()
        {

            if (!Spells.MountainBusterPvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.UsedMountainBuster)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
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

            return await Spells.FesterPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RadiantAegisPvp()
        {

            if (!Spells.RadiantAegisPvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.UsedRadiantAegis)
                return false;

            if (Core.Me.CurrentHealthPercent > SummonerSettings.Instance.UseRadiantAegisHealthPercent)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.RadiantAegisPvp.Cast(Core.Me);
        }


        public static async Task<bool> CrimsonStrikePvp()
        {

            if (!Spells.CrimsonStrikePvp.CanCast())
                return false;

            if (!SummonerSettings.Instance.UsedCrimsonStrike)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
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

    }
}
