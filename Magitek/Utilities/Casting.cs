using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Models.Account;
using Magitek.Utilities.Managers;
using Magitek.Utilities.Routines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Debug = Magitek.ViewModels.Debug;

namespace Magitek.Utilities
{
    internal static class Casting
    {
        #region Variables
        public static bool CastingHeal;
        public static SpellData CastingSpell;
        public static SpellData LastSpell;
        public static bool LastSpellSucceeded;
        public static DateTime LastSpellTimeFinishedUtc;
        public static GameObject LastSpellTarget;
        public static GameObject SpellTarget;
        public static TimeSpan SpellCastTime;
        public static bool DoHealthChecks;
        public static bool NeedAura;
        public static uint Aura;
        public static bool UseRefreshTime;
        public static int RefreshTime;
        public static readonly Stopwatch CastingTime = new Stopwatch();
        //public static bool CastingTankBuster;
        public static bool CastingGambit;
        //public static GameObject LastTankBusterTarget;
        //public static DateTime LastTankBusterTime;
        //public static SpellData LastTankBusterSpell;
        public static List<SpellCastHistoryItem> SpellCastHistory = new List<SpellCastHistoryItem>();
        #endregion

        public static async Task<bool> TrackSpellCast()
        {
            // Manage SpellCastHistory entries
            if (SpellCastHistory.Count > 20)
            {
                SpellCastHistory.Remove(SpellCastHistory.Last());

                if (BaseSettings.Instance.DebugSpellCastHistory)
                    Application.Current.Dispatcher.Invoke(delegate { Debug.Instance.SpellCastHistory = new List<SpellCastHistoryItem>(SpellCastHistory); });
            }

            // If we're not casting we can return false to keep going down the tree
            if (!Core.Me.IsCasting)
                return false;

            // The possibility here is that we're teleporting (casting)
            // So if the timer isn't running, it means Magitek didn't cast it, and the cast shouldn't be monitored
            if (!CastingTime.IsRunning)
                return false;

            await GambitLogic.ToastGambits();

            #region Debug and Target Checks

            if (BaseSettings.Instance.DebugPlayerCasting)
            {
                Debug.Instance.CastingTime = CastingTime.ElapsedMilliseconds.ToString();
            }

            #endregion

            #region Interrupt Casting Checks
            if (CastingGambit)
                return true;

            if (!SpellTarget.IsTargetable)
            {
                await CancelCast("Target is no Longer Targetable");
            }

            if (!SpellTarget.IsValid)
            {
                await CancelCast("Target is no Longer Valid");
            }

            if (await GambitLogic.InterruptCast())
            {
                await CancelCast();
                return true;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.BlueMage:
                    {
                        if (BlueMage.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.Scholar:
                    {
                        if (Scholar.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.Arcanist:
                    {
                        if (Scholar.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.WhiteMage:
                    {
                        if (WhiteMage.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.Conjurer:
                    {
                        if (WhiteMage.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.Astrologian:
                    {
                        if (Astrologian.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.Summoner:
                    {
                        if (Summoner.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
                case ClassJobType.BlackMage:
                    {
                        if (BlackMage.NeedToInterruptCast())
                        {
                            await CancelCast();
                        }
                        break;
                    }
            }

            #endregion

            return true;
        }

        private static async Task CancelCast(string msg = null)
        {
            try
            {
                ActionManager.StopCasting();
                await Coroutine.Wait(1000, () => !Core.Me.IsCasting);

                if (msg != null)
                    Logger.Error(msg);

                CastingTime.Stop();
            }
            catch (Exception)
            {
                //Ignore on Purpose
            }
        }

        public static async Task CheckForSuccessfulCast()
        {
            // If the timer isn't running it means it's already been stopped and the variables have already been set
            if (!CastingTime.IsRunning)
            {
                //CastingTankBuster = false;
                NeedAura = false;
                UseRefreshTime = false;
                DoHealthChecks = false;
                CastingHeal = false;
                //CastingTankBuster = false;
                CastingGambit = false;
                return;
            }

            #region Verify Successful Spell Cast

            // Compare Times
            Logger.WriteCast($@"Time Casting: {CastingTime.ElapsedMilliseconds} - Expected: {SpellCastTime.TotalMilliseconds}");
            var buffer = SpellCastTime.TotalMilliseconds - CastingTime.ElapsedMilliseconds;

            // Stop Timer
            CastingTime.Stop();

            // Did we successfully cast?
            if (buffer > 800)
            {
                NeedAura = false;
                UseRefreshTime = false;
                DoHealthChecks = false;
                CastingHeal = false;
                //CastingTankBuster = false;
                CastingGambit = false;
                LastSpellSucceeded = false;
                return;
            }

            if (BaseSettings.Instance.DebugPlayerCasting)
            {
                Debug.Instance.CastingTime = CastingTime.ElapsedMilliseconds.ToString();
            }
            // Within 500 milliseconds we're gonna assume the spell went off
            LastSpell = CastingSpell;
            LastSpellSucceeded = true;
            Debug.Instance.LastSpell = LastSpell;
            LastSpellTimeFinishedUtc = DateTime.UtcNow;
            LastSpellTarget = SpellTarget;
            Logger.WriteCast($@"Successfully Casted {LastSpell}");

            SpellCastHistory.Insert(0, new SpellCastHistoryItem { Spell = LastSpell,
                                                                  SpellTarget = SpellTarget,
                                                                  TimeCastUtc = LastSpellTimeFinishedUtc,
                                                                  TimeStartedUtc = LastSpellTimeFinishedUtc.Subtract(TimeSpan.FromMilliseconds(CastingTime.ElapsedMilliseconds)),
                                                                  DelayMs = CastingTime.ElapsedMilliseconds - SpellCastTime.TotalMilliseconds });

            if (BaseSettings.Instance.DebugSpellCastHistory)
                Application.Current.Dispatcher.Invoke(delegate { Debug.Instance.SpellCastHistory = new List<SpellCastHistoryItem>(SpellCastHistory); });

            #endregion

            #region Aura Checks

            if (NeedAura)
            {
                if (UseRefreshTime)
                    await Coroutine.Wait(3000, () => SpellTarget.HasAura(Aura, true, RefreshTime) || MovementManager.IsMoving);
                else
                {
                    await Coroutine.Wait(3000, () => SpellTarget.HasAura(Aura, true) || MovementManager.IsMoving);
                }

                if (CastingSpell.AdjustedCastTime == TimeSpan.Zero)
                    await Coroutine.Wait(3000, () => SpellTarget.HasAura(Aura));
            }

            #endregion

            #region Fill Variables

            /*if (CastingTankBuster)
            {
                LastTankBusterTarget = SpellTarget;
                LastTankBusterSpell = CastingSpell;
                LastTankBusterTime = DateTime.Now;
            }*/

            NeedAura = false;
            UseRefreshTime = false;
            DoHealthChecks = false;
            CastingHeal = false;
            //CastingTankBuster = false;
            CastingGambit = false;

            #endregion
        }
    }

    public class SpellCastHistoryItem
    {
        public SpellData Spell { get; set; }
        public GameObject SpellTarget { get; set; }
        public DateTime TimeCastUtc { get; set; }
        public DateTime TimeStartedUtc { get; set; }
        public double DelayMs { get; set; }

        public int AnimationLockRemainingMs
        {
            get
            {
                double timeSinceStartMs = DateTime.UtcNow.Subtract(TimeStartedUtc).TotalMilliseconds - DelayMs;
                return timeSinceStartMs > Globals.AnimationLockMs ? 0 : Globals.AnimationLockMs - (int)timeSinceStartMs;
            }
        }
    }
}