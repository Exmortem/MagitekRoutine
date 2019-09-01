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

            if (BlackMana < 30 || WhiteMana < 30)
                return false;

            if (Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach) < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Veraero2()
        {
            if (WhiteMana > BlackMana)
                return false;

            if (RedMageSettings.Instance.SwiftcastVerthunderVeraero)
            {
                if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.CorpsACorps || s.Spell == Spells.Riposte))
                    return false;

                if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                    return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);

                if (await Spells.Swiftcast.Cast(Core.Me))
                {
                    await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                    await Coroutine.Wait(2000,
                        () => ActionManager.CanCast(Spells.Veraero2, Core.Me.CurrentTarget));
                    return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);

                }
            }

            return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verthunder2()
        {
            if (BlackMana > WhiteMana)
                return false;

            if (RedMageSettings.Instance.SwiftcastVerthunderVeraero)
            {
                if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.CorpsACorps || s.Spell == Spells.Riposte))
                    return false;

                if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                    return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);

                if (await Spells.Swiftcast.Cast(Core.Me))
                {
                    await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                    await Coroutine.Wait(2000,
                        () => ActionManager.CanCast(Spells.Verthunder2, Core.Me.CurrentTarget));
                    return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);

                }
            }
            
            return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);
        }
    }
}
