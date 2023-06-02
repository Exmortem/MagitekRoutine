using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Roles;
using Magitek.Toggles;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Roles
{
    public class Healer
    {
        public static async Task<bool> LucidDreaming(bool useLucid, float manaPercent)
        {
            if (!useLucid)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent > manaPercent)
                return false;

            if (Spells.LucidDreaming.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.CombatTotalTimeLeft <= 20)
                return false;

            return await Spells.LucidDreaming.CastAura(Core.Me, Auras.LucidDreaming);
        }

        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }

        public static async Task<bool> Raise(
            SpellData spell,
            bool SwiftcastRes,
            bool SlowcastRes,
            bool ResOutOfCombat,
            // isSwiftcast, continue
            Func<bool, Task<bool>> extensions = null
        )
        {
            if (!Globals.InParty)
                return false;

            if (!spell.IsKnown())
                return false;

            if (Core.Me.CurrentMana < spell.Cost)
                return false;

            var deadList = Group.DeadAllies.Where(u => u.CurrentHealth == 0 &&
                                                       !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30 &&
                                                       u.IsVisible &&
                                                       u.InLineOfSight() &&
                                                       u.IsTargetable)
                .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (!deadTarget.IsTargetable)
                return false;

            if (Globals.PartyInCombat)
            {
                if (SwiftcastRes && Spells.Swiftcast.IsKnownAndReady())
                {
                    // Prevent burning switftcast if no mana to actually rez.
                    if (!ActionManager.CanCast(spell, deadTarget))
                        return false;

                    if (await Swiftcast())
                    {
                        while (Core.Me.HasAura(Auras.Swiftcast))
                        {
                            if (extensions != null && !await extensions.Invoke(true)) return false;
                            if (await spell.CastAura(deadTarget, Auras.Raise))
                                return true;
                            await Coroutine.Yield();
                        }
                    }
                }
            }

            if (Globals.PartyInCombat && SlowcastRes || !Globals.PartyInCombat && ResOutOfCombat)
            {
                if (extensions != null && !await extensions.Invoke(false)) return false;
                return await spell.CastAura(deadTarget, Auras.Raise);
            }

            return false;
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

        public static async Task<bool> UsePotion<T>(T settings) where T : HealerSettings
        {
            if (!settings.UsePotion)
                return false;

            if (Core.Me.HasAura(Auras.Medicated, true))
                return false;

            return await Potion.UsePotion((int)settings.PotionTypeAndGradeLevel);
        }


        /****************************************************************************************************
         *                                                 PVP
         * **************************************************************************************************/
        public static async Task<bool> Recuperate<T>(T settings) where T :HealerSettings
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

        public static async Task<bool> Purify<T>(T settings) where T : HealerSettings
        {
            if (!settings.Pvp_UsePurify)
                return false;

            if (!Spells.Purify.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.HasAura("Stun") && !Core.Me.HasAura("Heavy") && !Core.Me.HasAura("Bind") && !Core.Me.HasAura("Silence") && !Core.Me.HasAura("Half-asleep") && !Core.Me.HasAura("Sleep") && !Core.Me.HasAura("Deep Freeze"))
                return false;

            return await Spells.Purify.Cast(Core.Me);
        }

        public static async Task<bool> Guard<T>(T settings) where T : HealerSettings
        {
            if (!settings.Pvp_UseGuard)
                return false;

            if (!Spells.Guard.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_GuardHealthPercent)
                return false;

            if (!await Spells.Guard.CastAura(Core.Me, Auras.Guard))
                return false;

            return await Coroutine.Wait(1500, () => Core.Me.HasAura(Auras.Guard, true));
        }

        public static bool GuardCheck()
        {
            return Core.Me.CurrentTarget.HasAura(Auras.Guard);
        }
    }
}