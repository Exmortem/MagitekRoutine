using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Magitek.Gambits;
using Magitek.Models.Account;
using Magitek.Utilities;

namespace Magitek.Logic
{
    internal static class GambitLogic
    {
        private static Queue<Gambit> GambitQueue { get; set; } = new Queue<Gambit>();
        private static Queue<Gambit> InterruptGambitQueue { get; set; } = new Queue<Gambit>();
        private static Queue<Gambit> ToastGambitQueue { get; set; } = new Queue<Gambit>();

        public static Queue<Gambit> StaticGambitQueue { get; set; } = new Queue<Gambit>();
        public static Queue<Gambit> StaticInterruptGambitQueue { get; set; } = new Queue<Gambit>();
        public static Queue<Gambit> StaticToastGambitQueue { get; set; } = new Queue<Gambit>();
        private static readonly Dictionary<int, DateTime> ForbiddenGambits = new Dictionary<int, DateTime>();
        private static Stopwatch SleepTimer { get; } = new Stopwatch();

        internal static async Task<bool> Gambit()
        {
            if (!BaseSettings.Instance.AllowGambits)
                return false;

            if (StaticGambitQueue == null || StaticGambitQueue.Count == 0)
                return false;

            // Remove any forbidden gambits that have expired
            if (ForbiddenGambits.Any())
            {
                try
                {
                    foreach (var gambit in ForbiddenGambits)
                    {
                        if (gambit.Value < DateTime.Now)
                            ForbiddenGambits.Remove(gambit.Key);
                    }
                }
                catch (Exception e)
                {
                    // Ignore that shit
                }
            }

            if (!GambitQueue.Any())
            {
                GambitQueue = new Queue<Gambit>(StaticGambitQueue);
            }

            while (GambitQueue.Any())
            {
                var currentGambit = GambitQueue.Dequeue();

                if (currentGambit.OnlyUseInChain)
                {
                    continue;
                }

                // return true if the gambit is currently forbidden to execute again
                if (ForbiddenGambits.ContainsKey(currentGambit.Id))
                    continue;

                var currentGambitExecutionStatus = await currentGambit.Execute();

                if (!currentGambitExecutionStatus)
                    continue;

                if (currentGambitExecutionStatus)
                {
                    Logger.WriteInfo($"Executed Gambit: {currentGambit.Title}");

                    // make the current gambit forbidden if it has a timer
                    if (currentGambit.PreventSameActionForTheNextMilliseconds > 0)
                    {
                        var timeExpired = DateTime.Now.AddMilliseconds(currentGambit.PreventSameActionForTheNextMilliseconds);
                        ForbiddenGambits.Add(currentGambit.Id, timeExpired);
                    }
                }

                if (currentGambitExecutionStatus && currentGambit.HasChain)
                {
                    // Start Chain
                    Logger.WriteInfo($"Starting Gambit Chain: {currentGambit.Title}");

                    while (true)
                    {
                        // Get the gambit with the correct title
                        var nextGambit = GambitQueue.FirstOrDefault(x => x.Title == currentGambit.ChainTitle);

                        // Stop chain if the gambit doesn't exist
                        if (nextGambit == null)
                        {
                            Logger.WriteInfo($"Finished Gambit Chain");
                            break;
                        }

                        // Set the currentGambit to the gambit
                        currentGambit = nextGambit;

                        // If we're forcing chain actions
                        if (currentGambit.ForceChainActions)
                        {
                            SleepTimer.Restart();

                            // As long as the gambit did not execute
                            while (!await currentGambit.Execute())
                            {
                                // Stop the chain if we've exceeded the time allowed
                                if (SleepTimer.Elapsed.TotalMilliseconds >= currentGambit.ForceChainSleepMilliseconds)
                                {
                                    SleepTimer.Stop();
                                    Logger.WriteInfo($"Gambit: {currentGambit.Title} Timed Out. Stopping Chain");
                                    break;
                                }

                                // Keep trying to execute
                                await Coroutine.Yield();
                            }

                            // Stop the timer since we executed the gambit
                            SleepTimer.Stop();
                            Logger.WriteInfo($"Executed Gambit Chain: {currentGambit.Title}");
                        }

                        // If we're not forcing chain actions
                        else
                        {
                            if (!await currentGambit.Execute())
                            {
                                Logger.WriteInfo($"Gambit: {currentGambit.Title} failed to execute. Stopping Chain [Not Forcing]");
                                break;
                            }
                            Logger.WriteInfo($"Executed Gambit Chain: {currentGambit.Title}");
                        }

                        await Coroutine.Yield();
                    }
                }

                if (currentGambitExecutionStatus)
                {
                    GambitQueue = new Queue<Gambit>(StaticGambitQueue);
                    return true;
                }

                await Coroutine.Yield();
            }

            return false;
        }

