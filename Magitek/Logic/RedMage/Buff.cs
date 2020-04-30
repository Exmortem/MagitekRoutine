using ff14bot;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
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

            if (Core.Me.HasAura(Auras.VerfireReady) && Core.Me.HasAura(Auras.VerstoneReady))
                return false;

            if (WhiteMana >= 60 && BlackMana >= 60)
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

            if (!Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Riposte || s.Spell == Spells.Moulinet)) ;
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

            if ((WhiteMana < 40) || (WhiteMana > 50) || (BlackMana < 40) || (BlackMana > 50))
                return false;

            return await Spells.Manafication.Cast(Core.Me);
        }

        public static async Task<bool> Swiftcast()
        {
            if (!RedMageSettings.Instance.SwiftcastVerthunderVeraero)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            return await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast);
        }
    }
}
