using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Roles;
using Magitek.Toggles;
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

        //If the calling class has stuns or interrupts beyond the default tank abilities (e.g.,
        //Paladin has Shield Bash in addition to Low Blow and Interject), pass them in the
        //extraStuns and extraInterrupts parameters
        public static async Task<bool> Interrupt(TankSettings settings, IEnumerable<SpellData> extraStuns = null, IEnumerable<SpellData> extraInterrupts = null)
        {
            if (!settings.UseStunOrInterrupt)
                return false;

            List<SpellData> stuns = new List<SpellData>() { Spells.LowBlow };
            List<SpellData> interrupts = new List<SpellData>() { Spells.Interject };

            if (extraStuns != null)
            {
                stuns.AddRange(extraStuns);
            }

            if (extraInterrupts != null)
            {
                interrupts.AddRange(extraInterrupts);
            }

            return await InterruptAndStunLogic.StunOrInterrupt(stuns, interrupts, settings.Strategy);
        }

        public static bool ForceLimitBreak(SpellData limitBreak1Spell, SpellData limitBreak2Spell, SpellData limitBreak3Spell, SpellData gcd)
        {
            if (!BaseSettings.Instance.ForceLimitBreak)
                return false;

            //LB 3
            if (PartyManager.NumMembers == 8
                && !Casting.SpellCastHistory.Any(s => s.Spell == limitBreak3Spell)
                && gcd.Cooldown.TotalMilliseconds < 500)
            {
                ActionManager.DoAction(limitBreak3Spell, Core.Me);
                BaseSettings.Instance.ForceLimitBreak = false;
                TogglesManager.ResetToggles();
                return true;
            }

            //LB 2 or LB 1
            if (PartyManager.NumMembers == 4
                && !Casting.SpellCastHistory.Any(s => s.Spell == limitBreak1Spell)
                && !Casting.SpellCastHistory.Any(s => s.Spell == limitBreak2Spell)
                && gcd.Cooldown.TotalMilliseconds < 500)
            {
                if (!ActionManager.DoAction(limitBreak2Spell, Core.Me))
                    ActionManager.DoAction(limitBreak1Spell, Core.Me);

                BaseSettings.Instance.ForceLimitBreak = false;
                TogglesManager.ResetToggles();
                return true;
            }
            return false;
        }

        public static async Task<bool> UsePotion<T>(T settings) where T : TankSettings
        {
            if (!settings.UsePotion)
                return false;

            if (Core.Me.HasAura(Auras.Medicated, true))
                return false;

            return await Potion.UsePotion((int)settings.PotionTypeAndGradeLevel);
        }

        public static async Task<bool> Recuperate<T>(T settings) where T : TankSettings
        {
            if (!settings.Pvp_UseRecuperate)
                return false;

            if (!Spells.Recuperate.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_RecuperateHealthPercent)
                return false;

            return await Spells.Recuperate.Cast(Core.Me);
        }

        public static async Task<bool> Purify<T>(T settings) where T : TankSettings
        {
            if (!settings.Pvp_UsePurify)
                return false;

            if (!Spells.Purify.CanCast())
                return false;

            if (!Core.Me.HasAura("Stun") && !Core.Me.HasAura("Heavy") && !Core.Me.HasAura("Bind") && !Core.Me.HasAura("Silence") && !Core.Me.HasAura("Half-asleep") && !Core.Me.HasAura("Sleep") && !Core.Me.HasAura("Deep Freeze"))
                return false;

            return await Spells.Purify.Cast(Core.Me);
        }

        public static async Task<bool> Guard<T>(T settings) where T : TankSettings
        {
            if (!settings.Pvp_UseGuard)
                return false;

            if (!Spells.Guard.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_GuardHealthPercent)
                return false;

            return await Spells.Guard.Cast(Core.Me);
        }
    }
}