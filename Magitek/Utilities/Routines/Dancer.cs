namespace Magitek.Utilities.Routines
{
    internal static class Dancer
    {
        public static bool OnGcd => Weaving.GetCurrentWeavingCounter() < 2 && Spells.Cascade.Cooldown > Globals.AnimationLockTimespan;

    }
}