using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;

namespace Magitek.Rotations.Gunbreaker
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);

            if (GunbreakerSettings.Instance.PullWithLightningShot)
                await Spells.LightningShot.Cast(Core.Me.CurrentTarget);

            return await Combat.Execute();
        }
    }
}