using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Reaper
    {
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;

        public static int LemureShroud = ActionResourceManager.Reaper.LemureShroud;
        public static int ShroudGauge = ActionResourceManager.Reaper.ShroudGauge;
        public static int SoulGauge = ActionResourceManager.Reaper.SoulGauge;
        public static int VoidShroud = ActionResourceManager.Reaper.VoidShroud;



        public static void RefreshVars()
        {

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(15);
            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

        }

        
    }
}