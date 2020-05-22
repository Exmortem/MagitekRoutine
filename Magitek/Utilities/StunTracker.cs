using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Objects;

namespace Magitek.Utilities
{
    class StunTracker
    {
        //Concepts of Operation
        //
        //This class tracks stunned enemies to provide two services:
        //  1: Identifying enemies that cannot be stunned
        //  2: Identifying enemies that can be stunned, but that are on stun cooldown
        //
        //The first service is provided by having combat routines call ReportAttemptedStun(). This adds the reported
        //enemy to a monitored list. If no Stun aura appears on the target within 1.5 seconds after the report, the
        //enemy is marked as unstunnable.
        //
        //The second service is provided by periodically scanning all enemies. Each stunned enemy is added to a list of
        //stunnable enemies. The fact that they've been stunned once is also recorded, along with the remaining time that
        //they'll stay stunned.
        //Once they're in the list of stunnable enemies, any new stuns that appear increment their stun count. Once they
        //hit three stuns, they're reported as unstunnable for the next minute. After it expires, they go back to being
        //stunnable again.
        //
        //In order benefit from these services, combat routines can call IsStunnable() before attempting to stun an enemy.
        //
        //Note that this code doesn't make use of permanent storage, so each time it's loaded, it will need to discover
        //afresh what enemies can and cannot be stunned. This comes at the cost of a single stun attempt per enemy type,
        //after which, if the enemy is unstunnable, it won't try again. Note also that this has no effect on interrupts,
        //which will still work on interruptible spells cast by unstunnable enemies.

        //TODO: Enable non-tank classes with stuns to use this. Maybe refactor Tank.Interrupt into a general routine.

        private enum LogLevel
        {
            None = 0,
            Useful = 1,
            Debug = 2
        }
        private const LogLevel mLogLevelToShow = LogLevel.Useful;

        private const double MaxTimeForStunToTakeEffectMs = 1500;

        private static HashSet<uint> mUnstunnableEnemyIds = new HashSet<uint>();
        private static HashSet<uint> mStunnableEnemyIds = new HashSet<uint>();
        private static Dictionary<BattleCharacter, StunData> mAttemptedStunEnemies = new Dictionary<BattleCharacter, StunData>();
        private static Dictionary<BattleCharacter, StunData> mStunnableEnemies = new Dictionary<BattleCharacter, StunData>();

        //This must be called often to make sure we don't miss stuns
        public static void Update(List<BattleCharacter> enemies)
        {
            //Update the stunnable enemies list from current enemy data
            foreach (BattleCharacter bc in enemies)
            {
                Aura stun = bc.GetAuraById(Auras.Stun);
                //This enemy is in our list because it has been stunned before. Record whether it is stunned right now.
                if (mStunnableEnemies.ContainsKey(bc))
                {
                    if (bc.HasAura(Auras.Stun))
                    {
                        mStunnableEnemies[bc].MarkAsStunned(stun.TimespanLeft);
                    }
                    else
                    {
                        mStunnableEnemies[bc].MarkAsNotStunned();
                    }
                }
                //Else the enemy hasn't been stunned before, but it is now, so add it to our list
                else if (bc.HasAura(Auras.Stun))
                {
                    mStunnableEnemyIds.Add(bc.NpcId);
                    mUnstunnableEnemyIds.Remove(bc.NpcId);
                    mStunnableEnemies.Add(bc, new StunData(stun.TimespanLeft, bc));
                    Log(LogLevel.Debug, $"Added to stun list: {bc.EnglishName}");
                }
            }

            //Maintain the stunned enemies list
            foreach (BattleCharacter bc in mStunnableEnemies.Keys.ToList())
            {
                if (!bc.IsValid)
                {
                    Log(LogLevel.Debug, $"Removed no-longer-valid enemy from stun list");
                    mStunnableEnemies.Remove(bc);
                    continue;
                }

                //If the enemy is dead or its stun cooldown has expired, clean it out of the list
                if (!bc.IsAlive || mStunnableEnemies[bc].CooldownExpired)
                {
                    Log(LogLevel.Debug, $"Removed from stun list: {bc.EnglishName}");
                    if (bc.IsAlive)
                    {
                        Log(LogLevel.Useful, $"{bc.EnglishName.ToUpper()} COOLDOWN RESET");
                    }
                    mStunnableEnemies.Remove(bc);
                }
            }

            //Maintain the attempted stuns list
            foreach (BattleCharacter bc in mAttemptedStunEnemies.Keys.ToList())
            {
                if (!bc.IsValid)
                {
                    Log(LogLevel.Debug, $"Removed no-longer-valid enemy from attempted stun list");
                    mStunnableEnemies.Remove(bc);
                    continue;
                }

                //If the enemy is dead or it has been moved to the stunnable list, clean it out of the attempted stun list
                if (!bc.IsValid || !bc.IsAlive || mStunnableEnemies.ContainsKey(bc))
                {
                    Log(LogLevel.Debug, $"Removed from attempted stun list: {bc.EnglishName}");
                    mAttemptedStunEnemies.Remove(bc);
                }
                //Else if it's been long enough since the stun attempt was reported, and no stun aura has appeared, the enemy isn't stunnable
                else if (mAttemptedStunEnemies[bc].MsSinceLastStun > MaxTimeForStunToTakeEffectMs)
                {
                    Log(LogLevel.Debug, $"Removed from attempted stun list: {bc.EnglishName}");
                    mAttemptedStunEnemies.Remove(bc);
                    //Just in case we had a false positive, make sure it isn't already known to be stunnable
                    if (!mStunnableEnemyIds.Contains(bc.NpcId))
                    {
                        Log(LogLevel.Useful, $"{bc.EnglishName.ToUpper()} NOT STUNNABLE");
                        mUnstunnableEnemyIds.Add(bc.NpcId);
                    }
                }
            }
        }

