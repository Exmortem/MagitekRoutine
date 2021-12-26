using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Gunbreaker;
using GunbreakerRoutine = Magitek.Utilities.Routines.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Aoe
    {

        /*************************************************************************************
         *                                    Combo
         * ***********************************************************************************/
        public static async Task<bool> DemonSlice()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseAoe, Spells.DemonSlice))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < GunbreakerSettings.Instance.DemonSliceSlaughterEnemies)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.DemonSlice.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DemonSlaughter()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseAoe, Spells.DemonSlaughter))
                return false;

            if (!GunbreakerRoutine.CanContinueComboAfter(Spells.DemonSlice))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < GunbreakerSettings.Instance.DemonSliceSlaughterEnemies)
                return false;

            return await Spells.DemonSlaughter.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************
         *                                    GCD
         * ***********************************************************************************/
        public static async Task<bool> FatedCircle()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseFatedCircle, Spells.FatedCircle))
                return false;

            if (!GunbreakerSettings.Instance.UseAoe)
                return false;

            if (Cartridge < GunbreakerRoutine.RequiredCartridgeForFatedCircle)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;
            
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < GunbreakerSettings.Instance.FatedCircleEnemies)
                return false;

            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.DoubleDown, 4000) && Cartridge <= GunbreakerRoutine.RequiredCartridgeForDoubleDown)
                return false;

            return await Spells.FatedCircle.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************
         *                                    oGCD
         * ***********************************************************************************/
        public static async Task<bool> BowShock()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseBowShock, Spells.BowShock))
                return false;

            if (!Core.Me.HasAura(Auras.NoMercy))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.BowShock.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DoubleDown()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseDoubleDown, Spells.DoubleDown))
                return false;

            if (!Core.Me.HasAura(Auras.NoMercy))
                return false;

            if (Cartridge < GunbreakerRoutine.RequiredCartridgeForDoubleDown)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < GunbreakerSettings.Instance.DoubleDownEnemies)
                return false;

            return await Spells.DoubleDown.Cast(Core.Me.CurrentTarget);
        }
    }
}