using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;

namespace Magitek.Logic.Summoner
{
    internal static class Aoe
    {
        public static async Task<bool> Bane()
        {
            if (Core.Me.ClassLevel < 30) return false;

            if (!Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, SummonerSettings.Instance.BaneSecondsOnDots * 1000)) return false;

            if (!Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, SummonerSettings.Instance.BaneSecondsOnDots * 1000)) return false;

            var targets = Core.Me.CurrentTarget.EnemiesNearby(8).ToList();

            if (targets.Count < 2) return false;

            var targetsWithoutBio = targets.Where(x => !x.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, SummonerSettings.Instance.BaneSecondsOnDots * 1000)).ToList();
            var targetsWithoutMiasma = targets.Where(x => !x.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, SummonerSettings.Instance.BaneSecondsOnDots * 1000)).ToList();

            if (targetsWithoutBio.Count + targetsWithoutMiasma.Count < 1) return false;

            return await Spells.Bane.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnergySiphon()
        {
            if (Core.Me.ClassLevel < 35) return false;

            if (ActionResourceManager.Arcanist.Aetherflow > 0) return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3) return false;

            return await Spells.EnergySiphon.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Outburst()
        {
            if (Core.Me.ClassLevel < 40) return false;

            if (!Core.Me.HasAura(Auras.HellishConduit) && Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3) return false;

            if (Core.Me.HasAura(Auras.HellishConduit) && ActionResourceManager.Summoner.Timer.TotalMilliseconds < 250) return false;

            return await Spells.Outburst.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Painflare()
        {
            if (Core.Me.ClassLevel < 52) return false;

            if (ActionResourceManager.Arcanist.Aetherflow == 0) return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 2) return false;

            return await Spells.Painflare.Cast(Core.Me.CurrentTarget);
        }
    }
}