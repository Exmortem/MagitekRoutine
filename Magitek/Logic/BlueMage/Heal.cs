using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlueMage;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Logic.BlueMage
{
    internal static class Heal
    {
        public static async Task<bool> Cure()
        {
            if (!BlueMageSettings.Instance.SelfCure)
                return false;

            if (Core.Me.CurrentHealthPercent > BlueMageSettings.Instance.SelfCureHealthPercent)
                return false;

            if (Core.Me.HasAura(Auras.AetherialMimicryHealer) && ActionManager.HasSpell(Spells.PomCure.Id))
                return await Spells.PomCure.Heal(Core.Me);

            if (!ActionManager.HasSpell(Spells.WhiteWind.Id))
                return false;

            return await Spells.WhiteWind.Heal(Core.Me);
        }
    }
}
