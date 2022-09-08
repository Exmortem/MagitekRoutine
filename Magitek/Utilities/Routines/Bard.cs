using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using BardSong = ff14bot.Managers.ActionResourceManager.Bard.BardSong;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Bard
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Bard, Spells.HeavyShot);

        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;
        public static bool AlreadySnapped = false;
        public static bool IsUnderBuffWindow = false;
        public static int SongMaxDuration = 45000;

        public static SpellData LadonsBite => Core.Me.ClassLevel < 82 ? Spells.QuickNock : Spells.Ladonsbite;
        public static SpellData Stormbite => Core.Me.ClassLevel < 64 ? Spells.Windbite : Spells.Stormbite;
        public static SpellData CausticBite => Core.Me.ClassLevel < 64 ? Spells.VenomousBite : Spells.CausticBite;
        public static SpellData BurstShot => Core.Me.ClassLevel < 76 ? Spells.HeavyShot : Spells.BurstShot;
        public static uint WindbiteAura => (uint)(Core.Me.ClassLevel < 64 ? Auras.Windbite : Auras.StormBite);
        public static uint VenomousBiteAura => (uint)(Core.Me.ClassLevel < 64 ? Auras.VenomousBite : Auras.CausticBite);

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(15);
            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

            IsUnderBuffWindow = Spells.RadiantFinale.IsKnown() ? Core.Me.HasAura(Auras.RadiantFinale) && Core.Me.HasAura(Auras.BattleVoice) && Core.Me.HasAura(Auras.RagingStrikes)
            : Spells.BattleVoice.IsKnown() ? Core.Me.HasAura(Auras.BattleVoice) && Core.Me.HasAura(Auras.RagingStrikes)
            : Core.Me.HasAura(Auras.RagingStrikes);

            if (Casting.LastSpell == Spells.IronJaws && Core.Me.HasAura(Auras.RagingStrikes))
            {
                AlreadySnapped = true;
            }
            AlreadySnapped = AlreadySnapped && Core.Me.HasAura(Auras.RagingStrikes);
        }

        public static double CurrentSongDuration()
        {
            if (BardSong.WanderersMinuet.Equals(ActionResourceManager.Bard.ActiveSong))
            {
                if (BardSettings.Instance.EndWanderersMinuetEarly)
                    return ActionResourceManager.Bard.Timer.TotalMilliseconds - BardSettings.Instance.EndWanderersMinuetEarlyWithXMilliSecondsRemaining;
            }

            if (BardSong.MagesBallad.Equals(ActionResourceManager.Bard.ActiveSong))
            {
                if (BardSettings.Instance.EndMagesBalladEarly)
                    return ActionResourceManager.Bard.Timer.TotalMilliseconds - BardSettings.Instance.EndMagesBalladEarlyWithXMilliSecondsRemaining;
            }

            if (BardSong.ArmysPaeon.Equals(ActionResourceManager.Bard.ActiveSong))
            {
                if (BardSettings.Instance.EndArmysPaeonEarly)
                    return ActionResourceManager.Bard.Timer.TotalMilliseconds - BardSettings.Instance.EndArmysPaeonEarlyWithXMilliSecondsRemaining;
            }

            return ActionResourceManager.Bard.Timer.TotalMilliseconds;
        }

        public static double NextTickUnderCurrentSong()
        {
            //return ActionResourceManager.Bard.Timer.TotalMilliseconds - TimeUntilNextPossibleDoTTick();
            return CurrentSongDuration() - TimeUntilNextPossibleDoTTick();
        }

        public static double TimeUntilNextPossibleDoTTick()
        {
            if (ActionResourceManager.Bard.ActiveSong != BardSong.None)
                return ActionResourceManager.Bard.Timer.TotalMilliseconds % 3000;

            return 0;
        }

        public static bool CheckCurrentDamageIncrease(int neededDmgIncrease)
        {
            double dmgIncrease = 1;

            //From tincture
            if (Core.Me.HasAura(Auras.Medicated))
                dmgIncrease *= 1.08;

            //From PLD
            //From DRK
            //From GNB
            //From WAR
            //From WHM
            //From BLM
            //From SAM
            //From MCH

            //From BRD
            if (Core.Me.HasAura(Auras.RagingStrikes))
                dmgIncrease *= 1.15; 
            if (Core.Me.HasAura(Auras.RadiantFinale))
                dmgIncrease *= 1.06;
            if (Core.Me.HasAura(Auras.BattleVoice))
                dmgIncrease *= 1.01;
            if (Core.Me.HasAura(Auras.TheWanderersMinuet))
                dmgIncrease *= 1.02;
            if (Core.Me.HasAura(Auras.MagesBallad))
                dmgIncrease *= 1.01;
            if (Core.Me.HasAura(Auras.ArmysPaeon))
                dmgIncrease *= 1.005;

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

            if (dmgIncrease >= (1 + (double)neededDmgIncrease / 100))
                Logger.WriteInfo($@"[DamageIncrease] Calculated increased damage = {dmgIncrease} | Expected minimum Increased Damage = {1 + (double)neededDmgIncrease / 100}");
            
            return dmgIncrease >= (1 + (double)neededDmgIncrease / 100);
        }
    }
}