﻿using System;
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

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 12 + r.CombatReach) < MachinistSettings.Instance.SpreadShotEnemyCount)
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

            if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 12 + r.CombatReach) < MachinistSettings.Instance.BioBlasterEnemyCount)
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

                if (Combat.Enemies.Count(r => r.ValidAttackUnit() && r.Distance(Core.Me) <= 12 + r.CombatReach) < MachinistSettings.Instance.AutoCrossbowEnemyCount)
                    return false;

                return await Spells.AutoCrossbow.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Flamethrower()
        {
            if (ActionResourceManager.Machinist.Heat >= 50)
                return false;

            if (Spells.Wildfire.Cooldown.Seconds < 11)
                return false;

            if (Spells.Reassemble.Cooldown.Seconds < 11)
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

            if (Casting.LastSpell == Spells.Wildfire)
                return false;

            if (Core.Me.ClassLevel > 45)
            {
                if (Spells.Wildfire.Cooldown.Seconds < 2)
                    return false;
            }

            return await Spells.Ricochet.Cast(Core.Me.CurrentTarget);
        }

    }
}
