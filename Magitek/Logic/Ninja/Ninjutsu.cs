using Buddy.Coroutines;
using Clio.Utilities.Helpers;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Models.QueueSpell;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class Ninjutsu
    {
        public static bool ForceRaiton()
        {
            if (!NinjaSettings.Instance.ForceRaiton)
                return false;

            if (!ActionManager.HasSpell(Spells.Chi.Id))
                return false;

            if (Core.Me.ClassLevel < 35)
                return false;

            if (Casting.SpellCastHistory.Take(5).All(s => s.Spell == Spells.Raiton) /*&& Spells.TenChiJin.Cooldown.TotalMilliseconds < 5000*/)
                return false;

            if (!SpellDataExtensions.CanCast(Spells.Ten, null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Raiton });
            if (Spells.ForkedRaiju.IsKnown())
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.ForkedRaiju });
            NinjaSettings.Instance.ForceRaiton = false;
            TogglesManager.ResetToggles();
            return true;
        }

        #region BUFF

        public static bool Huton()
        {
            if (Core.Me.ClassLevel < 45)
                return false;

            if (!NinjaSettings.Instance.UseHuton)
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (HutonTimer.TotalMilliseconds != 0)
                return false;

            if (!Spells.Jin.CanCast(null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Huton, TargetSelf = true });
            return true;
        }

        public static bool Suiton()
        {
            if (Core.Me.ClassLevel < 45)
                return false;

            if (!NinjaSettings.Instance.UseTrickAttack)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Spells.TrickAttack.IsKnownAndReady(20000))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= NinjaSettings.Instance.AoeEnemies && NinjaSettings.Instance.DoNotUseTrickAttackDuringAoe)
                return false;

            if (!Spells.Ten.CanCast(null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Suiton });
            return true;
        }

        #endregion

        #region Single Target

        public static bool FumaShuriken()
        {
            if (Core.Me.ClassLevel < 30)
                return false;

            if (!NinjaSettings.Instance.UseFumaShuriken)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Spells.Ten.CanCast(null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.FumaShuriken });
            return true;
        }

        public static bool Raiton()
        {
            if (Core.Me.ClassLevel < 35)
                return false;

            if (!NinjaSettings.Instance.UseRaiton)
                return false;

            if (ActionManager.LastSpell == Spells.Raiton)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Spells.Ten.CanCast(null))
                return false;

            Logger.Write($@"[Magitek] Raiton : {SpellQueueLogic.InSpellQueue}");

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Raiton });
            return true;
        }

        public static bool HyoshoRanryu()
        {
            if (Core.Me.ClassLevel < 76)
                return false;

            if (!NinjaSettings.Instance.UseHyoshoRanryu)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra))
                return false;

            if(!Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.TrickAttack))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.HyoshoRanryu });
            return true;
        }

        #endregion

        #region AOE

        public static bool Doton()
        {
            if (Core.Me.ClassLevel < 45)
                return false;

            if (!NinjaSettings.Instance.UseAoe || !NinjaSettings.Instance.UseDoton)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < NinjaSettings.Instance.AoeEnemies)
                return false;

            if (Core.Me.HasAura(Auras.Doton) || Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Spells.Ten.CanCast(null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Doton, TargetSelf = true });
            return true;
        }

        public static bool Katon()
        {
            if (Core.Me.ClassLevel < 35)
                return false;

            if (!NinjaSettings.Instance.UseAoe || !NinjaSettings.Instance.UseKaton)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < NinjaSettings.Instance.AoeEnemies)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Spells.Chi.CanCast(null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Katon });
            return true;
        }

        public static bool GokaMekkyaku()
        {
            if (Core.Me.ClassLevel < 76)
                return false;

            if (!NinjaSettings.Instance.UseGokaMekkyaku)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < NinjaSettings.Instance.AoeEnemies)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Mudra))
                return false;

            if(!Core.Me.HasAura(Auras.Kassatsu))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.GokaMekkyaku });
            return true;
        }

        #endregion

        #region TCJ

        public static async Task<bool> TenChiJin()
        {
            if (Core.Me.ClassLevel < 70)
                return false;

            if (!NinjaSettings.Instance.UseTenChiJin)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Suiton) || Core.Me.HasAura(Auras.Mudra) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.TrickAttack))
                return false;

            if (!await Spells.TenChiJin.Cast(Core.Me))
                return false;

            if (!await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.TenChiJin)))
                return false;

            if (Utilities.Routines.Ninja.AoeEnemies5Yards > 1 && Utilities.Routines.Ninja.TCJState == 0 && !Core.Me.HasAura(Auras.Doton))
            {
                Utilities.Routines.Ninja.TCJState = 1;
            }

            if (Utilities.Routines.Ninja.AoeEnemies5Yards < 2 && Utilities.Routines.Ninja.TCJState == 0)
            {
                Utilities.Routines.Ninja.TCJState = 1;
            }

            Logger.Error("State is: " + Utilities.Routines.Ninja.TCJState);

            if (Utilities.Routines.Ninja.TCJState == 1)
            {
                Logger.Error("Queuing TCJ");

                SpellQueueLogic.SpellQueue.Clear();
                SpellQueueLogic.Timeout.Start();
                SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
                SpellQueueLogic.CancelSpellQueue = () => MovementManager.IsMoving;
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, Wait = new QueueSpellWait() { Name = "Wait for Shuriken", Check = () => SpellDataExtensions.CanCast(Spells.Ten, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, Wait = new QueueSpellWait() { Name = "Wait for Raiton", Check = () => SpellDataExtensions.CanCast(Spells.Chi, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, Wait = new QueueSpellWait() { Name = "Wait for Suiton", Check = () => SpellDataExtensions.CanCast(Spells.Jin, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                Utilities.Routines.Ninja.TCJState = 0;
                return false;
            }

            if (Utilities.Routines.Ninja.TCJState == 2)
            {
                Logger.Error("Queuing TCJ");

                SpellQueueLogic.SpellQueue.Clear();
                SpellQueueLogic.Timeout.Start();
                SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
                SpellQueueLogic.CancelSpellQueue = () => MovementManager.IsMoving;
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, Wait = new QueueSpellWait() { Name = "Wait for Mudra1", Check = () => SpellDataExtensions.CanCast(Spells.Ten, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, Wait = new QueueSpellWait() { Name = "Wait for Mudra2", Check = () => SpellDataExtensions.CanCast(Spells.Jin, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, Wait = new QueueSpellWait() { Name = "Wait for Mudra3", Check = () => SpellDataExtensions.CanCast(Spells.Chi, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                Utilities.Routines.Ninja.TCJState = 0;
                return false;
            }
            return false;
        }

        #endregion

    }
}
