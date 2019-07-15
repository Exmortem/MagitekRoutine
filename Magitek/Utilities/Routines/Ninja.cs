using System;
using ff14bot;
using ff14bot.Managers;

namespace Magitek.Utilities.Routines
{
    internal static class Ninja
    {
        public static bool OnGcd => Spells.SpinningEdge.Cooldown > TimeSpan.FromMilliseconds(400);
        public static bool CanCastNinjutsu => ActionManager.CanCast(Spells.Ninjutsu, null);
    }
}
