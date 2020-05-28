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

                //The amount of time before our interrupt will go off
                int minimumMsLeftOnEnemyCast =   BaseSettings.Instance.UserLatencyOffset
                                               + Globals.AnimationLockMs
                                               + Casting.SpellCastHistory.LastOrDefault()?.AnimationLockRemainingMs ?? 0;

                castingEnemies = castingEnemies.Where(r =>    r.InView()
                                                           && r.IsCasting
                                                           && r.SpellCastInfo.RemainingCastTime.TotalMilliseconds >= minimumMsLeftOnEnemyCast)
                                               .OrderBy(r => r.SpellCastInfo.RemainingCastTime);
            }

            foreach (SpellData stun in stuns)
            {
                BattleCharacter stunTarget = castingEnemies.FirstOrDefault(enemy => StunTracker.IsStunnable(enemy) && enemy.Distance(Core.Me) <= stun.Range + Core.Me.CombatReach + enemy.CombatReach);

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
                BattleCharacter interruptTarget = castingEnemies.FirstOrDefault(enemy => enemy.SpellCastInfo.Interruptible && enemy.Distance(Core.Me) <= interrupt.Range + Core.Me.CombatReach + enemy.CombatReach);

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
