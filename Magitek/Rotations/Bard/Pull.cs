using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Utilities;

namespace Magitek.Rotations.Bard
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            Utilities.Routines.Bard.RefreshVars();

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat.Execute();
        }
    }
}