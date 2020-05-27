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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Monk
    {
        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {


            await Casting.CheckForSuccessfulCast();

            if (await Buff.FistsOf()) return true;
            if (await Buff.Meditate()) return true;
            if (await Buff.FormShiftOOC()) return true;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
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
            if (await Buff.FormShiftOOC()) return true;
            return await Buff.Meditate();
        }
        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            Utilities.Routines.Monk.RefreshVars();

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

            if (await Buff.FistsOf()) return true;

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
                if (Weaving.GetCurrentWeavingCounter() < 2 && Spells.Bootshine.Cooldown.TotalMilliseconds > 750 + BaseSettings.Instance.UserLatencyOffset)
                {
                    if (await PhysicalDps.SecondWind(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Bloodbath(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Feint(MonkSettings.Instance)) return true;
                    if (await Buff.Brotherhood()) return true;
                    if (await Aoe.Enlightenment()) return true;
                    if (await SingleTarget.TheForbiddenChakra()) return true;
                    if (await SingleTarget.ShoulderTackle()) return true;
                    if (await Buff.PerfectBalance()) return true;
                    if (!Core.Me.HasAura(Auras.RiddleOfEarth))
                    {
                        if (await PhysicalDps.TrueNorth(MonkSettings.Instance)) return true;
                    }
                    if (await Buff.RiddleOfFire()) return true;
                    if (await Buff.RiddleOfEarth()) return true;
                    if (await SingleTarget.ElixerField()) return true;
                }
                if (await SingleTarget.PerfectBalanceRoT()) return true;
                if (await Aoe.Rockbreaker()) return true;
                if (await Aoe.FourPointStrike()) return true;
                if (await Aoe.ArmOfDestroyer()) return true;
                if (await SingleTarget.Demolish()) return true;
                if (await SingleTarget.SnapPunch()) return true;
                if (await SingleTarget.TwinSnakes()) return true;
                if (await SingleTarget.TrueStrike()) return true;
                if (await SingleTarget.Bootshine()) return true;
                if (await SingleTarget.DragonKick()) return true;
                return await Buff.FormShiftIC();
            }
            else
                return false;
        }
        public static async Task<bool> PvP()
        {
            return false;
        }

        public static void RegisterCombatMessages()
        {

            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat));

            //Second priority (tie): Bootshine
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "Bootshine: Get behind Enemy",
                                          () => Core.Me.HasAura(Auras.OpoOpoForm) && Core.Me.HasAura(Auras.LeadenFist) && !Core.Me.HasAura(Auras.PerfectBalance)));

            //Second priority (tie): TwinSnakes
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "TwinSnakes: Side of Enemy",
                                          () => Core.Me.HasAura(Auras.RaptorForm) && !Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1100) && !Core.Me.HasAura(Auras.PerfectBalance)));

            //Second priority (tie): TrueStrike
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "TrueStrike: Get behind Enemy",
                                          () => Core.Me.HasAura(Auras.RaptorForm) && Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000) && !Core.Me.HasAura(Auras.PerfectBalance)));

            //Second priority (tie): DragonKick
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "DragonKick: Side of Enemy",
                                          () => Core.Me.HasAura(Auras.OpoOpoForm) && !Core.Me.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000) && !Core.Me.HasAura(Auras.PerfectBalance)));

        }
    }
}
