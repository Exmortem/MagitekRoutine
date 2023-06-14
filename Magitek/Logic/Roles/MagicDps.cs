using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Roles;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Roles
{
    internal static class MagicDps
    {
        public static async Task<bool> Interrupt(MagicDpsSettings settings)
        {
            if (!settings.UseStunOrInterrupt)
                return false;

            List<SpellData> stuns = new List<SpellData>();
            List<SpellData> interrupts = new List<SpellData>();

            if (Core.Me.IsBlueMage())
            {
                interrupts.Add(Spells.FlyingSardine);
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
                ActionManager.DoActionLocation(limitBreak3Spell.Id, Core.Me.CurrentTarget.Location);
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
                if (!ActionManager.DoActionLocation(limitBreak2Spell.Id, Core.Me.CurrentTarget.Location))
                    ActionManager.DoActionLocation(limitBreak1Spell.Id, Core.Me.CurrentTarget.Location); ;

                BaseSettings.Instance.ForceLimitBreak = false;
                TogglesManager.ResetToggles();
                return true;
            }
            return false;
        }

        public static async Task<bool> UsePotion<T>(T settings) where T : MagicDpsSettings
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
        public static async Task<bool> Recuperate<T>(T settings) where T : MagicDpsSettings
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

        public static async Task<bool> Purify<T>(T settings) where T : MagicDpsSettings
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

        public static async Task<bool> Guard<T>(T settings) where T : MagicDpsSettings
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

            return await Coroutine.Wait(1500, () => Core.Me.HasAura(Auras.Guard, true)); ;
        }

        public static bool GuardCheck()
        {
            return Core.Me.CurrentTarget.HasAura(Auras.Guard);
        }
    }
}
