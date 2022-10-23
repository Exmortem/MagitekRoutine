using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Monk;
using MonkRoutine = Magitek.Utilities.Routines.Monk;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Monk
{
    internal static class Buff
    {
        public static async Task<bool> Meditate()
        {
            if (Core.Me.ClassLevel < 54)
                return false;

            if (!MonkSettings.Instance.UseAutoMeditate)
                return false;

            if (!Core.Me.InCombat && ActionResourceManager.Monk.ChakraCount < 5)
                return await Spells.Meditation.Cast(Core.Me);

            if (!Core.Me.HasTarget && ActionResourceManager.Monk.ChakraCount < 5)
                return await Spells.Meditation.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> PerfectBalance()
        {
            if (Core.Me.ClassLevel < 50)
                return false;

            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.DisciplinedFist))
                return false;

            if (MonkRoutine.AoeEnemies5Yards >= MonkSettings.Instance.AoeEnemies)
            {
                if (Casting.LastSpell != Spells.ArmOfTheDestroyer)
                    return false;
            }
            else
            {

                if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish))
                    return false;

                if (Casting.LastSpell != Spells.Bootshine)
                    return false;

            }

            return await Spells.PerfectBalance.Cast(Core.Me);
        }

        public static async Task<bool> RiddleOfEarth()
        {
            if (!MonkSettings.Instance.UseRiddleOfEarth)
                return false;

            if (Core.Me.ClassLevel < 50)
                return false;

            return await Spells.RiddleofEarth.Cast(Core.Me);

        }

        public static async Task<bool> RiddleOfFire()
        {
            if (!MonkSettings.Instance.UseRiddleOfFire)
                return false;

            if (Core.Me.ClassLevel < 68)
                return false;

            if (!Core.Me.HasAura(Auras.DisciplinedFist))
                return false;

            if (Core.Me.HasMyAura(Auras.Brotherhood))
                return false;

            return await Spells.RiddleofFire.Cast(Core.Me);
        }

        public static async Task<bool> RiddleOfWind()
        {
            if (!MonkSettings.Instance.UseRiddleOfWind)
                return false;

            if (Core.Me.ClassLevel < 72)
                return false;

            if (!Core.Me.HasAura(Auras.DisciplinedFist))
                return false;

            if (Core.Me.HasMyAura(Auras.Brotherhood))
                return false;

            return await Spells.RiddleofWind.Cast(Core.Me);
        }

        public static async Task<bool> Brotherhood()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseBrotherhood)
                return false;

            if (!Core.Me.HasAura(Auras.DisciplinedFist))
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


        public static async Task<bool> FormShiftIC()
        {
            if (!Spells.FormShift.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (Core.Me.HasAura(Auras.OpoOpoForm))
                return false;

            if (Core.Me.HasAura(Auras.RaptorForm))
                return false;

            if (Core.Me.HasAura(Auras.CoeurlForm))
                return false;

            if (Core.Me.HasAura(Auras.FormlessFist))
                return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.LeadenFist))
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            return await Spells.FormShift.Cast(Core.Me);
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.Brotherhood.IsKnown() && !Spells.Brotherhood.IsReady(11000))
                return false;

            return await PhysicalDps.UsePotion(MonkSettings.Instance);
        }
    }
}
