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
using Magitek.Models.Scholar;

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
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            await Casting.CheckForSuccessfulCast();
            if (WorldManager.InSanctuary)
                return false;

            if (await Buff.Meditate()) return true;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Buff.Mantra()) return true;

            return false;
        }

        public static async Task<bool> CombatBuff()
        {
            return await Buff.Meditate();
        }

        public static async Task<bool> Combat()
        {

            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();


            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            MonkRoutine.RefreshVars();

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) 
                    return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) 
                return true;

            //Limit Break
            if (SingleTarget.ForceLimitBreak())
                return true;

            //Buff
            if (await Buff.Meditate()) 
                return true;

            if (!Core.Me.HasAura(Auras.Anatman))
            {
                if (MonkRoutine.GlobalCooldown.CountOGCDs() < 2 && Spells.Bootshine.Cooldown.TotalMilliseconds > 750 + BaseSettings.Instance.UserLatencyOffset)
                {
                    if (await PhysicalDps.Interrupt(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.SecondWind(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Bloodbath(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Feint(MonkSettings.Instance)) return true;
                    if (await Buff.UsePotion()) return true;

                    if (await Buff.RiddleOfFire()) return true;
                    if (await Buff.RiddleOfWind()) return true;
                    if (await Aoe.Enlightenment()) return true;
                    if (await SingleTarget.TheForbiddenChakra()) return true;
                    if (await Buff.Brotherhood()) return true;
                    if (await Buff.PerfectBalance()) return true;
                    if (await Buff.RiddleOfWind()) return true;
                    if (await Buff.Mantra()) return true;
                }

                if (await Aoe.MasterfulBlitz()) return true;
                if (await SingleTarget.PerfectBalancePhoenix()) return true;
                if (await SingleTarget.PerfectBalanceElixir()) return true;
                if (await SingleTarget.PerfectBalanceRoT()) return true;

                if (await Aoe.FourPointStrike()) return true;
                if (await Aoe.Rockbreaker()) return true;
                if (await Aoe.ArmOfDestroyer()) return true;

                if (await SingleTarget.DragonKick()) return true;
                if (await SingleTarget.TwinSnakes()) return true;
                if (await SingleTarget.Demolish()) return true;
                if (await SingleTarget.TrueStrike()) return true;
                if (await SingleTarget.SnapPunch()) return true;
                if (await SingleTarget.Bootshine()) return true;

                return await Buff.FormShiftIC();
            }

            return false;
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
                                          () => MonkSettings.Instance.HidePositionalMessage && Core.Me.HasAura(Auras.TrueNorth) || MonkSettings.Instance.EnemyIsOmni));

            //Third priority (tie): Snap punch
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Snap punch: Side of Enemy", "/Magitek;component/Resources/Images/General/ArrowSidesHighlighted.png",
                                          () => Core.Me.HasAura(Auras.CoeurlForm) && !Core.Me.HasAura(Auras.PerfectBalance) && MonkRoutine.AoeEnemies5Yards < MonkSettings.Instance.AoeEnemies && MonkSettings.Instance.DemolishUseTtd && Core.Me.CurrentTarget.CombatTimeLeft() <= MonkSettings.Instance.DemolishMinimumTtd));

            //fourth priority (tie): Demolish
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(400,
                                          "Demolish: Get behind Enemy", "/Magitek;component/Resources/Images/General/ArrowDownHighlighted.png",
                                          () => Core.Me.HasAura(Auras.CoeurlForm) && !(Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000)) && !Core.Me.HasAura(Auras.PerfectBalance) && MonkRoutine.AoeEnemies5Yards < MonkSettings.Instance.AoeEnemies));

            //fourth priority (tie): Snap punch
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(400,
                                          "Snap punch: Side of Enemy", "/Magitek;component/Resources/Images/General/ArrowSidesHighlighted.png",
                                          () => Core.Me.HasAura(Auras.CoeurlForm) && (Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000)) && !Core.Me.HasAura(Auras.PerfectBalance) && MonkRoutine.AoeEnemies5Yards < MonkSettings.Instance.AoeEnemies));

        }

        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            MonkRoutine.RefreshVars();

            if (await PhysicalDps.Guard(MonkSettings.Instance)) return true;
            if (await PhysicalDps.Purify(MonkSettings.Instance)) return true;
            if (await PhysicalDps.Recuperate(MonkSettings.Instance)) return true;

            if (await Pvp.MeteodrivePvp()) return true;
            if (await Pvp.SixSidedStarPvp()) return true;

            if (await Pvp.EarthReplyPvp()) return true;
            if (await Pvp.RiddleofEarthPvp()) return true;
            if (await Pvp.RisingPhoenixPvp()) return true;

            if (await Pvp.ThunderclapPvp()) return true;
            if (await Pvp.EnlightenmentPvp()) return true;

            if (await Pvp.PhantomRushPvp()) return true;
            if (await Pvp.DemolishPvp()) return true;
            if (await Pvp.TwinSnakesPvp()) return true;
            if (await Pvp.DragonKickPvp()) return true;
            if (await Pvp.SnapPunchPvp()) return true;
            if (await Pvp.TrueStrikePvp()) return true;

            return (await Pvp.BootshinePvp());

        }
    }
}
