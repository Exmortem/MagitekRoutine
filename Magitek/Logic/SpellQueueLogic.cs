using Buddy.Coroutines;
using Clio.Utilities.Collections;
using Magitek.Extensions;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Debug = Magitek.ViewModels.Debug;

namespace Magitek.Logic
{
    internal static class SpellQueueLogic
    {
        internal static Queue<QueueSpell> SpellQueue { get; } = new Queue<QueueSpell>();
        internal static bool InSpellQueue { get; set; }
        internal static bool NeedToDequeueSuccessfulCast { get; set; }
        internal static Stopwatch Timeout { get; } = new Stopwatch();
        internal static Func<bool> CancelSpellQueue { get; set; }

        public static async Task<bool> SpellQueueMethod()
        {
            if (!InSpellQueue)
            {
                Logger.WriteInfo("Starting Spell Queue");
                Casting.LastSpell = null;
            }

            Debug.Instance.Queue = new AsyncObservableCollection<QueueSpell>(SpellQueue);
            InSpellQueue = true;

            if (CancelSpellQueue.Invoke())
            {
                SpellQueue.Clear();
                Timeout.Reset();
                Logger.WriteInfo("Had To Cancel Spell Queue");
                InSpellQueue = false;
                return true;
            }

            var spell = SpellQueue.Peek();

            if (NeedToDequeueSuccessfulCast)
            {
                if (Casting.LastSpell != null && Casting.LastSpell.Id == spell.Spell.Id)
                {
                    SpellQueue.Dequeue();
                    NeedToDequeueSuccessfulCast = false;
                    if (!SpellQueue.Any())
                    {
                        Logger.WriteInfo("Spell Queue Complete");
                        Timeout.Reset();
                        InSpellQueue = false;
                    }
                    return true;
                }
            }

            if (spell.SleepBefore)
            {
                await Coroutine.Sleep(spell.SleepMilliseconds);
            }

            if (spell.Wait != null)
            {
                if (await Coroutine.Wait(spell.Wait.WaitTime, spell.Wait.Check))
                {
                    Logger.Write($"Spell Queue Wait: {spell.Wait.Name}");
                }
                else
                {
                    SpellQueue.Dequeue();
                    return true;
                }
            }

            if (spell.Checks.Any(x => !x.Check.Invoke()))
            {
                SpellQueue.Dequeue();

                Logger.Write($"Removing {spell.Spell.LocalizedName} From The Spell Queue Because It Failed It's Checks.");

                foreach (var check in spell.Checks.Where(x => !x.Check.Invoke()))
                {
                    if (!check.SilentMode) Logger.Write($"Failed Check: {check.Name}");
                }

                return true;
            }

            var target = spell.Target();

            if (target == null)
                return SpellQueue.Any();

            if (!await spell.Spell.Cast(target))
                return SpellQueue.Any();



            Logger.WriteInfo($@"Queue Cast: {spell.Spell.LocalizedName}");

            NeedToDequeueSuccessfulCast = true;
            return SpellQueue.Any();
        }
    }
}
