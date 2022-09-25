using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using SamuraiRoutine = Magitek.Utilities.Routines.Samurai;
using System;
using System.Linq;
using System.Threading.Tasks;
using Iaijutsu = ff14bot.Managers.ActionResourceManager.Samurai.Iaijutsu;

namespace Magitek.Logic.Samurai
{
    internal static class Aoe
    {
        
        public static async Task<bool> Fuko()
        {
            if (!SamuraiSettings.Instance.UseAoe)
                return false;

            if (Core.Me.ClassLevel < 86 && SamuraiRoutine.EnemiesInCone < SamuraiSettings.Instance.AoeEnemies) // Fuga (lvl < 86) is a cone based attack
                return false;
          
            if (Core.Me.ClassLevel >= 86 && SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
                return false;

            return await SamuraiRoutine.Fuko.Cast(Core.Me.CurrentTarget);
        }

        /**********************************************************************************************
         *                                    Combo 1 To get Ka (purple)
         * ********************************************************************************************/
        public static async Task<bool> Oka()
        {
            if (!Core.Me.HasAura(Auras.MeikyoShisui) && !SamuraiRoutine.CanContinueComboAfter(SamuraiRoutine.Fuko))
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Ka))
                return false;

            return await Spells.Oka.Cast(Core.Me);
        }

        /**********************************************************************************************
         *                                    Combo 2 To get Getsu (blue)
         * ********************************************************************************************/
        public static async Task<bool> Mangetsu()
        {
            if (!SamuraiRoutine.CanContinueComboAfter(SamuraiRoutine.Fuko) && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Getsu))
                return false;

            return await Spells.Mangetsu.Cast(Core.Me);
        }


        /**********************************************************************************************
         *                                    oGCD using Kenki
         * ********************************************************************************************/
        public static async Task<bool> HissatsuKyuten()
        {
            if (!SamuraiSettings.Instance.UseHissatsuKyuten)
                return false;

            if (ActionResourceManager.Samurai.Kenki < 25 + SamuraiSettings.Instance.ReservedKenki)
                return false;

            if (SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
                return false;

            if (Spells.HissatsuGuren.IsKnownAndReady(10000))
                return false;

            return await Spells.HissatsuKyuten.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuGuren()
        {
            if (!SamuraiSettings.Instance.UseHissatsuGuren)
                return false;

            if (ActionResourceManager.Samurai.Kenki < 25 + SamuraiSettings.Instance.ReservedKenki)
                return false;

            if (!Core.Me.CurrentTarget.InView())
                return false;

            if (Combat.Enemies.Count(x => x.InView() && x.Distance(Core.Me) <= 10 + x.CombatReach) < SamuraiSettings.Instance.AoeEnemies)
                return false;

            return await Spells.HissatsuGuren.Cast(Core.Me.CurrentTarget);
        }


        /**********************************************************************************************
         *                                   oGCD
         * ********************************************************************************************/
        public static async Task<bool> ShohaII()
        {
            if (!SamuraiSettings.Instance.UseShohaII)
                return false;

            if (SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
                return false;

            if (ActionResourceManager.Samurai.Meditation < 3)
                return false;

            return await Spells.ShohaII.Cast(Core.Me.CurrentTarget);
        }


        /**********************************************************************************************
        *                                    Iaijutsu
        * ********************************************************************************************/
        public static async Task<bool> TenkaGoken()
        {
            if (!SamuraiSettings.Instance.UseTenkaGoken)
                return false;

            if (SamuraiRoutine.SenCount != 2)
                return false;

            if (SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
                return false;

            if (!await Spells.TenkaGoken.Cast(Core.Me.CurrentTarget))
            {
                SamuraiRoutine.iaijutsuSuccessful = false;
                return false;
            }
            SamuraiRoutine.iaijutsuSuccessful = true;

            return true;
        }


        /**********************************************************************************************
         *                              Tsubamegaeshi (after Iaijutsu)
         * ********************************************************************************************/
        public static async Task<bool> KaeshiGoken()
        {
            if (!SamuraiSettings.Instance.UseKaeshiGoken)
                return false;

            if (SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
                return false;

            return await Spells.KaeshiGoken.Cast(Core.Me.CurrentTarget);
        }


        /**********************************************************************************************
         *                                    Namikiri
         * ********************************************************************************************/
        public static async Task<bool> OgiNamikiri()
        {
            if (!SamuraiSettings.Instance.UseOgiNamikiri)
                return false;

            if (!Core.Me.HasAura(Auras.OgiReady))
                return false;

            var ogiReadyAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.OgiReady && x.CasterId == Core.Player.ObjectId);
            if (ogiReadyAura != null && ogiReadyAura.TimespanLeft.TotalMilliseconds <= 2700)
                return await Spells.OgiNamikiri.Cast(Core.Me.CurrentTarget);

            if (!SamuraiSettings.Instance.UseAoe || SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
            {
                if (!Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true, 20000))
                    return false;
            }

            if (!await Spells.OgiNamikiri.Cast(Core.Me.CurrentTarget))
                return false;

            SamuraiRoutine.InitializeFillerVar(false, false); // Remove Filler after Even Minutes Burst
            Logger.WriteInfo($@"[Filler] Remove Filler after Even Minutes Burst");

            return true;
        }

        public static async Task<bool> KaeshiNamikiri()
        {
            if (!SamuraiSettings.Instance.UseKaeshiNamikiri)
                return false;

            return await Spells.KaeshiNamikiri.Cast(Core.Me.CurrentTarget);
        }


    }
}
