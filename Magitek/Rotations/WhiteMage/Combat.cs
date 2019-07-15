using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.WhiteMage;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;

namespace Magitek.Rotations.WhiteMage
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (Utilities.Combat.Enemies.Count > WhiteMageSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (PartyManager.IsInParty)
            {
                if (!WhiteMageSettings.Instance.DoDamage)
                    return true;

                if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.MinimumManaPercentToDoDamage && !Core.Me.HasAura(Auras.ThinAir))
                    return true;
            }
            
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (await Aoe.Holy()) return true;
            if (await Aoe.AssizeDamage()) return true;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await SingleTarget.AfflatusMisery()) return true;
            if (await SingleTarget.Dots()) return true;
            return await SingleTarget.Stone();
        }
    }
}
