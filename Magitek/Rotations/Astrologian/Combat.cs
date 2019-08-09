using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Astrologian;
using Magitek.Models.Astrologian;
using Magitek.Utilities;

namespace Magitek.Rotations.Astrologian
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (PartyManager.IsInParty)
            {
                if (!AstrologianSettings.Instance.DoDamage)
                    return true;

                if (Core.Me.CurrentManaPercent < AstrologianSettings.Instance.MinimumManaPercentToDoDamage && Core.Target.CombatTimeLeft() > AstrologianSettings.Instance.DoDamageIfTimeLeftLessThan)
                    return true;
            }

            if (!GameSettingsManager.FaceTargetOnAction && !Core.Me.CurrentTarget.InView()) return false;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Globals.OnPvpMap)
            {
                if (await Pvp.Disable()) return true; //Damage
                return await Pvp.Malefic(); //Damage
            }

            if (await Aoe.Gravity()) return true;
            if (await SingleTarget.Dots()) return true;
            if (await SingleTarget.LordofCrowns()) return true;
            return await SingleTarget.Malefic();
        }
    }
}
