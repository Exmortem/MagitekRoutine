using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Models.QueueSpell;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class Aoe
    {
        public static async Task<bool> Scatter()
        {
            if (!RedMageSettings.Instance.Scatter)
                return false;

            if (!Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.ScatterEnemies)
                return false;

            else
                return await Spells.Scatter.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ContreSixte()
        {
            if (!RedMageSettings.Instance.UseContreSixte)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.ContreSixteEnemies)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            else
                return await Spells.ContreSixte.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Moulinet()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (!RedMageSettings.Instance.Moulinet)
                return false;

            if (Core.Me.ClassLevel < Spells.Moulinet.LevelAcquired)
                return false;
                
            if (BlackMana < 20 || WhiteMana < 20)
                return false;

            //Use the dualcast before Moulinet
            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            int enemiesInRange = Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach);

            if (enemiesInRange < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            //There are three conditions under which to do a big AoE Burst:
            //  1. Mana is maxed and Embolden is up, in which case fire off Embolden and then Moulinet 5x
            //  2. Mana is >90 and both Embolden and Manafication are up, in which case fire off Embolden, Moulinet 2x, Manafication, and then Moulinet 5x
            //  3. Mana is >50 (but not too much) and Manafication is up, in which case fire off Manafication, Embolden if available, and then Moulinet 5x
            bool burstEmboldenAt100 = 
                   BlackMana == 100
                && WhiteMana == 100
                && Spells.Embolden.Cooldown == TimeSpan.Zero
                && RedMageSettings.Instance.Embolden
                && Core.Me.ClassLevel >= Spells.Embolden.LevelAcquired;

            bool burstEmboldenAndManaficationAt90 =
                   BlackMana >= 90
                && WhiteMana >= 90
                && Spells.Manafication.Cooldown == TimeSpan.Zero
                && Spells.Embolden.Cooldown == TimeSpan.Zero
                && RedMageSettings.Instance.Manafication
                && RedMageSettings.Instance.Embolden
                && Core.Me.ClassLevel >= Spells.Manafication.LevelAcquired;

            bool burstManaficationAt50 =
                   BlackMana >= 50
                && WhiteMana >= 50
                && Spells.Manafication.Cooldown == TimeSpan.Zero
                && RedMageSettings.Instance.Manafication
                && Core.Me.ClassLevel >= Spells.Manafication.LevelAcquired;

            //No embolden yet, so use Moulinet if there are three enemies, or to burn mana if there are only 2
            if (Core.Me.ClassLevel < Spells.Embolden.LevelAcquired)
            {
                if (enemiesInRange >= 3)
                {
                    Logger.WriteInfo($"Using one Moulinet because no buffs available at this level ({enemiesInRange} enemies)");
                    return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
                }
                else if (enemiesInRange == 2 && BlackMana >= 90 && WhiteMana >= 90)
                {
                    Logger.WriteInfo($"Burning one Moulinet to use excess mana ({enemiesInRange} enemies)");
                    return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
                }
            }
            else if (   enemiesInRange >= 3
                     && (   burstEmboldenAt100
                         || burstEmboldenAndManaficationAt90
                         || burstManaficationAt50)) //TODO: Should we delay this if embolden also up, or will be up soon?
            {
                if (burstEmboldenAndManaficationAt90)
                {
                    Logger.WriteInfo($"Bursting Moulinet 7x with Embolden and Manafication ({enemiesInRange} enemies)");
                    SpellQueueLogic.SpellQueueReset(() => SpellQueueLogic.Timeout.ElapsedMilliseconds > 18000);
                    Enqueue7MoulinetWithEmboldenAndManafication();
                }
                else if (burstEmboldenAt100)
                {
                    Logger.WriteInfo($"Bursting Moulinet with Embolden ({enemiesInRange} enemies)");
                    SpellQueueLogic.SpellQueueReset(() => SpellQueueLogic.Timeout.ElapsedMilliseconds > 18000);
                    EnqueueMoulinetWithEmbolden();
                }
                else //ManaficationAt50
                {
                    //If we'd be wasting some mana, Moulinet 1x and come around again
                    if (BlackMana > 60 && WhiteMana > 60)
                    {
                        Logger.WriteInfo($"Burning one Moulinet to prepare for Manafication ({enemiesInRange} enemies)");
                        return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
                    }
                    else
                    {
                        SpellQueueLogic.SpellQueueReset(() => SpellQueueLogic.Timeout.ElapsedMilliseconds > 18000);
                        //Use Embolden if available
                        if (Spells.Embolden.Cooldown == TimeSpan.Zero)
                        {
                            Logger.WriteInfo($"Bursting Moulinet 5x with Embolden and Manafication ({enemiesInRange} enemies)");
                            Enqueue5MoulinetWithEmboldenAndManafication();
                        }
                        else
                        {
                            Logger.WriteInfo($"Bursting Moulinet with Manafication ({enemiesInRange} enemies)");
                            EnqueueMoulinetWithManafication();
                        }
                    }
                }

                if (Spells.Swiftcast.Cooldown == TimeSpan.Zero && !Core.Me.HasAura(Auras.Swiftcast))
                {
                    SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Swiftcast, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Swiftcast", Check = () => ActionManager.CanCast(Spells.Swiftcast, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
                    SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Impact, Wait = new QueueSpellWait() { Name = "Wait for Impact", Check = () => ActionManager.CanCast(Spells.Impact, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
                }

                return true;
            }
            //Only two enemies in range - just use Moulinet to prevent wasting mana
            else if (BlackMana >= 90 && WhiteMana >= 90 && enemiesInRange == 2)
            {
                Logger.WriteInfo($"Burning one Moulinet to use excess mana ({enemiesInRange} enemies)");
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
            }
            //Use a Moulinet so we don't waste mana while waiting for buffs to pop
            else if (   BlackMana >= 90 && WhiteMana >= 90
                     && enemiesInRange >= 3
                     && Spells.Embolden.Cooldown != TimeSpan.Zero)
            {
                Logger.WriteInfo($"Burning one Moulinet to use excess mana ({enemiesInRange} enemies)");
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
            }
            //Use a Moulinet immediately if any of our buffs are still up outside the burst
            else if (   enemiesInRange >= 3
                     && (Core.Me.HasAura(Auras.Manafication) || Core.Me.Auras.Any(aura => aura.Name == "Embolden")))
            {
                Logger.WriteInfo($"Using one Moulinet because buffs are up ({enemiesInRange} enemies)");
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        private static void Enqueue7MoulinetWithEmboldenAndManafication()
        {
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Embolden, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Embolden", Check = () => ActionManager.CanCast(Spells.Embolden, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Pre-Manafication Moulinet 1", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Pre-Manafication Moulinet 2", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            EnqueueMoulinetWithManafication();
        }

        private static void Enqueue5MoulinetWithEmboldenAndManafication()
        {
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Embolden, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Embolden", Check = () => ActionManager.CanCast(Spells.Embolden, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            EnqueueMoulinetWithManafication();
        }

        private static void EnqueueMoulinetWithEmbolden()
        {
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Embolden, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Embolden", Check = () => ActionManager.CanCast(Spells.Embolden, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 1", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 2", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 3", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 4", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 5", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
        }

        private static void EnqueueMoulinetWithManafication()
        {
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Manafication, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Manafication", Check = () => ActionManager.CanCast(Spells.Manafication, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 1", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 2", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 3", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 4", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet 5", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 2500, EndQueueIfWaitFailed = true }, });
        }

        public static async Task<bool> Veraero2()
        {
            if (WhiteMana > BlackMana)
                return false;

            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.Ver2Enemies)
                return false;

            else
                return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verthunder2()
        {
            if (BlackMana > WhiteMana)
                return false;

            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.Ver2Enemies)
                return false;

            else
                return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);
        }
    }
}
