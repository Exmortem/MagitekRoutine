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

        public static async Task<bool> DoStunAndInterrupt(IEnumerable<SpellData> stuns, IEnumerable<SpellData> interrupts, InterruptStrategy strategy)
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

            foreach (SpellData stun in stuns)
            {
                int spellAnimationLockMs = Globals.AnimationLockMs;
                if (animationLockDict.ContainsKey(stun))
                {
                    spellAnimationLockMs = animationLockDict[stun];
                }

                //The amount of time before our spell will take effect
                int minimumMsLeftOnEnemyCast =   BaseSettings.Instance.UserLatencyOffset
                                               + spellAnimationLockMs
                                               + Casting.SpellCastHistory.FirstOrDefault()?.AnimationLockRemainingMs ?? 0;

                //Default to the reported range if we don't have a more accurate range in our dictionary
                double stunRange = stun.Range;
                if (rangeDict.ContainsKey(stun))
                {
                    stunRange = rangeDict[stun];
                }

                BattleCharacter stunTarget = castingEnemies.FirstOrDefault(enemy =>    enemy.SpellCastInfo.RemainingCastTime.TotalMilliseconds >= minimumMsLeftOnEnemyCast
                                                                                    && StunTracker.IsStunnable(enemy)
                                                                                    && enemy.Distance(Core.Me) <= stunRange + Core.Me.CombatReach + enemy.CombatReach);

                if (stunTarget == null)
                {
                    continue;
                }

                if (await stun.Cast(stunTarget))
                {
                    StunTracker.RecordAttemptedStun(stunTarget);
                    return true;
                }
            }

            foreach (SpellData interrupt in interrupts)
            {
                int spellAnimationLockMs = Globals.AnimationLockMs;
                if (animationLockDict.ContainsKey(interrupt))
                {
                    spellAnimationLockMs = animationLockDict[interrupt];
                }

                //The amount of time before our spell will take effect
                int minimumMsLeftOnEnemyCast =   BaseSettings.Instance.UserLatencyOffset
                                               + spellAnimationLockMs
                                               + Casting.SpellCastHistory.FirstOrDefault()?.AnimationLockRemainingMs ?? 0;

                //Default to the reported range if we don't have a more accurate range in our dictionary
                double interruptRange = interrupt.Range;
                if (rangeDict.ContainsKey(interrupt))
                {
                    interruptRange = rangeDict[interrupt];
                }

                BattleCharacter interruptTarget = castingEnemies.FirstOrDefault(enemy =>    enemy.SpellCastInfo.RemainingCastTime.TotalMilliseconds >= minimumMsLeftOnEnemyCast
                                                                                         && enemy.SpellCastInfo.Interruptible
                                                                                         && enemy.Distance(Core.Me) <= interruptRange + Core.Me.CombatReach + enemy.CombatReach);

                if (interruptTarget == null)
                {
                    continue;
                }

                if (await interrupt.Cast(interruptTarget))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
