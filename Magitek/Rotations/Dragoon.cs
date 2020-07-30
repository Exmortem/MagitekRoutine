using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Dragoon;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Dragoon
    {
        private static readonly Stopwatch JumpGcdTimer = new Stopwatch();

        public static async Task<bool> Rest()
        {
            return Core.Me.CurrentHealthPercent < DragoonSettings.Instance.RestHealthPercent;
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (Globals.InParty)
            {
                // If we're in a party and we die, but our group is still in combat, we don't want to reset the counter
                if (!Utilities.Combat.Enemies.Any())
                    Utilities.Routines.Dragoon.MirageDives = 0;
            }
            else
            {
                Utilities.Routines.Dragoon.MirageDives = 0;
            }

            Utilities.Routines.Dragoon.MirageDives = 0;



            await Casting.CheckForSuccessfulCast();

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 5 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {


            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();


            if (await GambitLogic.Gambit()) return true;
            return false;
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            if (Core.Me.CurrentTarget == null)
                return false;

            if (!Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            #region Positional Overlay
            if (BaseSettings.Instance.UsePositionalOverlay)
            {
                if (Core.Me.HasAura(Auras.SharperFangandClaw))
                {
                    if (!Core.Me.CurrentTarget.IsFlanking)
                    {
                        ViewModels.BaseSettings.Instance.PositionalStatus = "OutOfPosition";
                        ViewModels.BaseSettings.Instance.PositionalText = "Move To Flank";
                    }
                    else
                    {
                        ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
                        ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
                    }
                }

                if (Core.Me.HasAura(Auras.EnhancedWheelingThrust))
                {
                    if (!Core.Me.CurrentTarget.IsBehind)
                    {
                        ViewModels.BaseSettings.Instance.PositionalStatus = "OutOfPosition";
                        ViewModels.BaseSettings.Instance.PositionalText = "Move Behind";
                    }
                    else
                    {
                        ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
                        ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
                    }
                }

                if (ActionManager.LastSpell == Spells.Disembowel)
                {
                    if (!Core.Me.CurrentTarget.IsBehind)
                    {
                        ViewModels.BaseSettings.Instance.PositionalStatus = "OutOfPosition";
                        ViewModels.BaseSettings.Instance.PositionalText = "Move Behind";
                    }
                    else
                    {
                        ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
                        ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
                    }
                }
            }
            #endregion       

            #region Off GCD debugging
            if (Utilities.Routines.Dragoon.Jumps.Contains(Casting.LastSpell))
            {
                // Check to see if we're OFF GCD
                if (Spells.TrueThrust.Cooldown == TimeSpan.Zero)
                {
                    // Start the stopwatch if it isn't running
                    if (!JumpGcdTimer.IsRunning)
                        JumpGcdTimer.Restart();
                }
            }
            else
            {
                // We didn't use a Jump last, check to see if the stopwatch is running
                if (JumpGcdTimer.IsRunning)
                {
                    // We'll give a 50ms buffer for the time it takes to tick
                    if (JumpGcdTimer.ElapsedMilliseconds > 50)
                    {
                        Logger.WriteInfo($@"We wasted {JumpGcdTimer.ElapsedMilliseconds} ms off GCD");
                    }

                    // Stop the stopwatch
                    JumpGcdTimer.Stop();
                }
            }
            #endregion

            if (await PhysicalDps.Interrupt(DragoonSettings.Instance)) return true;
            if (await PhysicalDps.SecondWind(DragoonSettings.Instance)) return true;
            if (await PhysicalDps.Bloodbath(DragoonSettings.Instance)) return true;

            Utilities.Routines.Dragoon.EnemiesInView = Utilities.Combat.Enemies.Count(r => r.Distance(Core.Me) <= 10 + r.CombatReach && r.InView());

            if (Utilities.Routines.Dragoon.OnGcd && !_usedJumpDuringGcd)
            {
                if (await Jumps.MirageDive())
                {
                    _usedJumpDuringGcd = true;
                    return true;
                }

                if (await Jumps.Execute())
                {
                    _usedJumpDuringGcd = true;
                    return true;
                }

                if (await SingleTarget.Geirskogul()) return true;

                if (await Buff.LanceCharge()) return true;
                if (await Aoe.Nastrond()) return true;
                if (await Buff.BloodOfTheDragon()) return true;
                if (await Buff.DragonSight()) return true;
                if (await Buff.BattleLitany()) return true;
                return await Buff.TrueNorth();
            }

            if (await OffGlobalCooldownRotation())
            {
                _usedJumpDuringGcd = false;
                return true;
            }

            return false;
        }

        private static bool _usedJumpDuringGcd;

        private static async Task<bool> OffGlobalCooldownRotation()
        {
            if (await SingleTarget.WheelingThrust()) return true;
            if (await SingleTarget.FangAndClaw()) return true;

            if (DragoonSettings.Instance.Aoe && Core.Me.CurrentTarget.EnemiesNearby(8).Count() >= DragoonSettings.Instance.AoeEnemies && Core.Me.ClassLevel >= Spells.DoomSpike.LevelAcquired)
            {
                if (await Aoe.CoethanTorment()) return true;
                if (await Aoe.SonicThrust()) return true;
                return await Aoe.DoomSpike();
            }

            if (await SingleTarget.ChaosThrust()) return true;
            if (await SingleTarget.Disembowel()) return true;
            if (await SingleTarget.FullThrust()) return true;
            if (await SingleTarget.VorpalThrust()) return true;
            return await SingleTarget.TrueThrust();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
