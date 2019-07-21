using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
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
                if (!Core.Me.HasAura(Auras.OpoOpoForm))
                    return false;

                if (!Core.Me.HasAura(Auras.LeadenFist))
                    return false;

                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrueStrike()
        {

                if (!Core.Me.HasAura(Auras.RaptorForm))
                    return false;

                if (!Core.Me.HasAura(Auras.TwinSnakes))
                    return false;

                return await Spells.TrueStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SnapPunch()
        {

                if (!Core.Me.HasAura(Auras.CoeurlForm))
                    return false;

                if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish))
                    return false;

                return await Spells.SnapPunch.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TwinSnakes()
        {

                if (!Core.Me.HasAura(Auras.RaptorForm))
                    return false;

                if (Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                    return false;

                return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DragonKick()
        {
                if (!Core.Me.HasAura(Auras.OpoOpoForm))
                    return false;

                //if (MonkSettings.Instance.DragonKickUseTtd)
                //{
                //    if (Core.Me.CurrentTarget.CombatTimeLeft() < MonkSettings.Instance.DragonKickMinimumTtd)
                //        return false;
                //}
                /*else
                {
                    if (!Core.Me.CurrentTarget.IsBoss())
                    {
                        if (!Core.Me.CurrentTarget.HealthCheck(MonkSettings.Instance.DragonKickMinimumHealth, MonkSettings.Instance.DragonKickMinimumHealthPercent))
                            return false;
                    }
                }
                */
                if (Core.Me.CurrentTarget.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000))
                    return false;

                return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Demolish()
        {
                if (!Core.Me.HasAura(Auras.CoeurlForm))
                    return false;

                //if (MonkSettings.Instance.DemolishUseTtd)
                //{
                //    if (Core.Me.CurrentTarget.CombatTimeLeft() < MonkSettings.Instance.DemolishMinimumTtd)
                //        return false;
                //}
                /*else
                {
                    if (!Core.Me.CurrentTarget.IsBoss())
                    {
                        if (!Core.Me.CurrentTarget.HealthCheck(MonkSettings.Instance.DemolishMinimumHealth, MonkSettings.Instance.DemolishMinimumHealthPercent))
                            return false;
                    }
                }*/

                if (Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000))
                    return false;

                return await Spells.Demolish.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShoulderTackle()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseShoulderTackle)
                return false;

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

    }
}