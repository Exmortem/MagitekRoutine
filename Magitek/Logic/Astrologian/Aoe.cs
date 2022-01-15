using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Astrologian
{
    internal static class Aoe
    {
        public static async Task<bool> Gravity()
        {
            if (!AstrologianSettings.Instance.Gravity)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            var target = Combat.SmartAoeTarget(Spells.Gravity, AstrologianSettings.Instance.SmartAoe);

            if (target == null)
                return false;
            
            if (Combat.Enemies.Count(r => r.Distance(target) <= Spells.Gravity.Radius) < AstrologianSettings.Instance.GravityEnemies)
                return false;
            
            return await Spells.Gravity.Cast(target);
        }

        public static async Task<bool> LordOfCrown()
        {
            if (!Core.Me.HasAura(Auras.LordOfCrownsDrawn))
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;
            
            var target = Combat.SmartAoeTarget(Spells.Gravity, AstrologianSettings.Instance.SmartAoe);

            if (target == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(target) <= Spells.LordofCrowns.Radius) >= AstrologianSettings.Instance.LordOfCrownsEnemies)
                return await Spells.CrownPlay.Cast(target);

            if (Spells.MinorArcana.Cooldown == System.TimeSpan.Zero)
                return await Spells.CrownPlay.Cast(target);

            return false;
        }

    }
}