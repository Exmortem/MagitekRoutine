using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Models.Roles;
using Magitek.Utilities;

namespace Magitek.Logic.Monk
{
    internal static class Buff
    {
        public static async Task<bool> FistsOf()
        {

            if(!Core.Me.InCombat)
            {
                switch (MonkSettings.Instance.SelectedFist)
                {
                    case MonkFists.Fire when !Core.Me.HasAura(Auras.FistsofFire):
                        return await Spells.FistsOfFire.Cast(Core.Me);//

                    case MonkFists.Wind when !Core.Me.HasAura(Auras.FistsofWind):
                        return await Spells.FistsOfWind.Cast(Core.Me);

                    case MonkFists.Earth when !Core.Me.HasAura(Auras.FistsofEarth):
                        return await Spells.FistsOfEarth.Cast(Core.Me);
                    default:
                        return false;
                }
            }

            if (!Core.Me.HasAura(Auras.FistsofFire) && !Core.Me.HasAura(Auras.FistsofWind) && !Core.Me.HasAura(Auras.FistsofEarth) && Core.Me.InCombat)
            {
                switch (MonkSettings.Instance.SelectedFist)
                {
                    case MonkFists.Fire when !Core.Me.HasAura(Auras.FistsofFire):
                        return await Spells.FistsOfFire.Cast(Core.Me);//

                    case MonkFists.Wind when !Core.Me.HasAura(Auras.FistsofWind):
                        return await Spells.FistsOfWind.Cast(Core.Me);

                    case MonkFists.Earth when !Core.Me.HasAura(Auras.FistsofEarth):
                        return await Spells.FistsOfEarth.Cast(Core.Me);
                    default:
                        return false;
                }
            }

            if (Core.Me.HasAura(Auras.FistsofFire) && ActionResourceManager.Monk.GreasedLightning >=3 && Core.Me.ClassLevel >= 76)
                return await Spells.FistsOfWind.Cast(Core.Me);

            if (Core.Me.HasAura(Auras.FistsofWind) && ActionResourceManager.Monk.GreasedLightning < 3)
                return await Spells.FistsOfFire.Cast(Core.Me);

            return false;
        }


        public static async Task<bool> PerfectBalance()
        {
            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.FistsofWind))
                return false;

            if (Casting.LastSpell != Spells.DragonKick)
                return false;

            return await Spells.PerfectBalance.Cast(Core.Me);
        }

        public static async Task<bool> RiddleOfFire()
        {
            if (!MonkSettings.Instance.UseRiddleOfFire)
                return false;

            return await Spells.RiddleofFire.Cast(Core.Me);
        }

        public static async Task<bool> RiddleOfEarth()
        {
            if (!MonkSettings.Instance.UseRiddleOfEarth)
                return false;

            return await Spells.RiddleofEarth.Cast(Core.Me);
        }

        public static async Task<bool> Brotherhood()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseBrotherhood)
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

        public static async Task<bool> FormShift()
        {
            if (Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (Core.Me.HasAura(Auras.OpoOpoForm))
                return false;

            if (Core.Me.HasAura(Auras.RaptorForm))
                return false;

            if (Core.Me.HasAura(Auras.CoeurlForm))
                return false;

            if (Core.Me.ClassLevel < 52)
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            return await Spells.FormShift.Cast(Core.Me);
        }
    }
}