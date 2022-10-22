using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using System;
using System.Linq;



namespace Magitek.Utilities.Routines
{

    internal static class BlackMage
    {
        public static int AoeEnemies5Yards;
        public static int AoeEnemies30Yards;
        public static void RefreshVars()
        {
            AoeEnemies5Yards = Combat.Enemies.Count(x => x.WithinSpellRange(5) && x.IsTargetable && x.IsValid && !x.HasAnyAura(Auras.Invincibility) && x.NotInvulnerable());
            AoeEnemies30Yards = Combat.Enemies.Count(x => x.WithinSpellRange(30) && x.IsTargetable && x.IsValid && !x.HasAnyAura(Auras.Invincibility) && x.NotInvulnerable());
        }
        public static bool NeedToInterruptCast()
        {
            if (Casting.SpellTarget?.CurrentHealth == 0)
            {
                {
                    Logger.Error($"Stopped {Casting.CastingSpell.LocalizedName}: because HE'S DEAD, JIM!");
                }
                return true;
            }
            return false;
        }

        public static readonly uint Ether = 4555;
        public static readonly uint HiEther = 4556;
        public static readonly uint XEther = 4558;
        public static readonly uint MegaEther = 13638;
        public static readonly uint SuperEther = 23168;
    }
}
