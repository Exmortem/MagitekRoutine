﻿using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Models.Roles;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Ninja
{
    internal static class Utility
    {
        //public static async Task<bool> ArmsLenght()
        //{

        //    if (!NinjaSettings.Instance.ForceArmsLenght)
        //        return false;

        //    if (!await Spells.ArmsLength.Cast(Core.Me)) return false;
        //    NinjaSettings.Instance.ForceArmsLenght = false;
        //    TogglesManager.ResetToggles();
        //    return true;

        //}

        public static async Task<bool> ForceBloodBath()
        {

            if (!NinjaSettings.Instance.ForceBloodBath)
                return false;

            if (!await Spells.Bloodbath.Cast(Core.Me)) return false;
            NinjaSettings.Instance.ForceBloodBath = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceTrueNorth()
        {

            if (!NinjaSettings.Instance.ForceTrueNorth)
                return false;

            if (!await Spells.TrueNorth.Cast(Core.Me)) return false;
            NinjaSettings.Instance.ForceTrueNorth = false;
            TogglesManager.ResetToggles();
            return true;

        }

        public static async Task<bool> ForceFeint()
        {

            if (!NinjaSettings.Instance.ForceFeint)
                return false;

            if (!await Spells.Feint.Cast(Core.Me.CurrentTarget)) return false;
            NinjaSettings.Instance.ForceFeint = false;
            TogglesManager.ResetToggles();
            return true;

        }

        public static async Task<bool> ForceShadeShift()
        {

            if (!NinjaSettings.Instance.ForceShadeShift)
                return false;

            if (!await Spells.ShadeShift.Cast(Core.Me)) return false;
            NinjaSettings.Instance.ForceShadeShift = false;
            TogglesManager.ResetToggles();
            return true;

        }

        public static async Task<bool> ShadeShift()
        {
            if (!NinjaSettings.Instance.UseShadeShift)
                return false;

            if (Core.Me.CurrentHealthPercent > NinjaSettings.Instance.ShadeShiftHealthPercent)
                return false;

            return await Spells.ShadeShift.Cast(Core.Me);
        }

        public static async Task<bool> ForceSecondWind()
        {

            if (!NinjaSettings.Instance.ForceSecondWind)
                return false;

            if (!await Spells.SecondWind.Cast(Core.Me)) return false;
            NinjaSettings.Instance.ForceSecondWind = false;
            TogglesManager.ResetToggles();
            return true;

        }
    }
}
