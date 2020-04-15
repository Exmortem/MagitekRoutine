using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
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

      if (BlackMana < 20 || WhiteMana < 20)
                return false;
			
			if (Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach) < RedMageSettings.Instance.MoulinetEnemies)
                return false;
			
      if (Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach) >= 4 && BlackMana >= 50 && WhiteMana >= 50 && Spells.Manafication.Cooldown == TimeSpan.Zero && RedMageSettings.Instance.Manafication)
      {
                Logger.Error("Bursting Moulinet");

                SpellQueueLogic.SpellQueue.Clear();
                SpellQueueLogic.Timeout.Start();
                SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 18000;
        
				        if (BlackMana >= 90 && WhiteMana >= 90 && RedMageSettings.Instance.Embolden)
				        {
					              SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Embolden, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Embolden", Check = () => ActionManager.CanCast(Spells.Embolden, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
                }
        
				        while (BlackMana >= 70 && WhiteMana >= 70)
				        {
             					SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
         				}
				        if (!Core.Me.HasAura(Auras.Manafication))
				        {
                      SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Manafication, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Manafication", Check = () => ActionManager.CanCast(Spells.Manafication, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
        				}	
				        while (BlackMana >= 20 && WhiteMana >= 20)
				        {
             					SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
          			}
				
				        if (Spells.Swiftcast.Cooldown == TimeSpan.Zero && !Core.Me.HasAura(Auras.Swiftcast))
				        {
                      SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Swiftcast, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for Swiftcast", Check = () => ActionManager.CanCast(Spells.Swiftcast, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
				        }
								if (Core.Me.HasAura(Auras.Swiftcast))
				        {
					            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Impact, Wait = new QueueSpellWait() { Name = "Wait for Impact", Check = () => ActionManager.CanCast(Spells.Impact, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
				        }		
				        else
                  		return false;
      }
			
			if (Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach) == 3 && BlackMana >= 90 && WhiteMana >= 90)
			{
				SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Moulinet, Wait = new QueueSpellWait() { Name = "Wait for Moulinet", Check = () => ActionManager.CanCast(Spells.Moulinet, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
			}
            
			else
				return false;
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
