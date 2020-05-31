using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.RedMage;
using RedMageRoutines = Magitek.Utilities.Routines.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class Buff
    {
        public static async Task<bool> Acceleration()
        {
            if (!RedMageSettings.Instance.Acceleration)
                return false;
			
			if (Core.Me.ClassLevel < 50)
                return false;

            if (!RedMageRoutines.CanWeave)
                return false;

            if (Core.Me.HasAura(Auras.VerfireReady) || Core.Me.HasAura(Auras.VerstoneReady))
                return false;
			
			else
				return await Spells.Acceleration.Cast(Core.Me);
        }

        public static async Task<bool> Embolden()
        {
            if (!RedMageSettings.Instance.Embolden)
                return false;

            if (Core.Me.ClassLevel < 58)
                return false;

            if (!RedMageRoutines.CanWeave)
                return false;

            //Save it for boss fights or the AoE combo if user has selected the bosses-only option
            //Note: Using this for the AoE combo is handled by Aoe.Embolden()
            if (RedMageSettings.Instance.MeleeComboBossesOnly && !Combat.Enemies.Any(e => e.IsBoss()))
                return false;

			else
				return await Spells.Embolden.Cast(Core.Me);
        }
        
        public static async Task<bool> LucidDreaming()
        {
            if (!RedMageSettings.Instance.LucidDreaming)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!RedMageRoutines.CanWeave)
                return false;

            if (Core.Me.CurrentManaPercent > RedMageSettings.Instance.LucidDreamingManaPercent)
                return false;
			
			else
				return await Spells.LucidDreaming.Cast(Core.Me);
        }

        public static async Task<bool> Manafication()
        {
            if (!RedMageSettings.Instance.Manafication)
                return false;

            if (!RedMageRoutines.CanWeave)
                return false;

            //Don't start the combo until we've used up Dualcast
            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            //Don't fire this off while we're already in a combo, because that wastes a combo opportunity
            if (SingleTarget.ComboInProgress)
                return false;

            //This skill is used to get to the melee combo faster. If we only want the melee combo on bosses,
            //then we shouldn't use it here. Aoe.Manafication will use it in non-boss scenarios.
            if (RedMageSettings.Instance.MeleeComboBossesOnly && !Combat.Enemies.Any(e => e.IsBoss()))
                return false;

            // Can this be simplified? Yes
            // I like the readability though
            if (WhiteMana < RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                return false;
            
            if (BlackMana < RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                return false;
            
            if (WhiteMana > RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana)
                return false;

            if (BlackMana > RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana)
                return false;
			
			else
				return await Spells.Manafication.Cast(Core.Me);
        }
    }
}
