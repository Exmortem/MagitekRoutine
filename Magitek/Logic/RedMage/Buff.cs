using ff14bot;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //if (Casting.LastSpell == Spells.Redoublement
            //    || Casting.LastSpell == Spells.Manafication)
            //{
            //Just use on CD I guess
            return await Spells.Embolden.Cast(Core.Me.CurrentTarget);
            //}

            //return false;
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

            if (WhiteMana > 50
                || BlackMana >  50)
                return false;

            //Manafication cancels any combo
            if (InCombo())
                return false;

            if (Spells.Embolden.Cooldown.TotalMilliseconds <= 5500)
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

            if (Core.Me.CurrentMana > RedMageSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (Spells.LucidDreaming.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
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

            if (Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration)
                || Core.Me.HasAura(Auras.VerfireReady)
                || Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (InCombo())
                return false;

            return await Spells.Swiftcast.Cast(Core.Me.CurrentTarget);
        }
        private static bool InCombo()
        {
            if (Casting.LastSpell == Spells.Engagement
                || Casting.LastSpell == Spells.CorpsACorps
                || Casting.LastSpell == Spells.Riposte
                || Casting.LastSpell == Spells.Zwerchhau
                || Casting.LastSpell == Spells.Redoublement
                || Casting.LastSpell == Spells.Verholy
                || Casting.LastSpell == Spells.Verflare
                || Casting.LastSpell == Spells.Scorch)
                return true;

            return false;
        }
    }
}
