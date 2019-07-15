using ff14bot.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class Machinist
    {
        public static bool OnGcd => Spells.SplitShot.Cooldown.TotalMilliseconds > 300;

        public static bool Overheated => ActionResourceManager.Machinist.Heat == 100 && ActionResourceManager.Machinist.OverheatRemaining.TotalMilliseconds > 0;
    }
}
