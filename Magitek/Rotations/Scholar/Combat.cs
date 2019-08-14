using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Scholar;
using Magitek.Models.Scholar;
using Magitek.Utilities;

namespace Magitek.Rotations.Scholar
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Core.Me.CurrentManaPercent <= ScholarSettings.Instance.MinimumManaPercent)
                return false;

            if (await SingleTarget.Bio()) return true;
            if (await SingleTarget.BioMultipleTargets()) return true;
            if (await SingleTarget.Ruin2()) return true;
            if (await SingleTarget.EnergyDrain2()) return true;
            return await SingleTarget.Broil();
        }
    }
}
