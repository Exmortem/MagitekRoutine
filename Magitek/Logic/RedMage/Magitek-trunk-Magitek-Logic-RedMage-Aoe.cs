using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
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
				if (BlackMana >= 90 && WhiteMana >= 90 && RedMageSettings.Instance.Embolden)
				{
					return await Spells.Embolden.Cast(Core.Me);
				}
				
				while (BlackMana >= 70 && WhiteMana >= 70)
				{
					return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
				}
				
				if (!Core.Me.HasAura(Auras.Manafication))
				{
					return await Spells.Manafication.Cast(Core.Me);
				}
				
				while (BlackMana >= 20 && WhiteMana >= 20)
				{
					return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
				}
				
				if (Spells.Swiftcast.Cooldown == TimeSpan.Zero && !Core.Me.HasAura(Auras.Swiftcast))
				{
					return await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast);
				}
				
				if (Core.Me.HasAura(Auras.Swiftcast)
				{
					return await Spells.Impact.Cast(Core.Me.CurrentTarget);
				}
			}
			
			if (Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach) == 3 && BlackMana >= 90 && WhiteMana >= 90)
				return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
			
            return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Veraero2()
        {
            if (WhiteMana > BlackMana)
                return false;

            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.Ver2Enemies)
                return false;

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

            return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);
        }
    }
}
