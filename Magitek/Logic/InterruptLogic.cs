using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Utilities;

namespace Magitek.Logic
{
    internal static class InterruptAndStunLogic
    {
        //SpellData.Range is incorrect for a lot of spells - these values were verified manually
        private static Dictionary<SpellData, double> rangeDict = new Dictionary<SpellData, double>()
        {
            { Spells.LowBlow,    3.4 },
            { Spells.Interject,  3.4 },
            { Spells.ShieldBash, 3.4 },
            { Spells.LegSweep,   3.4 },
            { Spells.HeadGraze,  25 }
        };

        //TODO: Verify the rest of these
        private static Dictionary<SpellData, int> animationLockDict = new Dictionary<SpellData, int>()
        {
            { Spells.LowBlow,    700 }, //Verified
            { Spells.Interject,  700 }, //Verified
            { Spells.ShieldBash, 550 }, //Verified
            { Spells.LegSweep,   700 }, //Just a guess
            { Spells.HeadGraze,  700 }  //Just a guess
        };

        //Try to stun or interrupt an enemy per the given strategy
        //Potential targets are sorted by the amount of time left before their spell goes off. The one with the smallest amount of time left
        //that's still enough for the stun or interrupt to go off is chosen.
        public static async Task<bool> StunOrInterrupt(IEnumerable<SpellData> stuns, IEnumerable<SpellData> interrupts, InterruptStrategy strategy)
        {
            IEnumerable<BattleCharacter> castingEnemies;

            if (strategy == InterruptStrategy.Never)
            {
                castingEnemies = new List<BattleCharacter>();
            }
            else
            {
                castingEnemies = Combat.Enemies;
                if (strategy == InterruptStrategy.BossesOnly)
                {
                    castingEnemies = castingEnemies.Where(e => e.IsBoss());
                }
                else if (strategy == InterruptStrategy.CurrentTargetOnly)
                {
                    castingEnemies = castingEnemies.Where(e => e == Core.Me.CurrentTarget);
                }

                castingEnemies = castingEnemies.Where(r => r.InView() && r.IsCasting)
                                               .OrderBy(r => r.SpellCastInfo.RemainingCastTime);
            }

            if (await DoStuns(castingEnemies, stuns)) return true;
            return await DoInterrupts(castingEnemies, interrupts);
        }

        //Try to stun an enemy and record the results
        private static async Task<bool> DoStuns(IEnumerable<BattleCharacter> enemies, IEnumerable<SpellData> stuns)
        {
            BattleCharacter target = await SelectTargetAndCastSpell(enemies, stuns, (enemy) => StunTracker.IsStunnable(enemy));
            if (target != null)
            {
                StunTracker.RecordAttemptedStun(target);
                return true;
            }
            return false;
        }

        //Try to interrupt an enemy
        private static async Task<bool> DoInterrupts(IEnumerable<BattleCharacter> enemies, IEnumerable<SpellData> interrupts)
        {
            return await SelectTargetAndCastSpell(enemies, interrupts, (enemy) => enemy.SpellCastInfo.Interruptible) != null ? true : false;
        }


        private static async Task<BattleCharacter> SelectTargetAndCastSpell(IEnumerable<BattleCharacter> enemies, IEnumerable<SpellData> spells, Func<BattleCharacter, bool> validateEnemy)
        {
            foreach (SpellData spell in spells)
            {
                int spellAnimationLockMs = Globals.AnimationLockMs;
                if (animationLockDict.ContainsKey(spell))
                {
                    spellAnimationLockMs = animationLockDict[spell];
                }

                //The amount of time before our spell will take effect
                int minimumMsLeftOnEnemyCast = BaseSettings.Instance.UserLatencyOffset
                                               + spellAnimationLockMs
                                               + Casting.SpellCastHistory.FirstOrDefault()?.AnimationLockRemainingMs ?? 0;

                //Default to the reported range if we don't have a more accurate range in our dictionary
                double interruptRange = spell.Range;
                if (rangeDict.ContainsKey(spell))
                {
                    interruptRange = rangeDict[spell];
                }

                BattleCharacter target = enemies.FirstOrDefault(enemy =>    enemy.SpellCastInfo.RemainingCastTime.TotalMilliseconds >= minimumMsLeftOnEnemyCast
                                                                         && validateEnemy(enemy)
                                                                         && enemy.Distance(Core.Me) <= interruptRange + Core.Me.CombatReach + enemy.CombatReach);

                if (target == null)
                {
                    continue;
                }

                if (await spell.Cast(target))
                {
                    return target;
                }
            }

            return null;
        }
    }
}
