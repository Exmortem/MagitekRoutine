using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Roles;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Roles
{
    public class Tank
    {
        public static async Task<bool> Provoke<T>(T settings) where T : TankSettings
        {
            if (Globals.OnPvpMap) return false;
            if (!settings.UseProvoke) return false;
            if (!Globals.InParty) return false;
            if (Spells.Provoke.Cooldown > TimeSpan.Zero) return false;

            var provokeTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) <= 25 && r.TargetGameObject.IsHealer());

            if (provokeTarget != null)
            {
                if (await Spells.Provoke.Cast(provokeTarget))
                {
                    if (provokeTarget.Distance(Core.Me) > 5)
                    {
                        return (await Spells.ShieldLob.Cast(provokeTarget));
                    }
                    Logger.Write($@"Provoke On {provokeTarget.Name} To Pull Aggro");
                    return true;
                }
            }

            provokeTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) <= 25 && r.TargetGameObject.IsDps());

            if (provokeTarget == null)
                return false;

            if (!await Spells.Provoke.Cast(provokeTarget))
                return false;

            Logger.Write($@"Provoke On {provokeTarget.Name} To Pull Aggro");
            return true;
        }

        public static async Task<bool> Rampart<T>(T settings) where T : TankSettings
        {
            return await BasicHpSetting(settings.UseRampart, settings.RampartHpPercentage, Spells.Rampart, Core.Me, Auras.Rampart);
        }

        private static async Task<bool> BasicHpSetting(bool useSetting, int hpPercentage, SpellData spell, GameObject target, uint aura)
        {
            if (!useSetting)
                return false;

            if (Core.Me.CurrentHealthPercent > hpPercentage)
                return false;

            return await spell.CastAura(target, aura);
        }

        public static async Task<bool> Interrupt<T>(T settings) where T : TankSettings
        {
            if (!settings.UseInterrupt)
                return false;

            //The amount of time before our interrupt will go off
            int minimumMsLeftOnEnemyCast =   BaseSettings.Instance.UserLatencyOffset
                                           + Globals.AnimationLockMs
                                           + Casting.SpellCastHistory.LastOrDefault()?.AnimationLockRemainingMs ?? 0;

            IEnumerable<BattleCharacter> castingEnemies;
            BattleCharacter stunTarget;
            BattleCharacter interruptTarget;

            switch (settings.Strategy)
            {
                case InterruptStrategy.NeverInterrupt:
                    return false;

                case InterruptStrategy.InterruptOnlyBosses:
                    castingEnemies = GameObjectManager.Attackers.Where(r =>    r.IsBoss()
                                                                            && r.InView()
                                                                            && r.IsCasting
                                                                            && r.SpellCastInfo.RemainingCastTime.TotalMilliseconds >= minimumMsLeftOnEnemyCast)
                                                                .OrderBy(r => r.SpellCastInfo.RemainingCastTime);

                    stunTarget = castingEnemies.FirstOrDefault();

                    if (stunTarget == null)
                        return false;

                    if (await Spells.LowBlow.Cast(stunTarget))
                        return true;

                    interruptTarget = castingEnemies.FirstOrDefault(r => r.SpellCastInfo.Interruptible);

                    return await Spells.Interject.Cast(interruptTarget);

                case InterruptStrategy.AlwaysInterrupt:
                    castingEnemies = Combat.Enemies.Where(r =>    r.InView() 
                                                               && r.IsCasting
                                                               && r.SpellCastInfo.RemainingCastTime.TotalMilliseconds >= minimumMsLeftOnEnemyCast)
                                                   .OrderBy(r => r.SpellCastInfo.RemainingCastTime);

                    stunTarget = castingEnemies.FirstOrDefault();

                    if (stunTarget == null)
                        return false;

                    if (await Spells.LowBlow.Cast(stunTarget))
                        return true;

                    interruptTarget = castingEnemies.FirstOrDefault(r => r.SpellCastInfo.Interruptible);

                    return await Spells.Interject.Cast(interruptTarget);

                default:
                    return false;
            }
        }
    }
}