using Magitek.Models.BlueMage;
using System;

namespace Magitek.Utilities.Routines
{
    internal static class BlueMage
    {
        public static bool OnGcd => Spells.Hakaze.Cooldown > TimeSpan.FromMilliseconds(BlueMageSettings.Instance.UseOffGCDAbilitiesWithMoreThanXMSLeft);

    }
}
