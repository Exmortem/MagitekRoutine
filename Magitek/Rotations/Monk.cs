using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Monk;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Monk;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using MonkRoutine = Magitek.Utilities.Routines.Monk;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Monk
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {
            await Casting.CheckForSuccessfulCast();
            if (WorldManager.InSanctuary)
                return false;

            if (await Buff.Meditate()) return true;
            //if (await Buff.FormShiftOOC()) return true;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Buff.Mantra()) return true;

            return false;
        }

        public static async Task<bool> CombatBuff()
        {
            //if (await Buff.FormShiftOOC()) return true;
            return await Buff.Meditate();
        }

        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            MonkRoutine.RefreshVars();

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;


            if (await Buff.Meditate()) return true;

            //var count = Utilities.Combat.Enemies.Count;
            //if (2 >= count && count < 5)
            //{
            //    //TODO: Add 2-4 target DPS rotation
            //}
            //else if (count >= 5)
            //{
            //    //TODO: Add 5+ target DPS rotation
            //}
            if (!Core.Me.HasAura(Auras.Anatman) || MonkSettings.Instance.UseManualPB && Core.Me.HasAura(Auras.PerfectBalance))
            {
                if (MonkRoutine.GlobalCooldown.CountOGCDs() < 2 && Spells.Bootshine.Cooldown.TotalMilliseconds > 750 + BaseSettings.Instance.UserLatencyOffset)
                {
                    if (await PhysicalDps.Interrupt(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.SecondWind(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Bloodbath(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Feint(MonkSettings.Instance)) return true;
                    if (await Buff.RiddleOfFire()) return true;
                    if (await Buff.Brotherhood()) return true;
                    if (await SingleTarget.TornadoKick()) return true;
                    if (await Aoe.Enlightenment()) return true;
                    if (await SingleTarget.TheForbiddenChakra()) return true;
                    if (await Buff.PerfectBalance()) return true;
                    if (await Buff.TrueNorthRiddleOfEarth()) return true;
                    //if (await Buff.RiddleOfFire()) return true;
                    //if (await Buff.Brotherhood()) return true;
                    if (await SingleTarget.ElixirField()) return true;
                }
                if (await Aoe.Rockbreaker()) return true;
                if (await Aoe.FourPointStrike()) return true;
                if (await Aoe.ArmOfDestroyer()) return true;
                if (await SingleTarget.PerfectBalanceRoT()) return true;
                //if (await Aoe.Rockbreaker()) return true;
                //if (await Aoe.FourPointStrike()) return true;
                //if (await Aoe.ArmOfDestroyer()) return true;
                if (await SingleTarget.Demolish()) return true;
                if (await SingleTarget.SnapPunch()) return true;
                if (await SingleTarget.TwinSnakes()) return true;
                if (await SingleTarget.TrueStrike()) return true;
                if (await SingleTarget.Bootshine()) return true;
                if (await SingleTarget.DragonKick()) return true;
                return await Buff.FormShiftIC();
            }

            return false;
        }
        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }

        public static void RegisterCombatMessages()
        {

            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat));

            //Second priority: Don't show anything if positional requirements are Nulled
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "",
                                          () => MonkSettings.Instance.HidePositionalToastsWithTn && Core.Me.HasAura(Auras.TrueNorth) || Core.Me.HasAura(Auras.RiddleOfEarth)));

            //Third priority (tie): Bootshine
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Bootshine: Get behind Enemy",
                                          () => Core.Me.HasAura(Auras.OpoOpoForm) && Core.Me.HasAura(Auras.LeadenFist)));

            //Third priority (tie): TwinSnakes
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "TwinSnakes: Side of Enemy",
                                          () => Core.Me.HasAura(Auras.RaptorForm) && !Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1100)));

            //Third priority (tie): TrueStrike
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "TrueStrike: Get behind Enemy",
                                          () => Core.Me.HasAura(Auras.RaptorForm) && Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000)));

            //Third priority (tie): SnapPunch
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "SnapPunch: Side of Enemy",
                                          () => Core.Me.HasAura(Auras.CoeurlForm) && Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000)));

            //Third priority (tie): DragonKick
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "DragonKick: Side of Enemy",
                                          () => Core.Me.HasAura(Auras.OpoOpoForm) && !Core.Me.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000)));
        }
    }
}
