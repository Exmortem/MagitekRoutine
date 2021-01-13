using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Logic.Roles;

namespace Magitek.Logic.Monk
{
    internal static class Buff
    {
        public static async Task<bool> FistsOf()
        {
            switch (MonkSettings.Instance.SelectedFist)
            {
                case MonkFists.Fire when !Core.Me.HasAura(Auras.FistsofFire):
                    return await Spells.FistsOfFire.Cast(Core.Me);
                case MonkFists.Wind when !Core.Me.HasAura(Auras.FistsofWind):
                    return await Spells.FistsOfWind.Cast(Core.Me);
                case MonkFists.Earth when !Core.Me.HasAura(Auras.FistsofEarth):
                    return await Spells.FistsOfEarth.Cast(Core.Me);
                default:
                    return false;
            }
        }

        public static async Task<bool> Meditate()
        {
            if (Core.Me.ClassLevel < 54)
                return false;

            if (!MonkSettings.Instance.UseAutoMeditate)
                return false;

            if (!Core.Me.InCombat && ActionResourceManager.Monk.FithChakra < 5)
                return await Spells.Meditation.Cast(Core.Me);

            if (!Core.Me.HasTarget && ActionResourceManager.Monk.FithChakra < 5)
                return await Spells.Meditation.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> PerfectBalance()
        {
            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.TwinSnakes))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish))
                return false;

            if (Casting.LastSpell != Spells.DragonKick)
                return false;

            return await Spells.PerfectBalance.Cast(Core.Me);
        }

        public static async Task<bool> TrueNorthRiddleOfEarth() {
            if (!MonkSettings.Instance.UseTrueNorth && !MonkSettings.Instance.UseRiddleOfEarth)
                return false;

            if (Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.HasAura(Auras.RiddleOfEarth) || Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.LastSpell == Spells.TrueNorth || Casting.LastSpell == Spells.RiddleofEarth)
                return false;

            if (MonkSettings.Instance.UseTrueNorth && await PhysicalDps.TrueNorth(MonkSettings.Instance))
                return true;
            if (MonkSettings.Instance.UseRiddleOfEarth && await Spells.RiddleofEarth.Cast(Core.Me))
                return true;

            return false;
        }

        //public static async Task<bool> RiddleOfEarth()
        //{
        //    if (!MonkSettings.Instance.UseRiddleOfEarth)
        //        return false;

        //    if (Core.Me.HasAura(Auras.RiddleOfEarth))
        //        return false;

        //    if (Casting.LastSpell == Spells.RiddleofEarth || Casting.LastSpell == Spells.TrueNorth)
        //        return false;
            
        //    return await Spells.RiddleofEarth.Cast(Core.Me);
        //}

        public static async Task<bool> RiddleOfFire() 
        {
            if (!MonkSettings.Instance.UseRiddleOfFire)
                return false;

            if (Core.Me.HasMyAura(Auras.Brotherhood))
                return false;

            return await Spells.RiddleofFire.Cast(Core.Me);
        }

        public static async Task<bool> Brotherhood()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseBrotherhood)
                return false;

            if (MonkSettings.Instance.UseRiddleOfFire && Spells.RiddleofFire.Cooldown.TotalMilliseconds == 0)
                return false;
            
            return await Spells.Brotherhood.Cast(Core.Me);
        }

        public static async Task<bool> Mantra()
        {
            if (CustomOpenerLogic.InOpener)
                return false;

            if (!MonkSettings.Instance.UseMantra)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (!ActionManager.CanCast(Spells.Mantra.Id, Core.Me))
                return false;

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= MonkSettings.Instance.MantraHealthPercent) < MonkSettings.Instance.MantraAllies)
                return false;

            return await Spells.Mantra.Cast(Core.Me);
        }

        //public static async Task<bool> FormShiftOOC()
        //{

        //    if (MonkSettings.Instance.UseAutoFormShift && !Core.Me.HasTarget && DutyManager.InInstance)
        //    {
        //        if (Core.Me.ClassLevel >= 76)
        //        {
        //            if (ActionResourceManager.Monk.Timer.Seconds < 6 && ActionResourceManager.Monk.GreasedLightning == 4)
        //                return await Spells.FormShift.Cast(Core.Me);

        //            if (MonkSettings.Instance.AutoFormShiftStopCoeurl && !Core.Me.HasAura(Auras.CoeurlForm) && ActionResourceManager.Monk.GreasedLightning == 4)
        //                return await Spells.FormShift.Cast(Core.Me);

        //            if (MonkSettings.Instance.AutoFormShiftStopRaptor && !Core.Me.HasAura(Auras.RaptorForm) && ActionResourceManager.Monk.GreasedLightning == 4)
        //                return await Spells.FormShift.Cast(Core.Me);
        //        }
        //        if (Core.Me.ClassLevel >= 52 && Core.Me.ClassLevel < 76)
        //        {
        //            if (ActionResourceManager.Monk.Timer.Seconds < 6 && ActionResourceManager.Monk.GreasedLightning == 3)
        //                return await Spells.FormShift.Cast(Core.Me);

        //            if (MonkSettings.Instance.AutoFormShiftStopCoeurl && !Core.Me.HasAura(Auras.CoeurlForm) && ActionResourceManager.Monk.GreasedLightning == 3)
        //                return await Spells.FormShift.Cast(Core.Me);

        //            if (MonkSettings.Instance.AutoFormShiftStopRaptor && !Core.Me.HasAura(Auras.RaptorForm) && ActionResourceManager.Monk.GreasedLightning == 3)
        //                return await Spells.FormShift.Cast(Core.Me);
        //        }
        //    }

        //    return false;
        //}

        public static async Task<bool> FormShiftIC()
        {
            if (Core.Me.ClassLevel < 50)
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            if (Core.Me.InCombat || !ActionManager.HasSpell(Spells.FormShift.Id) || !MonkSettings.Instance.UseAutoFormShift) {
                if (ActionManager.HasSpell(Spells.DragonKick.Id))
                    return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);
            }
            
            if (Core.Me.InCombat || Core.Me.HasAura(Auras.FormlessFist) || !MonkSettings.Instance.UseAutoFormShift)
                return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);

            return await Spells.FormShift.Cast(Core.Me);
        }
    }
}
