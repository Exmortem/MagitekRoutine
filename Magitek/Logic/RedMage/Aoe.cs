using ff14bot;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Logic.Roles;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal class Aoe
    {
        public static async Task<bool> Moulinet()
        {
            if (!RedMageSettings.Instance.Moulinet)
                return false;

            if (Core.Me.ClassLevel < Spells.Moulinet.LevelAcquired)
                return false;

            //If embolden coming off cd soon, wait
            if (Core.Me.ClassLevel >= Spells.Embolden.LevelAcquired
                && Spells.Embolden.Cooldown.Seconds <= 10)
                return false;

            //Hopefully cast 3 moulinet in a row so we can combo
            if (InAoeCombo())
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);

            //As to not overcap on black/white mana
            if (WhiteMana == 100
                && BlackMana == 100)
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> ContreSixte()
        {
            if (!RedMageSettings.Instance.UseAoe)
                return false;

            if (Core.Me.ClassLevel < Spells.ContreSixte.LevelAcquired)
                return false;

            if (Spells.ContreSixte.Cooldown != TimeSpan.Zero)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.ContreSixte.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Scatter()
        {
            if (!RedMageSettings.Instance.Scatter)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.ClassLevel < Spells.Scatter.LevelAcquired)
                return false;

            if (!Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (!Core.Me.HasAura(Auras.Swiftcast)
                && !RedMageSettings.Instance.SwiftcastScatter)
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            if (InCombo())
                return false;

            return await Spells.Scatter.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Impact()
        {
            if (!RedMageSettings.Instance.UseAoe)
                return false;

            if (Core.Me.ClassLevel < Spells.Impact.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            if (Casting.LastSpell == Spells.Impact)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return await Spells.Impact.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Swiftcast)
                && RedMageSettings.Instance.SwiftcastScatter)
                return await Spells.Impact.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.Acceleration))
                return await Spells.Impact.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Verthunder2()
        {
            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.ClassLevel < Spells.Verthunder2.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (BlackMana + 7 - WhiteMana > 15)
                return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Veraero2()
        {
            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.ClassLevel < Spells.Veraero2.LevelAcquired)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (WhiteMana + 7 - BlackMana > 15)
                return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);

            if (WhiteMana == 100
                && BlackMana == 100)
                return false;

            return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);
        }
        private static bool InCombo()
        {
            if (Core.Me.ClassLevel >= 6
                && Core.Me.ClassLevel < 35)
            {
                if (Casting.LastSpell == Spells.CorpsACorps)
                    return true;
            }
            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel < 50)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte)
                    return true;
            }
            if (Core.Me.ClassLevel >= 50
                && Core.Me.ClassLevel < 68)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau)
                    return true;
            }
            if (Core.Me.ClassLevel >= 68
                && Core.Me.ClassLevel < 80)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement)
                    return true;
            }
            if (Core.Me.ClassLevel >= 80
                && Core.Me.ClassLevel < 90)
            {
                if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Verflare)
                    return true;
            }

            if (Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Scorch)
                return true;

            return false;
        }
        public static bool InAoeCombo()
        {
            if (!RedMageSettings.Instance.UseAoe)
                return false;

            if (Core.Me.EnemiesNearby(10).Count() < RedMageSettings.Instance.AoeEnemies)
                return false;

            if (Casting.SpellCastHistory.Take(3).All(x => x.Spell == Spells.Moulinet))
                return true;

            if (WhiteMana >= 60
                    && BlackMana >= 60)
                return true;

            if (Casting.LastSpell == Spells.Moulinet)
                if (WhiteMana >= 20
                    && BlackMana >= 20)
                    return true;

            return false;
        }
        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return MagicDps.ForceLimitBreak(Spells.Skyshard, Spells.Starstorm, Spells.Meteor, Spells.Blizzard);
        }
    }
}
