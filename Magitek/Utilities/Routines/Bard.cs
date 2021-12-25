using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Bard
    {
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;
        public static bool AlreadySnapped = false;


        public static SpellData LadonsBite => Core.Me.ClassLevel < 82
                                            ? Spells.QuickNock
                                            : Spells.Ladonsbite;

        public static SpellData Stormbite => Core.Me.ClassLevel < 64
                                            ? Spells.Windbite
                                            : Spells.Stormbite;

        public static SpellData CausticBite => Core.Me.ClassLevel < 64
                                            ? Spells.VenomousBite
                                            : Spells.CausticBite;

        public static SpellData BurstShot => Core.Me.ClassLevel < 76
                                            ? Spells.HeavyShot
                                            : Spells.BurstShot;


        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(15);
            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

            if (Casting.LastSpell == Spells.IronJaws && Core.Me.HasAura(Auras.RagingStrikes))
            {
                AlreadySnapped = true;
            }
            AlreadySnapped = AlreadySnapped && Core.Me.HasAura(Auras.RagingStrikes);


        }

        public static uint Windbite => (uint)(Core.Me.ClassLevel < 64 ? Auras.Windbite : Auras.StormBite);
        public static uint VenomousBite => (uint)(Core.Me.ClassLevel < 64 ? Auras.VenomousBite : Auras.CausticBite);

        public static List<uint> DotsList => Core.Me.ClassLevel >= 64 ?
            new List<uint>() { Auras.StormBite, Auras.CausticBite } :
            new List<uint>() { Auras.Windbite, Auras.VenomousBite };


        public static double TimeUntilNextPossibleDoTTick()
        {
            double nextTickInXms = 0;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None) {
                nextTickInXms = ActionResourceManager.Bard.Timer.TotalMilliseconds % 3000;
            }
            
            return nextTickInXms;
        }

        public static double CurrentDurationWanderersMinuet()
        {
            if (BardSettings.Instance.EndWanderersMinuetEarly)
                return ActionResourceManager.Bard.Timer.TotalMilliseconds - (1000 * BardSettings.Instance.EndWanderersMinuetEarlyWithXSecondsRemaining);

            return ActionResourceManager.Bard.Timer.TotalMilliseconds;
        }
        
        public static double NextTickUnderWanderersMinuet()
        {
            return CurrentDurationWanderersMinuet() - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick();
        }

        public static double CurrentDurationMagesBallad()
        {
            if (BardSettings.Instance.EndMagesBalladEarly)
                return ActionResourceManager.Bard.Timer.TotalMilliseconds - (1000 * BardSettings.Instance.EndMagesBalladEarlyWithXSecondsRemaining);

            return ActionResourceManager.Bard.Timer.TotalMilliseconds;
        }

        public static double NextTickUnderMagesBallad()
        {
            return CurrentDurationMagesBallad() - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick();
        }

        public static double CurrentDurationArmysPaeon()
        {
            if (BardSettings.Instance.EndArmysPaeonEarly)
                return ActionResourceManager.Bard.Timer.TotalMilliseconds - (1000 * BardSettings.Instance.EndArmysPaeonEarlyWithXSecondsRemaining);

            return ActionResourceManager.Bard.Timer.TotalMilliseconds;
        }

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

            return dmgIncrease >= (1 + (double)neededDmgIncrease / 100);
        }
    }
}