using ff14bot.Enums;
using Magitek.Utilities.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class Dancer
    {
        public static bool OnGcd => Spells.Cascade.Cooldown > Globals.AnimationLockTimespan;
        public static OgcdManager DancerGCD = new OgcdManager(ClassJobType.Dancer, Spells.Cascade);

    }
}