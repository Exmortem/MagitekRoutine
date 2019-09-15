﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;

namespace Magitek.Logic.Monk
{
    public class SingleTarget
    {
        public static async Task<bool> Bootshine()
        {
            if (Core.Me.ClassLevel < 6)
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.HasAura(Auras.OpoOpoForm))
                    return false;

                if (!Core.Me.HasAura(Auras.LeadenFist) && Core.Me.ClassLevel >= 50)
                    return false;

                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrueStrike()
        {
                if (Core.Me.ClassLevel < 4)
                    return false;

                if (!Core.Me.HasAura(Auras.RaptorForm))
                    return false;

                if (!Core.Me.HasAura(Auras.TwinSnakes) && Core.Me.ClassLevel >= 18)
                    return false;

                return await Spells.TrueStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SnapPunch()
        {

                if (Core.Me.ClassLevel < 6)
                    return false;    

                if (!Core.Me.HasAura(Auras.CoeurlForm))
                    return false;

                if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish) && Core.Me.ClassLevel >= 30)
                    return false;

                return await Spells.SnapPunch.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TwinSnakes()
        {

                if (Core.Me.ClassLevel < 18)
                    return false;

                if (!Core.Me.HasAura(Auras.RaptorForm))
                        return false;

                if (Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                    return false;

                return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DragonKick()
        {

                if (Core.Me.ClassLevel < 50)
                    return false;

                if (!Core.Me.HasAura(Auras.OpoOpoForm))
                        return false;

                if (Core.Me.CurrentTarget.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000))
                    return false;

                return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Demolish()
        {

                if (Core.Me.ClassLevel < 30)
                    return false;

                if (!Core.Me.HasAura(Auras.CoeurlForm))
                    return false;

                if (Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000))
                    return false;

                return await Spells.Demolish.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShoulderTackle()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseShoulderTackle)
                return false;

            Logger.Write($@"[Magitek] The toggle for ShoulderTackle is {MonkSettings.Instance.UseShoulderTackle}");

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) < 10)
                return false;

            return await Spells.ShoulderTackle.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> TheForbiddenChakra()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseTheForbiddenChakra)
                return false;

            if (ActionResourceManager.Monk.FithChakra < 5)
                return false;

            return await Spells.TheForbiddenChakra.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ElixerField()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseElixerField)
                return false;

            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 25 && r.InCombat);
            var cosCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);

            var canCoS = cosCount >= enemyCount || cosCount > 2;

            if (!canCoS)
                return false;

            if (Spells.ElixirField.Cooldown.Seconds != 0)
                return false;

            return await Spells.ElixirField.Cast(Core.Me);
        }

        public static async Task<bool> PerfectBalanceRoT()
        {
            if (Core.Me.HasAura(Auras.PerfectBalance))
            {
                if (!Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                    return await Spells.TwinSnakes.CastAura(Core.Me.CurrentTarget, Auras.TwinSnakes);

                if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000))
                    return await Spells.Demolish.CastAura(Core.Me.CurrentTarget, Auras.Demolish);

                if (ActionResourceManager.Monk.Timer.Seconds < 3)
                    return await Spells.SnapPunch.Cast(Core.Me.CurrentTarget);

                if (Core.Me.HasAura(Auras.LeadenFist))
                    return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

                    return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

    }
}