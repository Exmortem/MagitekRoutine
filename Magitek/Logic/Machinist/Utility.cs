using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Toggles;
using Magitek.Utilities;

namespace Magitek.Logic.Machinist
{
    class Utility
    {
        public static async Task<bool> Tactician()
        {

            if (!MachinistSettings.Instance.ForceTactician)
                return false;

            if (!await Spells.Tactician.Cast(Core.Me)) return false;
            MachinistSettings.Instance.ForceTactician = false;
            TogglesManager.ResetToggles();
            return true;

        }
        
        public static async Task<bool> HeadGraze()
        {

            if (!MachinistSettings.Instance.UseHeadGraze)
                return false;

            BattleCharacter interruptTarget = null;

            interruptTarget = MachinistSettings.Instance.OnlyInterruptCurrentTarget ?
                Combat.Enemies.FirstOrDefault(r => r.InView() && r == Core.Me.CurrentTarget && r.IsCasting && r.SpellCastInfo.Interruptible)
                : Combat.Enemies.FirstOrDefault(r => r.InView() && r.IsCasting && r.SpellCastInfo.Interruptible);

            if (interruptTarget == null)
                return false;

            return await Spells.HeadGraze.Cast(interruptTarget);

        }
    }
}
