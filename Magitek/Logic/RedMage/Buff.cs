using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class Buff
    {
        public static async Task<bool> Acceleration()
        {
            if (!RedMageSettings.Instance.Acceleration)
                return false;

            if (!Core.Me.InCombat)
                return false;
			
			if (Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.HasAura(Auras.VerfireReady) || Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (WhiteMana >= 60 && BlackMana >=60 && Core.Me.HasAura(Auras.VerfireReady) && Core.Me.HasAura(Auras.VerstoneReady))
                return false;
			
			return await Spells.Acceleration.Cast(Core.Me);
        }

        public static async Task<bool> Embolden()
        {
            if (!RedMageSettings.Instance.Embolden)
                return false;
            
            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.ClassLevel < 58)
                return false;

            if (ActionManager.LastSpell != Spells.Zwerchhau)
                return false;
			
            return await Spells.Embolden.Cast(Core.Me);
        }
        
        public static async Task<bool> LucidDreaming()
        {
            if (!RedMageSettings.Instance.LucidDreaming)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent > RedMageSettings.Instance.LucidDreamingManaPercent)
                return false;
			
			return await Spells.LucidDreaming.Cast(Core.Me);
        }

        public static async Task<bool> Manafication()
        {
            if (!RedMageSettings.Instance.Manafication)
                return false;

            // Why tho?
            //if (ActionManager.LastSpell != Spells.Jolt && ActionManager.LastSpell != Spells.Jolt2)
            //    return false;

            // Can this be simplified? Yes
            // I like the readability though
            /*if (WhiteMana < RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                return false;
            
            if (BlackMana < RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                return false;
            
            if (WhiteMana > RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana)
                return false;

            if (BlackMana > RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana)
                return false;*/

            // Should really only be between 40 and 50 for optimal use

			if (WhiteMana < 40 || WhiteMana > 50 || BlackMana < 40 || BlackMana > 50)
                return false;

			return await Spells.Manafication.Cast(Core.Me);
        }
    }
}
