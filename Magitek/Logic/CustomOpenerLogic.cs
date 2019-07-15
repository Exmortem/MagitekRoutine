using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Gambits;
using Magitek.Gambits.Actions;
using Magitek.Models.Account;
using Magitek.Utilities;

namespace Magitek.Logic
{
    internal static class CustomOpenerLogic
    {
        public static List<OpenerGroup> OpenerGroups { get; set; } = new List<OpenerGroup>();
        public static bool InOpener;
        private static OpenerGroup _executingOpener = null;
        private static Queue<Gambit> _currentOpenerQueue = null;
        private static Gambit _executingGambit = null;
        private static Stopwatch GambitTimer { get; set; }

        internal static async Task<bool> Opener()
        {
            // Reset Openers from the overlay
            // User sets it to true, when it iterates through we set it back to false
            if (BaseSettings.Instance.ResetOpeners)
            {
                InOpener = false;
                _executingOpener = null;
                _executingGambit = null;
                BaseSettings.Instance.ResetOpeners = false;
                Logger.WriteInfo(@"Opener Reset");
                return true;
            }

            if (!BaseSettings.Instance.UseOpeners)
                return false;

            // Don't do openers with autonomous botbases
            if (BotManager.Current.IsAutonomous)
                return false;

            // If we're not already in the opener
            if (!InOpener)
            {
                if (!CheckForOpener())
                    return false;
            }
            else
            {
                // Verify that we really are already in an opener
                if (_executingOpener == null)
                {
                    InOpener = false;
                    return false;
                }

                if (_currentOpenerQueue == null)
                {
                    InOpener = false;
                    return false;
                }
            }

            InOpener = true;

            // Check to see if we died in the middle of an opener
            if (Core.Me.IsDead)
            {
                Logger.Error($@"Finished Opener [{_executingOpener.Name}] Because We Died");
                _executingGambit = null;
                InOpener = false;
                return true;
            }

            // At this point we are in an opener
            // If the current executing gambit is null, dequeue the next one from the queue
            if (_executingGambit == null)
            {
                _executingGambit = _currentOpenerQueue.Dequeue();

                // Start the timer
                GambitTimer = new Stopwatch();
                GambitTimer.Restart();
            }

            #region Check Conditions

            if (_executingGambit.Conditions.Any())
            {
                if (!await Coroutine.Wait(TimeSpan.FromMilliseconds(_executingGambit.MaxTimeToWaitForCondition), CheckConditions))
                {
                    Logger.WriteInfo($@"Opener [{_executingOpener.Name}] Conditions False For Action [{_executingGambit.Order}] [{_executingGambit.Title}]");
                    _executingGambit = null;

                    if (_currentOpenerQueue.Any())
                        return true;

                    Logger.WriteInfo($@"Finished Opener [{_executingOpener.Name}]");
                    InOpener = false;
                    return true;
                }
            }

            #endregion

            #region Execute the gambit

            if (!await _executingGambit.Execute())
            {
                if (GambitTimer.ElapsedMilliseconds > _executingGambit.MaxTimeToWaitForAction)
                {
                    if (_executingGambit.AbandonOpenerIfActionFail)
                    {
                        Logger.Error($@"Opener [{_executingOpener.Name}] Action [{_executingGambit.Order}][{_executingGambit.Title}] Timed Out");
                        Logger.WriteInfo($@"Finished Opener [{_executingOpener.Name}] Because Action [{_executingGambit.Title}] Failed");
                        _executingGambit = null;
                        InOpener = false;
                    }
                    else
                    {
                        Logger.Error($@"Opener [{_executingOpener.Name}] Action [{_executingGambit.Order}][{_executingGambit.Title}] Timed Out");
                        _executingGambit = null;

                        if (!_currentOpenerQueue.Any())
                        {
                            Logger.WriteInfo($@"Finished Opener [{_executingOpener.Name}]");
                            InOpener = false;
                            return true;
                        }
                    }                
                }
                
                return true;
            }

            Logger.WriteInfo($@"Opener [{_executingOpener.Name}] Action [{_executingGambit.Order}][{_executingGambit.Title}] Successful");
            _executingGambit = null;

            // Return true if there are any gambits left
            if (_currentOpenerQueue.Any())
                return true;

            Logger.WriteInfo($@"Finished Opener [{_executingOpener.Name}]");
            InOpener = false;
            return true;

            #endregion 
        }

        private static bool CheckForOpener()
        {
            _executingOpener = null;
            _executingGambit = null;

            // Look for an Opener that meets the opening conditions
            foreach (var opener in OpenerGroups)
            {
                if (opener.Gambits.Count == 0)
                    continue;

                // Check to see if we should only use the opener once per combat
                if (opener.OnlyUseOncePerCombat)
                {
                    // If we've been in combat for more than 25 seconds (some bosses put us in combat but don't let us attack them instantly - Demon Chadarnook o6s for example) then we find a different opener
                    if (Combat.CombatTime.ElapsedMilliseconds > 25000)
                        continue;
                }

                if (opener.StartOpenerConditions.Any(condition => !condition.Check()))
                    continue;

                _executingOpener = opener;
                break;
            }

            // Return false if there is no opener
            if (_executingOpener == null)
                return false;

            // We've past the loop and haven't returned false, so we're gonna start the opener
            _currentOpenerQueue = new Queue<Gambit>(_executingOpener.Gambits.OrderBy(r => r.Order));
            Logger.WriteInfo($@"Starting Opener [{_executingOpener.Name}]");
            return true;
        }

        private static bool CheckConditions()
        {
            if (!_executingGambit.Conditions.Any())
                return true;

            GameObject target = null;

            switch (_executingGambit.ActionType)
            {
                case GambitActionTypes.NoAction:
                    return true;

                case GambitActionTypes.CastSpellOnCurrentTarget:
                    if (_executingGambit.Conditions.Any(condition => !condition.Check(Core.Me.CurrentTarget)))
                        return false;

                    break;

                case GambitActionTypes.CastSpellOnSelf:
                    if (_executingGambit.Conditions.Any(condition => !condition.Check(Core.Me)))
                        return false;

                    break;

                case GambitActionTypes.CastSpellOnAlly:
                    target = Group.CastableAlliesWithin30.FirstOrDefault(x => _executingGambit.Conditions.All(z => z.Check(x)));

                    if (target == null)
                        return false;

                    break;

                case GambitActionTypes.CastSpellOnEnemy:
                    target = Combat.Enemies.FirstOrDefault(x => _executingGambit.Conditions.All(z => z.Check(x)));

                    if (target == null)
                        return false;

                    break;

                case GambitActionTypes.CastSpellOnFriendlyNpc:
                    target = GameObjectManager.GameObjects.FirstOrDefault(x => !x.CanAttack && x.Type != GameObjectType.Pc && _executingGambit.Conditions.All(z => z.Check(x)));

                    if (target == null)
                        return false;

                    break;

                case GambitActionTypes.UseItemOnSelf:
                case GambitActionTypes.ToastMessage:
                case GambitActionTypes.SleepForMilliseconds:
                    if (_executingGambit.Conditions.Any(condition => !condition.Check(Core.Me)))
                        return false;

                    break;

                default:
                    return true;
            }

            return true;
        }
    }
}
