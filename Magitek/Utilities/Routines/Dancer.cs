using ff14bot.Enums;

namespace Magitek.Utilities.Routines
{
    internal static class Dancer
    {
        public static bool OnGcd => Spells.Cascade.Cooldown > Globals.AnimationLockTimespan;
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Dancer, Spells.Cascade);

    }
}