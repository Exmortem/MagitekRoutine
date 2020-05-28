using System.Collections.Generic;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Roles;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Roles
{
    internal static class PhysicalDps
    {
        public static async Task<bool> SecondWind<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseSecondWind)
                return false;

            if (Core.Me.CurrentHealthPercent > settings.SecondWindHpPercent)
                return false;

            return await Spells.SecondWind.Cast(Core.Me);
        }

        public static async Task<bool> TrueNorth<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseTrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.SpellCastHistory.Take(10).Any(s => s.Spell == Spells.TrueNorth))
                return false;

            return await Spells.TrueNorth.Cast(Core.Me);
        }

        public static async Task<bool> ArmsLength<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.ForceArmsLength)
                return false;

            if (!await Spells.ArmsLength.Cast(Core.Me)) return false;
            settings.ForceArmsLength = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> Bloodbath<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseBloodbath)
                return false;

            if (Core.Me.CurrentHealthPercent > settings.BloodbathHpPercent)
                return false;

            return await Spells.Bloodbath.Cast(Core.Me);
        }

        public static async Task<bool> Feint<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UseFeint)
                return false;

            return await Spells.Feint.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Interrupt(PhysicalDpsSettings settings)
        {
            List<SpellData> stuns = new List<SpellData>();
            List<SpellData> interrupts = new List<SpellData>();

            if (Core.Me.IsMeleeDps())
            {
                stuns.Add(Spells.LegSweep);
            }

            if (Core.Me.IsRangedDps())
            {
                interrupts.Add(Spells.HeadGraze);
            }

            return await InterruptAndStunLogic.DoStunAndInterrupt(stuns, interrupts, settings.Strategy);
        }

        public static async Task<bool> Peloton<T>(T settings) where T : PhysicalDpsSettings
        {
            if (!settings.UsePeloton)
                return false;

            if (Globals.InParty)
            {
                if (Globals.PartyInCombat)
                    return false;
            }

            if (Combat.OutOfCombatTime.ElapsedMilliseconds <= 3000)
                return false;

            if (!MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Peloton, false, 1000))
                return false;

            return await Spells.Peloton.CastAura(Core.Me, Auras.Peloton);
        }
    }
}
