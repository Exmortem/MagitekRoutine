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
    internal static class PhysicalDps
    {
        public static async Task<bool> SecondWind<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseSecondWind)
                return false;

            if (Core.Me.CurrentHealthPercent > settings.SecondWindHpPercent)
                return false;

            return await Spells.SecondWind.Cast(Core.Me);
        }

        public static async Task<bool> TrueNorth<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseTrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.SpellCastHistory.Take(10).Any(s => s.Spell == Spells.TrueNorth))
                return false;

            return await Spells.TrueNorth.Cast(Core.Me);
        }

        public static async Task<bool> ArmsLength<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.ForceArmsLength)
                return false;

            if (!await Spells.ArmsLength.Cast(Core.Me)) return false;
            settings.ForceArmsLength = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> Bloodbath<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseBloodbath)
                return false;

            if (Core.Me.CurrentHealthPercent > settings.BloodbathHpPercent)
                return false;

            return await Spells.Bloodbath.Cast(Core.Me);
        }

        public static async Task<bool> Feint<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseFeint)
                return false;

            return await Spells.Feint.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Interrupt(PhysicalDpsSettings settings)
        {
            if (!settings.UseStunOrInterrupt)
                return false;

            List<SpellData> stuns = new List<SpellData>();
            List<SpellData> interrupts = new List<SpellData>();

            if (Core.Me.IsMeleeDps())
            {
                stuns.Add(Spells.LegSweep);
            }

            if (Core.Me.IsRangedDps())
            {
                interrupts.Add(Spells.HeadGraze);
            }

            return await InterruptAndStunLogic.StunOrInterrupt(stuns, interrupts, settings.Strategy);
        }

        public static async Task<bool> Peloton<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UsePeloton)
                return false;

            if (Globals.InParty)
            {
                if (Globals.PartyInCombat)
                    return false;
            }

            if (Combat.OutOfCombatTime.ElapsedMilliseconds <= 3000)
                return false;

            if (!MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Peloton, false, 1000))
                return false;

            return await Spells.Peloton.CastAura(Core.Me, Auras.Peloton);
        }

        public static async Task<bool> UsePotion<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UsePotion)
                return false;

            if (Core.Me.HasAura(Auras.Medicated, true))
                return false;

            return await Potion.UsePotion((int) settings.PotionTypeAndGradeLevel);
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
                ActionManager.DoAction(limitBreak3Spell, Core.Me.CurrentTarget);
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
                if (!ActionManager.DoAction(limitBreak2Spell, Core.Me.CurrentTarget))
                    ActionManager.DoAction(limitBreak1Spell, Core.Me.CurrentTarget);

                BaseSettings.Instance.ForceLimitBreak = false;
                TogglesManager.ResetToggles();
                return true;
            }
            return false;
        }

        #region pvp
        public static async Task<bool> Recuperate<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.Pvp_UseRecuperate)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false; 

            if (!Spells.Recuperate.CanCast())
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_RecuperateHealthPercent)
                return false;

            return await Spells.Recuperate.Cast(Core.Me);
        }

        public static async Task<bool> Purify<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.Pvp_UsePurify)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.Purify.CanCast())
                return false;

            if (!Core.Me.HasAura("Stun") && !Core.Me.HasAura("Heavy") && !Core.Me.HasAura("Bind") && !Core.Me.HasAura("Silence") && !Core.Me.HasAura("Half-asleep") && !Core.Me.HasAura("Sleep") && !Core.Me.HasAura("Deep Freeze"))
                return false;

            return await Spells.Purify.Cast(Core.Me);
        }

        public static async Task<bool> Guard<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.Pvp_UseGuard)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Spells.Guard.IsKnown() && !Spells.Guard.IsReady())
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_GuardHealthPercent)
                return false;

            return await Spells.Guard.CastAura(Core.Me, Auras.Guard);
        }
        #endregion
    }
}
