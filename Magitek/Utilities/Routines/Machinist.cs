using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.Machinist;
using System;


namespace Magitek.Utilities.Routines
{
    internal static class Machinist
    {
        public static bool IsInWeaveingWindow => ActionResourceManager.Machinist.OverheatRemaining != TimeSpan.Zero
                                                ? Weaving.GetCurrentWeavingCounter() < 1 && HeatedSplitShot.Cooldown != TimeSpan.Zero
                                                : Weaving.GetCurrentWeavingCounter() < 2 && HeatedSplitShot.Cooldown != TimeSpan.Zero
                                                                            && HeatedSplitShot.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + 50 + MachinistSettings.Instance.UserLatencyOffset;


        /*
        // Export IsInWeaveingWindow in a method to help for debug if necessary
        public static bool IsInWeaveingWindow => IsInWeaveingWindowMethod();
        public static bool IsInWeaveingWindowMethod()
        {
            var heatedSplitShotCooldown = HeatedSplitShot.Cooldown;
            var heatedSplitShotCooldownTotalMilliseconds = HeatedSplitShot.Cooldown.TotalMilliseconds;
            var currentWeavingCounter = Weaving.GetCurrentWeavingCounter();

            
            //Logger.WriteInfo($@"heatedSplitShotCooldownTotalMilliseconds: {heatedSplitShotCooldownTotalMilliseconds}");
            //Logger.WriteInfo($@"currentWeavingCounter: {currentWeavingCounter}");

            return ActionResourceManager.Machinist.OverheatRemaining != TimeSpan.Zero
                                                ? currentWeavingCounter < 1 && heatedSplitShotCooldown != TimeSpan.Zero
                                                : currentWeavingCounter < 2 && heatedSplitShotCooldown != TimeSpan.Zero
                                                                            && heatedSplitShotCooldownTotalMilliseconds > AnimationLock + 50 + MachinistSettings.Instance.UserLatencyOffset;
        }
        */

        //skill upgrades
        public static SpellData HeatedSplitShot => Core.Me.ClassLevel < 54 //|| !ActionManager.HasSpell(Spells.HeatedSplitShot.Id)
                                                    ? Spells.SplitShot
                                                    : Spells.HeatedSplitShot;
        public static SpellData HeatedSlugShot => Core.Me.ClassLevel < 60 //|| !ActionManager.HasSpell(Spells.HeatedSlugShot.Id)
                                                    ? Spells.SlugShot
                                                    : Spells.HeatedSlugShot;

        public static SpellData HeatedCleanShot => Core.Me.ClassLevel < 64
                                                    ? Spells.CleanShot
                                                    : Spells.HeatedCleanShot;

        public static SpellData HotAirAnchor => Core.Me.ClassLevel < 76
                                                    ? Spells.HotShot
                                                    : Spells.AirAnchor;

        public static SpellData RookQueenPet => Core.Me.ClassLevel < 80
                                                    ? Spells.RookAutoturret
                                                    : Spells.AutomationQueen;

        public static SpellData RookQueenOverdrive => Core.Me.ClassLevel < 80
                                                    ? Spells.RookOverdrive
                                                    : Spells.QueenOverdrive;

        public static SpellData Scattergun => Core.Me.ClassLevel < 82
                                                    ? Spells.SpreadShot
                                                    : Spells.Scattergun;

    }
}
