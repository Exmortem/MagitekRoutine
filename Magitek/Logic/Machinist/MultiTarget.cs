using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using MachinistGlobals = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class MultiTarget
    {
        public static async Task<bool> SpreadShot()
        {
            if (!MachinistSettings.Instance.UseSpreadShot)
                return false;

            if (!MachinistSettings.Instance.UseAoe)
                return false;

            if (ActionManager.LastSpell == Spells.Hypercharge)
                return false;

            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.SpreadShotEnemyCount)
                return false;

            return await Spells.SpreadShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BioBlaster()
        {
            if (!MachinistSettings.Instance.UseBioBlaster)
                return false;

            if (!MachinistSettings.Instance.UseAoe)
                return false;

            if (Core.Me.HasAura(Auras.Reassembled))
                return false;

            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.BioBlasterEnemyCount)
                return false;

            return await Spells.Bioblaster.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AutoCrossbow()
        {
                if (!MachinistSettings.Instance.UseBioBlaster)
                    return false;

                if (!MachinistSettings.Instance.UseAoe)
                    return false;

                if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                    return false;

                if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.AutoCrossbowEnemyCount)
                    return false;

                return await Spells.AutoCrossbow.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Flamethrower()
        {
            if (ActionResourceManager.Machinist.Heat >= 50)
                return false;

            if (Spells.Wildfire.Cooldown.Seconds < 10)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true) || Casting.SpellCastHistory.Any(x => x.Spell == Spells.Wildfire))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.EnemiesInCone(8) < MachinistSettings.Instance.FlamethrowerEnemyCount
                || !MachinistSettings.Instance.UseFlamethrower || !MachinistSettings.Instance.UseAoe)
                return false;

            return await Spells.Flamethrower.CastAura(Core.Me, Auras.Flamethrower);
        }
        public static async Task<bool> Ricochet()
        {
            if (!MachinistSettings.Instance.UseRicochet)
                return false;

            if (!MachinistGlobals.IsInWeaveingWindow)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < MachinistSettings.Instance.RicochetEnemyCount)
                return false;

            /*add some mor precise logic for pooling/dumping
            if (Spells.Ricochet.Charges < 1.8f)
                return false;*/

            return await Spells.Ricochet.Cast(Core.Me.CurrentTarget);
        }

    }
}