        internal static async Task<bool> ToastGambits()
        {
            if (!BaseSettings.Instance.AllowGambits)
                return false;

            if (!StaticToastGambitQueue.Any())
                return false;

            // Remove any forbidden gambits that have expired
            if (ForbiddenGambits.Any())
            {
                try
                {
                    foreach (var gambit in ForbiddenGambits)
                    {
                        if (gambit.Value < DateTime.Now)
                            ForbiddenGambits.Remove(gambit.Key);
                    }
                }
                catch (Exception e)
                {
                    // Ignore that shit
                }
            }

            if (!ToastGambitQueue.Any())
            {
                ToastGambitQueue = new Queue<Gambit>(StaticToastGambitQueue);
            }

            while (ToastGambitQueue.Any())
            {
                var currentGambit = ToastGambitQueue.Dequeue();

                // return true if the gambit is currently forbidden to execute again
                if (ForbiddenGambits.ContainsKey(currentGambit.Id))
                    continue;

                var currentGambitExecutionStatus = await currentGambit.Execute();

                if (currentGambitExecutionStatus)
                {
                    Logger.WriteInfo($"Executed Gambit: {currentGambit.Title}");

                    // make the current gambit forbidden if it has a timer
                    if (currentGambit.PreventSameActionForTheNextMilliseconds > 0)
                    {
                        var timeExpired = DateTime.Now.AddMilliseconds(currentGambit.PreventSameActionForTheNextMilliseconds);
                        ForbiddenGambits.Add(currentGambit.Id, timeExpired);
                    }

                    ToastGambitQueue = new Queue<Gambit>(StaticToastGambitQueue);
                    return true;
                }
            }

            return false;
        }

        internal static async Task<bool> InterruptCast()
        {
            if (!BaseSettings.Instance.AllowGambits)
                return false;

            if (!StaticInterruptGambitQueue.Any())
                return false;

            // Remove any forbidden gambits that have expired
            if (ForbiddenGambits.Any())
            {
                try
                {
                    foreach (var gambit in ForbiddenGambits)
                    {
                        if (gambit.Value < DateTime.Now)
                            ForbiddenGambits.Remove(gambit.Key);
                    }
                }
                catch (Exception e)
                {
                    // Ignore that shit
                }
            }

            if (!InterruptGambitQueue.Any())
            {
                InterruptGambitQueue = new Queue<Gambit>(StaticInterruptGambitQueue);
            }

            while (InterruptGambitQueue.Any())
            {
                var currentGambit = InterruptGambitQueue.Dequeue();               

                // return true if the gambit is currently forbidden to execute again
                if (ForbiddenGambits.ContainsKey(currentGambit.Id))
                    continue;

                if (currentGambit.Conditions.All(x => x.Check()))
                {
                    Logger.WriteInfo($"Canceling Cast To Execute Gambit: {currentGambit.Title}");
                    InterruptGambitQueue = new Queue<Gambit>(StaticInterruptGambitQueue);
                    return true;
                }            

                await Coroutine.Yield();
            }

            return false;
        }
    }
}