        //Method intended for combat routines to report an attempted stun. This allows the StunTracker to determine
        //if an enemy is stunnable or not
        public static void RecordAttemptedStun(BattleCharacter enemy)
        {
            //If we've already seen it before, we know whether it can be stunned, so don't bother doing it again
            if (   !mUnstunnableEnemyIds.Contains(enemy.NpcId)
                && !mStunnableEnemyIds.Contains(enemy.NpcId)
                && !mStunnableEnemies.ContainsKey(enemy)
                && !mAttemptedStunEnemies.ContainsKey(enemy))
            {
                Log(LogLevel.Debug, $"Recording attempted stun: {enemy.EnglishName}");
                mAttemptedStunEnemies.Remove(enemy);
                mAttemptedStunEnemies.Add(enemy, new StunData(TimeSpan.Zero, enemy));
            }
        }

        //Method intended for use by combat routines to check if an enemy is stunnable before trying to stun it
        //This will report "true" for unknown enemies. If they resist the stun, it will report "false" afterward.
        public static bool IsStunnable(BattleCharacter enemy)
        {
            // - If they're in the unstunnable list, return false
            // - If they're in the stun list and they're on cooldown, return false
            // - If they're in the attempted stun list, we already tried stunning within the last
            //   1500 ms. Either the stun hasn't had time to show up yet, in which case we don't
            //   need to try to stun again already, or they're immune. Either way, return false
            // - If they're already stunned, return false
            //TODO: Figure out a way to prevent trying to stun immediately after an interrupt
            if (   !mUnstunnableEnemyIds.Contains(enemy.NpcId)
                && (!mStunnableEnemies.ContainsKey(enemy) || !mStunnableEnemies[enemy].OnCooldown)
                && !mAttemptedStunEnemies.ContainsKey(enemy)
                && !enemy.HasAura(Auras.Stun))
            {
                return true;
            }

            return false;
        }

        private static void Log(LogLevel logLevel, string strToLog)
        {
            if (logLevel <= mLogLevelToShow)
            {
                Logger.WriteInfo($"[StunTracker] {strToLog}");
            }
        }

        private class StunData
        {
            private const double CooldownTimeoutMs = 60000;

            private int StunCount;
            private DateTime TimeStunDetected;
            private TimeSpan TimeLeftOnStun;
            private BattleCharacter Enemy;

            public double MsSinceLastStun => DateTime.UtcNow.Subtract(TimeStunDetected).TotalMilliseconds;
            public bool OnCooldown => StunCount > 2;
            public bool CooldownExpired => MsSinceLastStun > CooldownTimeoutMs;

            public StunData(TimeSpan timeLeftOnStun, BattleCharacter enemy)
            {
                Enemy = enemy;
                TimeStunDetected = DateTime.UtcNow;
                TimeLeftOnStun = timeLeftOnStun;
                if (timeLeftOnStun > TimeSpan.Zero)
                {
                    Log(LogLevel.Debug, $"Stun count = 1 on {Enemy.EnglishName}");
                    StunCount = 1;
                }
                else
                {
                    Log(LogLevel.Debug, $"Stun count = 0 on {Enemy.EnglishName}");
                    StunCount = 0;
                }
            }

            public void MarkAsStunned(TimeSpan timeLeftOnStun)
            {
                //They're stunned, and thre's more time left on their stun than the last time this was called.
                //This means they've been stunned again.
                if (timeLeftOnStun > TimeLeftOnStun)
                {
                    StunCount++;
                    TimeStunDetected = DateTime.UtcNow;
                    Log(LogLevel.Debug, $"Stun count = {StunCount} on {Enemy.EnglishName}");
                    if (OnCooldown)
                    {
                        Log(LogLevel.Useful, $"{Enemy.EnglishName.ToUpper()} ON COOLDOWN");
                    }
                }
                TimeLeftOnStun = timeLeftOnStun;
            }

            public void MarkAsNotStunned()
            {
                TimeLeftOnStun = TimeSpan.Zero;
            }
        }
    }
}
