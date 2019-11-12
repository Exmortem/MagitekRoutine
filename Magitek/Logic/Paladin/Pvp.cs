using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;

namespace Magitek.Logic.Paladin
{
    internal static class Pvp
    {
        public static async Task<bool> Testudo()
        {
            if (!PaladinSettings.Instance.Testudo)
                return false;

            if (!Globals.InParty)
                return false;

            var testudoCount = Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent < PaladinSettings.Instance.TestudoHealth);

            if (testudoCount < PaladinSettings.Instance.TestudoAllies)
                return false;

            return await Spells.Testudo.Cast(Core.Me);
        }

        public static async Task<bool> PushBack()
        {
            if (!PaladinSettings.Instance.PushBack)
                return false;

            if (!Core.Me.CurrentTarget.IsDps())
                return false;

            return await Spells.PushBack.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Enliven()
        {
            return await Spells.Enliven.Cast(Core.Me);
        }

        public static async Task<bool> GlorySlash()
        {
            if (!PaladinSettings.Instance.GlorySlash)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 6) < 2 + Core.Me.CombatReach)
                return false;

            return await Spells.GlorySlash.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FullSwing()
        {
            if (!PaladinSettings.Instance.FullSwing)
                return false;

            return await Spells.FullSwing.Cast(Core.Me.CurrentTarget);
        }
    }
}