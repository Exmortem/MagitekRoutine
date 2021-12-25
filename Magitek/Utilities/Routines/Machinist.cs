using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
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
        public static SpellData HeatedSplitShot => Core.Me.ClassLevel < 54
                                                    ? Spells.SplitShot
                                                    : Spells.HeatedSplitShot;
        public static SpellData HeatedSlugShot => Core.Me.ClassLevel < 60
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

        public static bool CheckCurrentDamageIncrease(int neededDmgIncrease)
        {
            double dmgIncrease = 1;

            //From PLD
            //From DRK
            //From GNB
            //From WAR
            //From WHM
            //From BLM
            //From SAM
            //From MCH
            //From BRD
            if (Core.Me.HasAura(Auras.RadiantFinale))
                dmgIncrease *= 1.06;
            if (Core.Me.HasAura(Auras.BattleVoice))
                dmgIncrease *= 1.01;
            if (Core.Me.HasAura(Auras.TheWanderersMinuet))
                dmgIncrease *= 1.02;
            if (Core.Me.HasAura(Auras.MagesBallad))
                dmgIncrease *= 1.01;
            if (Core.Me.HasAura(Auras.ArmysPaeon))
                dmgIncrease *= 1.01;

            //From DNC
            if (Core.Me.HasAura(Auras.Devilment))
                dmgIncrease *= 1.01;
            if (Core.Me.HasAura(Auras.TechnicalFinish))
                dmgIncrease *= 1.06;
            if (Core.Me.HasAura(Auras.StandardFinish))
                dmgIncrease *= 1.06;

            //From RDM
            if (Core.Me.HasAura(Auras.Embolden))
                dmgIncrease *= 1.05;

            //From SMN
            if (Core.Me.HasAura(Auras.SearingLight))
                dmgIncrease *= 1.03;

            //From MNK
            if (Core.Me.HasAura(Auras.Brotherhood))
                dmgIncrease *= 1.05;

            //From NIN
            if (Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
                dmgIncrease *= 1.1;

            //From DRG
            if (Core.Me.HasAura(Auras.LeftEye))
                dmgIncrease *= 1.05;
            if (Core.Me.HasAura(Auras.BattleLitany))
                dmgIncrease *= 1.01;

            //From RPR
            if (Core.Me.HasAura(Auras.ArcaneCircle))
                dmgIncrease *= 1.03;

            //From SCH
            if (Core.Me.CurrentTarget.HasAura(Auras.ChainStratagem))
                dmgIncrease *= 1.01;

            //From SGE

            //From AST
            if (Core.Me.HasAura(Auras.Divination))
                dmgIncrease *= 1.06;
            if (Core.Me.HasAnyDpsCardAura())
                dmgIncrease *= 1.06;

            Logger.WriteInfo($@"Damage Increase: {dmgIncrease}");
            return dmgIncrease >= (1 + (double)neededDmgIncrease / 100);
        }

    }
}
