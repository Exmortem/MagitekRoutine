using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using Iaijutsu = ff14bot.Managers.ActionResourceManager.Samurai.Iaijutsu;
using SamuraiRoutine = Magitek.Utilities.Routines.Samurai;

namespace Magitek.Logic.Samurai
{
    internal static class SingleTarget
    {

        public static async Task<bool> Hakaze()
        {
            return await Spells.Hakaze.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************************
         *                                    Combo 1 - Filler 3 GCD
         * ***********************************************************************************************/
        public static async Task<bool> Jinpu()
        {
            if (!SamuraiRoutine.CanContinueComboAfter(Spells.Hakaze))
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Getsu))
                return false;

            return await Spells.Jinpu.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Gekko()
        {
            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Getsu))
                return false;

            if (!SamuraiRoutine.CanContinueComboAfter(Spells.Jinpu) && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            return await Spells.Gekko.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************************
         *                                    Combo 2 - Filler 3 GCD
         * ***********************************************************************************************/
        public static async Task<bool> Shifu()
        {
            if (!SamuraiRoutine.CanContinueComboAfter(Spells.Hakaze))
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Ka))
                return false;

            return await Spells.Shifu.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Kasha()
        {
            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Ka))
                return false;

            if (!SamuraiRoutine.CanContinueComboAfter(Spells.Shifu) && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            return await Spells.Kasha.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************************
         *                                    Combo3 - Filler 2 GCD & 4GCD
         * ***********************************************************************************************/
        public static async Task<bool> Yukikaze()
        {
            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Setsu))
                return false;

            if (!SamuraiRoutine.CanContinueComboAfter(Spells.Hakaze) && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (SamuraiRoutine.isReadyFillerRotation && SamuraiSettings.Instance.SamuraiFillerStrategy.Equals(SamuraiFillerStrategy.ThreeGCD))
                return false;

            return await Spells.Yukikaze.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************************
         *                                    Filler 1 GCD
         * ***********************************************************************************************/
        public static async Task<bool> Enpi()
        {
            if (!SamuraiSettings.Instance.UseEnpi)
                return false;

            if (Core.Me.CurrentTarget.Distance() < 10f && !Core.Me.HasAura(Auras.EnhancedEnpi))
                return false;

            return await Spells.Enpi.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************************
         *                                    oGCD
         * ***********************************************************************************************/
        public static async Task<bool> Shoha()
        {
            if (!SamuraiSettings.Instance.UseShoha)
                return false;

            if (ActionResourceManager.Samurai.Meditation < 3)
                return false;

            return await Spells.Shoha.Cast(Core.Me.CurrentTarget);
        }


        /**********************************************************************************************
        *                                    oGCD using Kenki
        * ********************************************************************************************/
        public static async Task<bool> HissatsuGyoten() //dash forward
        {
            if (!SamuraiSettings.Instance.UseHissatsuGyoten)
                return false;

            if (!SamuraiRoutine.GlobalCooldown.CanWeave(1))
                return false;

            if (Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (ActionResourceManager.Samurai.Kenki < 10 + SamuraiSettings.Instance.ReservedKenki)
                return false;

            if (SamuraiSettings.Instance.UseHissatsuGyotenOnlyWhenOutOfMeleeRange && !Core.Me.CurrentTarget.WithinSpellRange(Spells.Hakaze.Range))
            {
                return Combat.Enemies.Count(x => x.Distance(Core.Me) <= SamuraiRoutine.Fuko.Radius + Core.Me.CombatReach) < SamuraiSettings.Instance.AoeEnemies 
                    ? await Spells.HissatsuGyoten.Cast(Core.Me.CurrentTarget) 
                    : false;
            }

            return await Spells.HissatsuGyoten.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuYaten() //dash backward - to use with 1GCD Filler - Should not be used with Routine to avoid dying in a wall
        {
            if (!SamuraiSettings.Instance.UseHissatsuYaten)
                return false;

            if (!SamuraiRoutine.GlobalCooldown.CanWeave(1))
                return false;

            if (Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (ActionResourceManager.Samurai.Kenki < 10 + SamuraiSettings.Instance.ReservedKenki) //10 for Yaten + 10 for Guoten
                return false;

            if (!Spells.HissatsuGyoten.IsKnownAndReady())
                return false;

            if (Casting.LastSpell != Spells.MidareSetsugekka)
                return false;

            return await Spells.HissatsuYaten.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuSenei()
        {
            if (!SamuraiSettings.Instance.UseHissatsuSenei)
                return false;

            if (ActionResourceManager.Samurai.Kenki < 25 + SamuraiSettings.Instance.ReservedKenki)
                return false;

            if (Casting.LastSpell != Spells.MidareSetsugekka)
                return false;

            return await Spells.HissatsuSenei.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuShinten()
        {
            Logger.Write($@"Kenki Jauge = {ActionResourceManager.Samurai.Kenki} ");
            if (!SamuraiSettings.Instance.UseHissatsuShinten)
                return false;

            if (ActionResourceManager.Samurai.Kenki < 25 + SamuraiSettings.Instance.ReservedKenki)
                return false;
            
            if (Casting.LastSpell == Spells.HissatsuSenei)
                return false;

            if (SamuraiRoutine.SenCount == 3)
                return false;

            if (Spells.HissatsuSenei.IsKnownAndReady() || (Spells.HissatsuSenei.IsKnownAndReady(10000) && ActionResourceManager.Samurai.Kenki <= 85))
                return false;

            return await Spells.HissatsuShinten.Cast(Core.Me.CurrentTarget);
        }


        /**********************************************************************************************
        *                                    Iaijutsu
        * ********************************************************************************************/

        public static async Task<bool> MidareSetsugekka()
        {
            if (!SamuraiSettings.Instance.UseMidareSetsugekka)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 6)
                return false;

            if (SamuraiRoutine.SenCount != 3)
                return false;

            if(!await Spells.MidareSetsugekka.Cast(Core.Me.CurrentTarget))
                return false;

            if (SamuraiRoutine.prepareFillerRotation && (Spells.TsubameGaeshi.Charges < 1 || Spells.KaeshiSetsugekka.Charges < 1))
                SamuraiRoutine.InitializeFillerVar(false, true);

            return true;
        }

        public static async Task<bool> Higanbana()
        {
            if (!SamuraiSettings.Instance.UseHigabana)
                return false;

            if (Utilities.Combat.Enemies.Count(x => x.InView() && x.Distance(Core.Me) <= 6 + x.CombatReach) >= SamuraiSettings.Instance.AoeEnemies)
            {
                if (SamuraiSettings.Instance.UseAoe)
                    return false;
            }

            if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 3)
                return false;

            if (SamuraiRoutine.SenCount != 1)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true))
                return false;

            if (Spells.MeikyoShisui.Charges >= 1)
                return false;

            if (!await Spells.Higanbana.Cast(Core.Me.CurrentTarget))
                return false;

            SamuraiRoutine.InitializeFillerVar(true, false);

            return true;
        }

        /**********************************************************************************************
        *                              Tsubamegaeshi (after Iaijutsu)
        * ********************************************************************************************/
        public static async Task<bool> KaeshiHiganbana()
        {
            if (!SamuraiSettings.Instance.UseKaeshiHiganbana)
                return false;

            return await Spells.KaeshiHiganbana.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> KaeshiSetsugekka()
        {
            if (!SamuraiSettings.Instance.UseKaeshiSetsugekka)
                return false;

            if (!await Spells.KaeshiSetsugekka.Cast(Core.Me.CurrentTarget))
                return false;

            SamuraiRoutine.InitializeFillerVar(false, false);

            return true;
        }
    }
}
