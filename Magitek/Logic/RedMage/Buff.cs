using ff14bot;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.RedMage;


namespace Magitek.Logic.RedMage
{
    internal class Buff
    {
        public static async Task<bool> Acceleration()
        {
            if (!RedMageSettings.Instance.Acceleration)
                return false;

            if (Core.Me.ClassLevel < Spells.Acceleration.LevelAcquired)
                return false;

            if (Spells.Acceleration.Cooldown != TimeSpan.Zero
                && Spells.Acceleration.Charges == 0)
                return false;

            if (InAoeCombo())
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration)
                || Core.Me.HasAura(Auras.VerfireReady)
                || Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (WhiteMana >= 50
                &&  BlackMana >= 50)
                return false;

            if (InCombo())
                return false;

            return await Spells.Acceleration.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Embolden()
        {
            if (!RedMageSettings.Instance.Embolden)
                return false;

            if (Core.Me.ClassLevel < Spells.Embolden.LevelAcquired)
                return false;

            if (Spells.Embolden.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (WhiteMana < 31
                || BlackMana < 31)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            //Just use on CD I guess
            return await Spells.Embolden.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Manafication()
        {
            if (!RedMageSettings.Instance.Manafication)
                return false;

            if (Core.Me.ClassLevel < Spells.Manafication.LevelAcquired)
                return false;

            if (Spells.Manafication.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (WhiteMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana
                || BlackMana <= RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                return false;

            if (BlackMana > RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana
                || WhiteMana > RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana)
                return false;

            if (InAoeCombo())
                return false;

            //Manafication cancels any combo
            if (InCombo())
                return false;

            if (Spells.Embolden.Cooldown.TotalMilliseconds <= 10000)
                return false;

            return await Spells.Manafication.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> MagickBarrier()
        {
            if (!RedMageSettings.Instance.MagickBarrier)
               return false;

            if (Core.Me.ClassLevel < Spells.MagickBarrier.LevelAcquired)
                return false;

            if (Spells.MagickBarrier.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.MagickBarrier.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> LucidDreaming()
        {
            if (!RedMageSettings.Instance.LucidDreaming)
                return false;

            if (Core.Me.ClassLevel < Spells.LucidDreaming.LevelAcquired)
                return false;

            if (Core.Me.CurrentManaPercent > RedMageSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (Spells.LucidDreaming.Cooldown != TimeSpan.Zero)
                return false;

            if (InCombo())
                return false;

            return await Spells.LucidDreaming.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Swiftcast()
        {
            if (Core.Me.ClassLevel < Spells.Swiftcast.LevelAcquired)
                return false;

            if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                return false;

            if (InAoeCombo())
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration)
                || Core.Me.HasAura(Auras.VerfireReady)
                || Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (InCombo())
                return false;

            if (!RedMageSettings.Instance.SwiftcastVerthunderVeraero
                && !RedMageSettings.Instance.SwiftcastScatter)
                return false;

            if (WhiteMana >= 50
                && BlackMana >= 50)
                return false;

            return await Spells.Swiftcast.Cast(Core.Me.CurrentTarget);
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
    }
}
