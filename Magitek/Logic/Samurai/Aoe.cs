using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.QueueSpell;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using Iaijutsu = ff14bot.Managers.ActionResourceManager.Samurai.Iaijutsu;
using System;

namespace Magitek.Logic.Samurai
{
    internal static class Aoe
    {
        public static async Task<bool> Fuga()
        {
            if (SamuraiSettings.Instance.OnlyAoeComboWithJinpuShifu && (!Core.Me.HasAura(Auras.Shifu) || !Core.Me.HasAura(Auras.Jinpu)))
                return false;

            if (!SamuraiSettings.Instance.AoeCombo)
                return false;

            if (Utilities.Routines.Samurai.SenCount == 2)
                return false;

            //if (!Core.Me.HasAura(Auras.Jinpu, true, 4000) || !Core.Me.HasAura(Auras.Shifu, true, 4000))
            //    return false;

            if (SamuraiSettings.Instance.UseConeBasedAoECalculationMethod)
            {
                if (Core.Me.EnemiesInCone(5.5f) < SamuraiSettings.Instance.AoeComboEnemies)
                    return false;
            }
            else
            {
                if (Utilities.Routines.Samurai.AoeEnemies5Yards < SamuraiSettings.Instance.AoeComboEnemies)
                    return false;
            }

            return await Spells.Fuga.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Oka()
        {
            if (Utilities.Routines.Samurai.SenCount == 2)
                return false;

            if (ActionManager.LastSpell != Spells.Fuga && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Ka))
                return false;
            if (!Core.Me.HasAura(Auras.Jinpu, true, 7000))
                return false;
            if (Utilities.Routines.Samurai.AoeEnemies5Yards < SamuraiSettings.Instance.AoeComboEnemies)
                return false;
            
            return await Spells.Oka.Cast(Core.Me);
        }

        public static async Task<bool> Mangetsu()
        {
            if (Utilities.Routines.Samurai.SenCount == 2)
                return false;

            if (ActionManager.LastSpell != Spells.Fuga && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Getsu))
                return false;
            if (!Core.Me.HasAura(Auras.Shifu, true, 7000))
                return false;
            if (Utilities.Routines.Samurai.AoeEnemies5Yards < SamuraiSettings.Instance.AoeComboEnemies)
                return false;

            return await Spells.Mangetsu.Cast(Core.Me);
        }

        public static async Task<bool> HissatsuGuren()
        {
            if (!SamuraiSettings.Instance.HissatsuGuren)
                return false;

            if (SamuraiSettings.Instance.HissatsuGurenOnlyWithJinpu && !Core.Me.HasAura(Auras.Jinpu))
                return false;

            if (Core.Me.CurrentTarget == null) return false;

            if (ActionResourceManager.Samurai.Kenki < 50)
                return false;

            if (!Core.Me.CurrentTarget.InView())
                return false;

            if (Combat.Enemies.Count(x => x.InView() && x.Distance(Core.Me) <= 10 + x.CombatReach) < SamuraiSettings.Instance.HissatsuGurenEnemies)
                return false;

            return await Spells.HissatsuGuren.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> KaeshiGoken()
        {
            if (Core.Me.ClassLevel < 76)
                return false;

            if (Casting.LastSpell != Spells.TenkaGoken || (DateTime.Now - Casting.LastSpellTimeFinished) > TimeSpan.FromSeconds(14))
                return false;

            if (SamuraiSettings.Instance.UseConeBasedAoECalculationMethod)
            {
                if (Core.Me.EnemiesInCone(10) < SamuraiSettings.Instance.TenkaGokenEnemies)
                    return false;
            }
            else
            {
                if (Utilities.Routines.Samurai.AoeEnemies8Yards < SamuraiSettings.Instance.TenkaGokenEnemies)
                    return false;
            }

            if (Core.Me.CurrentTarget.Distance(Core.Me) + Core.Me.CurrentTarget.CombatReach > 10)
                return false;

            //Don't go further down the tree, wait for Tsubame if we're over level 76
            return await Spells.KaeshiGoken.Cast(Core.Me.CurrentTarget) || (Spells.KaeshiGoken.Cooldown.TotalMilliseconds <= 2000 && Casting.LastSpell == Spells.TenkaGoken);
        }

        public static async Task<bool> TenkaGoken()
        {
            if (!SamuraiSettings.Instance.TenkaGoken)
                return false;

            if (Utilities.Routines.Samurai.SenCount != 2)
                return false;

            //if (!Core.Me.HasAura(Auras.Jinpu, true, 4000) || !Core.Me.HasAura(Auras.Shifu, true, 4000))
            //    return false;
            if (SamuraiSettings.Instance.UseConeBasedAoECalculationMethod)
            {
                if (Core.Me.EnemiesInCone(10) < SamuraiSettings.Instance.TenkaGokenEnemies)
                    return false;
            }
            else
            {
                if (Utilities.Routines.Samurai.AoeEnemies8Yards < SamuraiSettings.Instance.TenkaGokenEnemies)
                    return false;
            }

            if (Core.Me.CurrentTarget.Distance(Core.Me) + Core.Me.CurrentTarget.CombatReach > 10)
                return false;

            if (Core.Me.ClassLevel < 52)
                return await Spells.TenkaGoken.Cast(Core.Me.CurrentTarget);

            if (SamuraiSettings.Instance.OnlyUseTenkaGokenWithKaiten && !Core.Me.HasAura(Auras.Kaiten) && ActionResourceManager.Samurai.Kenki < 20)
                return false;

            if (!Core.Me.HasAura(Auras.Kaiten) && ActionResourceManager.Samurai.Kenki >= 20)
                return await Spells.HissatsuKaiten.Cast(Core.Me) || Casting.LastSpell == Spells.HissatsuKaiten;

            if (SamuraiSettings.Instance.OnlyUseTenkaGokenWithKaiten && !Core.Me.HasAura(Auras.Kaiten))
                return false;

            return await Spells.TenkaGoken.Cast(Core.Me.CurrentTarget) || Core.Me.HasAura(Auras.Kaiten);
        }

        public static async Task<bool> HissatsuKyuten()
        {
            if (!SamuraiSettings.Instance.HissatsuKyuten)
                return false;

            if (ActionManager.HasSpell(Spells.HissatsuGuren.Id) && SamuraiSettings.Instance.HissatsuGuren && Spells.HissatsuGuren.Cooldown < TimeSpan.FromSeconds(2))
                return false;

            if (ActionResourceManager.Samurai.Kenki < 45)
                return false;

            if (Utilities.Routines.Samurai.AoeEnemies5Yards < SamuraiSettings.Instance.HissatsuKyutenEnemies)
                return false;

            return await Spells.HissatsuKyuten.Cast(Core.Me.CurrentTarget);
        }
    }
}