using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities.Collections;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;
using Magitek.ViewModels;

namespace Magitek.Logic
{
    internal static class OpenerLogic
    {
        internal static Queue<QueueSpell> OpenerQueue { get; } = new Queue<QueueSpell>();
        internal static bool InOpener { get; set; }
        internal static bool NeedToDequeueSuccessfulCast { get; set; }
        internal static Func<bool> CancelOpener { get; set; }

        internal static bool CanStartOpenerBase
        {
            get
            {
                if (!OpenerQueue.Any())
                    return false;

                if (Core.Me.ClassLevel < 70)
                    return false;

                if (BotManager.Current.IsAutonomous)
                    return false;

                if (Core.Me.InCombat)
                {
                    if (Combat.CombatTime.ElapsedMilliseconds > 25000)
                        return false;
                }

                return true;
            }
        }
        
        internal static async Task<bool> Opener()
        {
            if (!InOpener)
            {
                Logger.WriteInfo("Starting Opener");
                Casting.LastSpell = null;
            }

            Debug.Instance.Queue = new AsyncObservableCollection<QueueSpell>(OpenerQueue);
            InOpener = true;

            var spell = OpenerQueue.Peek();

            if (NeedToDequeueSuccessfulCast)
            {
                if (Casting.LastSpell != null && Casting.LastSpell.Id == spell.Spell.Id)
                {
                    OpenerQueue.Dequeue();
                    NeedToDequeueSuccessfulCast = false;
                    if (!OpenerQueue.Any())
                    {
                        Logger.WriteInfo("Opener Complete");
                        InOpener = false;
                    }
                    return true;
                }
            }

            if (CancelOpener.Invoke())
            {
                OpenerQueue.Clear();
                Logger.WriteInfo("Had To Cancel Opener");
                InOpener = false;
                return true;
            }

            if (spell.Wait != null)
            {
                if (await Coroutine.Wait(spell.Wait.WaitTime, spell.Wait.Check))
                {
                    Logger.Write($"Opener Wait: {spell.Wait.Name}");
                }
                else
                {
                    if (spell.Wait.EndQueueIfWaitFailed)
                    {
                        OpenerQueue.Clear();
                        Logger.WriteInfo("Had To Cancel Opener");
                        InOpener = false;
                        return true;
                    }
                    else
                    {
                        Logger.Write($"Removing {spell.Spell.LocalizedName} From The Opener Because It Failed A Wait.");
                        OpenerQueue.Dequeue();
                        return true;
                    }
                }
            }

            // New ?
            foreach (var check in spell.Checks)
            {
                if (check.Check.Invoke())
                    continue;

                if (check.EndQueueIfCheckFailed)
                {
                    OpenerQueue.Clear();

                    if (!check.SilentMode)
                        Logger.WriteInfo($@"Had To Cancel Opener: {check.Name}");

                    InOpener = false;
                }
                else
                {
                    if (!check.SilentMode)
                        Logger.Write($"Removing {spell.Spell.LocalizedName} From The Opener Because It Failed It's Checks.");

                    OpenerQueue.Dequeue();
                    return true;
                }
            }

            if (spell.UsePotion)
            {
                await Core.Me.UseItem(spell.PotionId);
                NeedToDequeueSuccessfulCast = true;
                return OpenerQueue.Any();
            }

            var target = spell.Target();

            if (target == null)
                return OpenerQueue.Any();

            if (!await spell.Spell.Cast(target))
                return OpenerQueue.Any();

            NeedToDequeueSuccessfulCast = true;
            return OpenerQueue.Any();
        }
    }
}