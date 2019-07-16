using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;

namespace Magitek.Logic.Machinist
{
    internal static class Aoe
    {
        public static async Task<bool> SpreadShot()
        {
            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.AoeEnemies)
                return false;

            return await Spells.SpreadShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Ricochet()
        {
            return await Spells.Ricochet.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Bioblaster()
        {
            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.AoeEnemies)
                return false;
            
            if (Core.Me.CurrentTarget.HasAura(Auras.Bioblaster))
                return false;

            return await Spells.Bioblaster.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AutoCrossbow()
        {
            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.AoeEnemies)
            {
                return false;
            }

            return await Spells.AutoCrossbow.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> Flamethrower()
        {
            if (!MachinistSettings.Instance.UseFlameThrower)
                return false;

            if (MovementManager.IsMoving)
                return false;

            // Stop if we're overheated 
            if (ActionResourceManager.Machinist.OverheatRemaining.Milliseconds > 1)
                return false;

            if (Core.Me.EnemiesInCone(8) < MachinistSettings.Instance.FlamethrowerEnemies)
                return false;

            return await Spells.Flamethrower.CastAura(Core.Me, Auras.Flamethrower);
        }
    }
}
