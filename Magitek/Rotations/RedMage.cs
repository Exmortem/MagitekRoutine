using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.RedMage;
using Magitek.Models;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using RedMageRoutines = Magitek.Utilities.Routines.RedMage;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Rotations
{
    public static class RedMage
    {
        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.ClassLevel < 2 ? 3 : 20);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Logic.RedMage.Heal.Verraise()) return true;
            return await Logic.RedMage.Heal.Vercure();
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.ClassLevel < 2 ? 3 : 20);
                }
            }

            if (await Buff.LucidDreaming()) return true;

            if (RedMageRoutines.CanWeave)
            {
                if (await SingleTarget.Fleche()) return true;
                if (await Aoe.ContreSixte()) return true;
            }

            if (RedMageSettings.Instance.UseAoe)
            {
                if (await Aoe.Embolden()) return true;
                if (await Aoe.Manafication()) return true;
                if (await Aoe.Moulinet()) return true;
                if (await Aoe.Scatter()) return true;
                if (await Aoe.Veraero2()) return true;
                if (await Aoe.Verthunder2()) return true;
            }

            if (await Buff.Embolden()) return true;
            if (await Buff.Manafication()) return true;

            if (await SingleTarget.CorpsACorps()) return true;

            if (await SingleTarget.Scorch()) return true;
            if (await SingleTarget.Verholy()) return true;
            if (await SingleTarget.Verflare()) return true;
            if (await SingleTarget.Redoublement()) return true;
            if (await SingleTarget.Zwerchhau()) return true;
            if (await SingleTarget.Riposte()) return true;

            if (await SingleTarget.Displacement()) return true;
            if (await SingleTarget.Engagement()) return true;

            if (await SingleTarget.Verstone()) return true;
            if (await SingleTarget.Verfire()) return true;
            if (await SingleTarget.Veraero()) return true;
            if (await SingleTarget.Verthunder()) return true;
            if (await Buff.Acceleration()) return true;
            if (await SingleTarget.Jolt()) return true;
            return await SingleTarget.Reprise();
        }

        public static async Task<bool> PvP()
        {
            return false;
        }

        public static void RegisterCombatMessages()
        {
            Func<bool> bossPresenceOk = () => !RedMageSettings.Instance.MeleeComboBossesOnly || Utilities.Combat.Enemies.Any(e => e.IsBoss());

            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat));

            //Second priority: Melee combo is ready
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "Combo Ready!",
                                          () =>    SingleTarget.ReadyForCombo()
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    SingleTarget.ReadyForCombo(BlackMana + 9, WhiteMana + 9)
                                                && !SingleTarget.ComboInProgress
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon, but based on different conditions
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    SingleTarget.ReadyForManaficationComboSoon
                                                && !SingleTarget.ComboInProgress
                                                && bossPresenceOk()
                                                && RedMageSettings.Instance.Manafication
                                                && Spells.Manafication.Cooldown.TotalMilliseconds <= 5000));
        }
    }
}
