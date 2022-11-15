using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using MonkRoutine = Magitek.Utilities.Routines.Monk;

namespace Magitek.Logic.Monk
{
    internal static class Pvp
    {
        public static async Task<bool> BootshinePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BootshinePvp.CanCast())
                return false;

            return await Spells.BootshinePvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrueStrikePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.TrueStrikePvp.CanCast())
                return false;

            return await Spells.TrueStrikePvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SnapPunchPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SnapPunchPvp.CanCast())
                return false;

            return await Spells.SnapPunchPvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> DragonKickPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.DragonKickPvp.CanCast())
                return false;

            return await Spells.DragonKickPvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> TwinSnakesPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.TwinSnakesPvp.CanCast())
                return false;

            return await Spells.TwinSnakesPvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> DemolishPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.DemolishPvp.CanCast())
                return false;

            return await Spells.DemolishPvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhantomRushPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.PhantomRushPvp.CanCast())
                return false;

            return await Spells.PhantomRushPvp.CastPvpCombo(Spells.PhantomRushPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SixSidedStarPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SixSidedStarPvp.CanCast())
                return false;

            if(!MonkSettings.Instance.Pvp_SixSidedStar)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.SixSidedStarPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnlightenmentPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.EnlightenmentPvp.CanCast())
                return false;

            if (!MonkSettings.Instance.Pvp_Enlightenment)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 10)
                return false;

            return await Spells.EnlightenmentPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ThunderclapPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ThunderclapPvp.CanCast())
                return false;

            if (!MonkSettings.Instance.Pvp_Thunderclap)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.PvpPressurePoint))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20) 
                return false;

            return await Spells.ThunderclapPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RisingPhoenixPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RisingPhoenixPvp.CanCast())
                return false;

            if (!MonkSettings.Instance.Pvp_RisingPhoenix)
                return false;

            if (Core.Me.HasAura(Auras.PvpFireResonance))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.RisingPhoenixPvp.Cast(Core.Me);
        }

        public static async Task<bool> RiddleofEarthPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.RiddleofEarthPvp.CanCast())
                return false;

            if (!MonkSettings.Instance.Pvp_RisingPhoenix)
                return false;

            if (Core.Me.HasAura(Auras.PvpEarthResonance))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.RiddleofEarthPvp.Cast(Core.Me);
        }

        public static async Task<bool> EarthReplyPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.EarthReplyPvp.CanCast())
                return false;

            if (!MonkSettings.Instance.Pvp_EarthReply)
                return false;

            if (!Core.Me.HasAura(Auras.PvpEarthResonance))
                return false;

            if (Core.Me.HasAura(Auras.PvpEarthResonance, true, 2000) && Core.Me.CurrentHealthPercent > MonkSettings.Instance.Pvp_EarthReplyHealthPercent)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.EarthReplyPvp.Cast(Core.Me);
        }

        public static async Task<bool> MeteodrivePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!MonkSettings.Instance.Pvp_Meteodrive)
                return false;

            if (!Spells.MeteodrivePvp.CanCast())
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if(Core.Me.CurrentTarget.CurrentHealthPercent > MonkSettings.Instance.Pvp_MeteodriveHealthPercent)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.PvpPressurePoint) && MonkSettings.Instance.Pvp_MeteodriveWithEnlightenment)
                return false;

            return await Spells.MeteodrivePvp.Cast(Core.Me.CurrentTarget);
        }
    }
}
