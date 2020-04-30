using ff14bot;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class Aoe
    {
        public static async Task<bool> Scatter()
        {
            if (!RedMageSettings.Instance.Scatter)
                return false;

            if (Utilities.Routines.RedMage.AoeEnemies5Yards < RedMageSettings.Instance.ScatterEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.Dualcast)
                || (!Core.Me.HasAura(Auras.Swiftcast)))
                return false;

            if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Moulinet)
                && ((BlackMana < 20) || (WhiteMana < 20))
                && (Utilities.Routines.RedMage.EnemiesInCone >= 4)
                && (Spells.Swiftcast.Cooldown == TimeSpan.Zero))
                await Buff.Swiftcast();

            return await Spells.Scatter.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> ContreSixte()
        {
            if (!RedMageSettings.Instance.UseContreSixte)
                return false;

            if (Utilities.Routines.RedMage.AoeEnemies6Yards < RedMageSettings.Instance.ContreSixteEnemies)
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

            if (Utilities.Routines.RedMage.EnemiesInCone < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            if (BlackMana < 20 || WhiteMana < 20)
                return false;

            if ((BlackMana < 90 || WhiteMana < 90)
                && (Utilities.Routines.RedMage.EnemiesInCone == 3))
                return false;

            return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Veraero2()
        {
            if (!RedMageSettings.Instance.Ver2)
                return false;

            if ((Math.Abs(WhiteMana - BlackMana) > 23) && (WhiteMana > BlackMana))
                return false;

            if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Moulinet)
                && ((BlackMana >= 20) && (WhiteMana >= 20))
                && (Utilities.Routines.RedMage.EnemiesInCone >= 4))
                return false;

            if (Utilities.Routines.RedMage.AoeEnemies5Yards < RedMageSettings.Instance.Ver2Enemies)
                return false;

            if (WhiteMana > BlackMana)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast) || Core.Me.HasAura(Auras.Swiftcast))
                return false;

            return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verthunder2()
        {
            if (!RedMageSettings.Instance.Ver2)
                return false;

            if ((Math.Abs(WhiteMana - BlackMana) > 23) && (WhiteMana < BlackMana))
                return false;

            if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Moulinet)
                && ((BlackMana >= 20) && (WhiteMana >= 20))
                && (Utilities.Routines.RedMage.EnemiesInCone >= 4))
                return false;

            if (Utilities.Routines.RedMage.AoeEnemies5Yards < RedMageSettings.Instance.Ver2Enemies)
                return false;

            if (BlackMana > WhiteMana)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast) || Core.Me.HasAura(Auras.Swiftcast))
                return false;

            return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);
        }
    }
}