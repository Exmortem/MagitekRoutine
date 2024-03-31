using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Utilities;
using Magitek.ViewModels;
using System.Collections.Generic;
using System.Linq;


namespace Magitek.Extensions
{
    internal static class PlayerExtensions
    {
        public static bool HasAetherflow(this LocalPlayer me)
        {
            return ActionResourceManager.Scholar.Aetherflow > 0;
        }

        public static bool HasDarkArts(this LocalPlayer me)
        {
            return MagitekActionResourceManager.DarkKnight.DarkArts;
        }

        public static bool OnPvpMap(this LocalPlayer player)
        {
            if (!WorldManager.InPvP)
            {
                BaseSettings.Instance.InPvp = false;
                return false;
            }

            BaseSettings.Instance.InPvp = true;
            return true;
        }

        private static readonly HashSet<ushort> PvpMaps = new HashSet<ushort>()
        {
            149,
            175,
            184,
            186,
            250,
            336,
            337,
            352,
            376,
            422,
            431,
            502,
            506,
            518,
            525,
            526,
            527,
            528,
            537,
            538,
            539,
            540,
            541,
            542,
            543,
            544,
            545,
            546,
            547,
            548,
            549,
            550,
            551,
            552,
            554
        };

        public static int EnemiesInCone(this LocalPlayer player, float maxdistance)
        {
            return Combat.Enemies.Count(r => r.Distance(Core.Me) <= maxdistance + r.CombatReach && r.RadiansFromPlayerHeading() < 0.9599f);
        }
    }
}
