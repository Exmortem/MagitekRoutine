using System;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Summoner
{
    internal static class Aoe
    {

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

            var inFireBird = Spells.SummonPhoenix.Cooldown <= TimeSpan.FromSeconds(15) && Spells.SummonPhoenix.Cooldown > TimeSpan.Zero;

            if (!inFireBird && Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3) return false;

            if (inFireBird && ActionResourceManager.Summoner.TranceTimer < 2) return false;

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