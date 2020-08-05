using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Models.QueueSpell;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

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

            if (!ActionManager.CanCast(Spells.Ten, null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Raiton });
            NinjaSettings.Instance.ForceRaiton = false;
            TogglesManager.ResetToggles();
            return true;
        }


        public static bool FumaShuriken()
        {
            if (!NinjaSettings.Instance.UseFumaShuriken)
                return false;

            if (!ActionManager.HasSpell(Spells.FumaShuriken.Id))
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000)
                return false;

            if (Core.Me.ClassLevel < 30)
                return false;

            if (Core.Me.ClassLevel > 76 && Core.Me.HasAura(Auras.Kassatsu))
                return false;

            // First Mudra of the line
            if (!ActionManager.CanCast(Spells.Ten, null))
                return false;

            if (Spells.TrickAttack.Cooldown.Seconds < 22)
                return false;

            // Basic checks
            if (!Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;



            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.FumaShuriken });
            return true;
        }

        public static bool Huton()
        {
            if (!NinjaSettings.Instance.UseHuton)
                return false;

            if (Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (!ActionManager.HasSpell(Spells.Huton.Id))
                return false;

            //if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000)
            //    return false;

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!ActionManager.CanCast(Spells.Jin, null))
                return false;

            if (ActionResourceManager.Ninja.HutonTimer.TotalMilliseconds > 1)
                return false;

            if (ActionManager.HasSpell(Spells.ArmorCrush.Id))
            {
                if (ActionManager.LastSpell == Spells.GustSlash)
                    return false;
            }

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Huton, TargetSelf = true });
            return true;
        }

        public static bool GokaMekkyaku()
        {

            if (!NinjaSettings.Instance.UseGokaMekkyaku)
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000)
                return false;

            if (!ActionManager.HasSpell(Spells.GokaMekkyaku.Id))
                return false;

            if (Core.Me.ClassLevel < 76)
                return false;

            if (!ActionManager.CanCast(Spells.Chi, null))
                return false;

            if (!Core.Me.HasAura(Auras.Kassatsu) && Casting.LastSpell != Spells.Kassatsu)
                return false;

            if (Core.Me.EnemiesNearby(8).Count() < NinjaSettings.Instance.GokaMekkyakuMinEnemies)
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.GokaMekkyaku });
            return true;
        }

        public static bool Doton()
        {
            if (!NinjaSettings.Instance.UseDoton)
                return false;

            if (Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (Core.Me.HasAura(Auras.Doton))
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000 && !NinjaSettings.Instance.UseForceNinjutsu)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (!ActionManager.HasSpell(Spells.Doton.Id))
                return false;

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!ActionManager.CanCast(Spells.Ten, null))
                return false;

            if (Core.Me.HasAura(Auras.Doton))
                return false;

            if (Core.Me.EnemiesNearby(8).Count() < NinjaSettings.Instance.DotonMinEnemies)
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
            if (!NinjaSettings.Instance.UseKaton)
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000)
                return false;

            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 15)
                return false;

            if (!ActionManager.HasSpell(Spells.Katon.Id))
                return false;

            if (Core.Me.ClassLevel < 35)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack) && !Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack, true, 2500))
                return false;

            if (!ActionManager.CanCast(Spells.Chi, null))
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5 + Core.Me.CurrentTarget.CombatReach).Count() < NinjaSettings.Instance.KatonMinEnemies)
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Katon });
            return true;
        }

        public static bool HyoshoRanryu()
        {
            if (!NinjaSettings.Instance.UseHyoshoRanryu)
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000)
                return false;

            if (!ActionManager.HasSpell(Spells.HyoshoRanryu.Id))
                return false;

            if (Core.Me.ClassLevel < 76)
                return false;

            if (!ActionManager.CanCast(Spells.Chi, null))
                return false;

            if (!Core.Me.HasAura(Auras.Kassatsu) && Casting.LastSpell != Spells.Kassatsu)
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.HyoshoRanryu });
            return true;
        }

        public static bool Raiton()
        {
            if (!NinjaSettings.Instance.UseRaiton)
                return false;

            if (Core.Me.ClassLevel > 76 && Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 22000 && !NinjaSettings.Instance.UseForceNinjutsu)
                return false;
           
            if (Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack) && !Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack, true, 2500))
                return false;
           
            if (!ActionManager.HasSpell(Spells.Chi.Id))
                return false;

            if (Core.Me.ClassLevel < 35)
                return false;
            //if we'd rather use katon, don't use raiton -Sage
            if (Core.Me.CurrentTarget.EnemiesNearby(5 + Core.Me.CurrentTarget.CombatReach).Count() > NinjaSettings.Instance.KatonMinEnemies && NinjaSettings.Instance.UseKaton)
                return false;

            if (Casting.SpellCastHistory.Take(5).All(s => s.Spell == Spells.Raiton) /*&& Spells.TenChiJin.Cooldown.TotalMilliseconds < 5000*/)
                return false;

            if (!ActionManager.CanCast(Spells.Ten, null))
                return false;

            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.Timeout.Start();
            SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 5000;
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, TargetSelf = true });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Raiton });
            return true;
        }

        public static bool Suiton()
        {
            if (Core.Me.HasAura(Auras.Suiton))
                return false;

            if (!NinjaSettings.Instance.UseTrickAttack)
                return false;

            if (!ActionManager.HasSpell(Spells.Suiton.Id))
                return false;

            if (Core.Me.ClassLevel > 76 && Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!ActionManager.CanCast(Spells.Ten, null))
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds > 20000)
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

        public static async Task<bool> TenChiJin()
        {
            if (!NinjaSettings.Instance.UseTenChiJin)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Suiton))
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 45000 && !Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
                return false;

            if (Spells.ShadowFang.Cooldown.TotalMilliseconds < 5000)
                return false;

            if (Spells.SpinningEdge.Cooldown.TotalMilliseconds < 1200)
                return false;

            if (Casting.LastSpell == Spells.Raiton)
                return false;

            if (Casting.LastSpell == Spells.Assassinate)
                return false;

            if (Casting.LastSpell == Spells.DreamWithinaDream || Spells.DreamWithinaDream.Cooldown.TotalMilliseconds < 2000)
                return false;

            //return await Spells.TenChiJin.Cast(Core.Me);

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
                SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 7000;
                SpellQueueLogic.CancelSpellQueue = () => MovementManager.IsMoving;
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, Wait = new QueueSpellWait() { Name = "Wait for Shuriken", Check = () => ActionManager.CanCast(Spells.Ten, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, Wait = new QueueSpellWait() { Name = "Wait for Raiton", Check = () => ActionManager.CanCast(Spells.Chi, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, Wait = new QueueSpellWait() { Name = "Wait for Suiton", Check = () => ActionManager.CanCast(Spells.Jin, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                Utilities.Routines.Ninja.TCJState = 0;
                return false;
            }

            if (Utilities.Routines.Ninja.TCJState == 2)
            {
                Logger.Error("Queuing TCJ");

                SpellQueueLogic.SpellQueue.Clear();
                SpellQueueLogic.Timeout.Start();
                SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 7000;
                SpellQueueLogic.CancelSpellQueue = () => MovementManager.IsMoving;
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ten, Wait = new QueueSpellWait() { Name = "Wait for Mudra1", Check = () => ActionManager.CanCast(Spells.Ten, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Jin, Wait = new QueueSpellWait() { Name = "Wait for Mudra2", Check = () => ActionManager.CanCast(Spells.Jin, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Chi, Wait = new QueueSpellWait() { Name = "Wait for Mudra3", Check = () => ActionManager.CanCast(Spells.Chi, null), WaitTime = 2000, EndQueueIfWaitFailed = true }, });
                Utilities.Routines.Ninja.TCJState = 0;
                return false;
            }

            return false;
        }
    }
}
