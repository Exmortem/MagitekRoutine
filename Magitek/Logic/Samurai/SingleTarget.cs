using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using Iaijutsu = ff14bot.Managers.ActionResourceManager.Samurai.Iaijutsu;

namespace Magitek.Logic.Samurai
{
    internal static class SingleTarget
    {

        public static async Task<bool> Enpi()
        {
            if (Core.Me.CurrentTarget == null) return false;

            if (!SamuraiSettings.Instance.Enpi)
                return false;

            if (Core.Me.CurrentTarget.Distance() < 10f && !Core.Me.HasAura(Auras.EnhancedEnpi))
                return false;

            return await Spells.Enpi.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuGuren()
        {
            if (Core.Me.ClassLevel >= 72)
                return false;

            if (Core.Me.CurrentTarget == null) return false;

            if (ActionResourceManager.Samurai.Kenki < 50)
                return false;

            if (!Core.Me.CurrentTarget.InView())
                return false;

            if (!Core.Me.HasAura(Auras.Jinpu))
                return false;

            return await Spells.HissatsuGuren.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuSenei()
        {
            if (Core.Me.CurrentTarget == null) return false;

            if (!SamuraiSettings.Instance.UseSenei)
                return false;

            if (SamuraiSettings.Instance.HissatsuSeneiOnlyWithJinpu && !Core.Me.HasAura(Auras.Jinpu))
                return false;

            if (Core.Me.ClassLevel < 72)
                return false;

            if (ActionResourceManager.Samurai.Kenki < 50)
                return false;

            //if we're about to refresh Higanbana or cast Midare, we need kenki for Kaiten
            if ((!Core.Target.HasAura(Auras.Higanbana, true, SamuraiSettings.Instance.HiganbanaRefreshTime) || Utilities.Routines.Samurai.SenCount == 3) && ActionResourceManager.Samurai.Kenki < 70)
                return false;

            return await Spells.HissatsuSenei.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuSeigan()
        {
            if (SamuraiSettings.Instance.HissatsuSeigan == false)
                return false;

            if (Core.Me.ClassLevel < 66)
                return false;

            if (!Core.Me.HasAura(Auras.EyesOpen))
                return false;

            if (ActionResourceManager.Samurai.Kenki < (15 + SamuraiSettings.Instance.ReservedKenki))
                return false;

            if (Utilities.Routines.Samurai.SenCount == 1 && !Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true, SamuraiSettings.Instance.HiganbanaRefreshTime * 1000))
                return false;

            if (Utilities.Routines.Samurai.SenCount >= 2 && ActionResourceManager.Samurai.Kenki < (45 + SamuraiSettings.Instance.ReservedKenki))
                return false;

            //Prevent using seigan here to stop double weaves
            if (Utilities.Routines.Samurai.SenCount == 3 && ActionResourceManager.Samurai.Kenki <= 90)
                return false;

            return await Spells.HissatsuSeigan.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Shoha()
        {
            if (Core.Me.ClassLevel < 80)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            //if (!Core.Me.HasAura("Meditation")) //TODO: Fix with aura ID when known
            //return false;

            return await Spells.Shoha.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> KaeshiSetsugekka()
        {
            if (Core.Me.ClassLevel < 76)
                return false;

            return await Spells.KaeshiSetsugekka.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HissatsuShinten()
        {
            if (Casting.LastSpell == Spells.HissatsuSenei)
                return false;

            if (Core.Me.ClassLevel < 62)
                return false;

            if (Utilities.Routines.Samurai.SenCount == 3)
                return false;


            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionResourceManager.Samurai.Kenki < (25 + SamuraiSettings.Instance.ReservedKenki))
                return false;


            if (Core.Me.ClassLevel > 70 && Spells.HissatsuGuren.Cooldown.TotalMilliseconds <= 10000 && ActionResourceManager.Samurai.Kenki < 90)
                return false;

            if (Utilities.Routines.Samurai.SenCount == 1 && !Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true, SamuraiSettings.Instance.HiganbanaRefreshTime * 1000))
                return false;

            if (Utilities.Routines.Samurai.SenCount >= 2 && ActionResourceManager.Samurai.Kenki < (45 + SamuraiSettings.Instance.ReservedKenki))
                return false;

            //Prevent using shinten here to stop double weaves
            if (Utilities.Routines.Samurai.SenCount == 3 && ActionResourceManager.Samurai.Kenki <= 90)
                return false;

            return await Spells.HissatsuShinten.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MidareSetsugekka()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (Core.Me.ClassLevel >= 62 && !Core.Me.HasAura(Auras.Kaiten) && ActionResourceManager.Samurai.Kenki < 20)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 3)
                return false;

            if (Utilities.Routines.Samurai.SenCount != 3)
                return false;

            if (Core.Me.ClassLevel >= 52 && !Core.Me.HasAura(Auras.Kaiten))
                return await Spells.HissatsuKaiten.Cast(Core.Me) || Casting.LastSpell == Spells.HissatsuKaiten;

            if (BaseSettings.Instance.DebugPlayerCasting)
                Logger.WriteInfo($"Already have Kaiten buff or too low level to cast, casting {Spells.MidareSetsugekka.LocalizedName} alone...");

            var casted = await Spells.MidareSetsugekka.Cast(Core.Me.CurrentTarget);
            var hasKaiten = Core.Me.HasAura(Auras.Kaiten);

            //If we get interrupted casting Midare and still have Kaiten up, wait until we either succeed casting it, or we lose Kaiten
            return casted || hasKaiten;

        }

        public static async Task<bool> Higanbana()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (!SamuraiSettings.Instance.UseHigabana)
                return false;

            if (Core.Me.CurrentTarget.CombatTimeLeft() < 40)
                return false;

            if (SamuraiSettings.Instance.HiganbanaOnlyWithJinpu && !Core.Me.HasAura(Auras.Jinpu))
                return false;

            if (Utilities.Combat.Enemies.Count(x => x.InView() && x.Distance(Core.Me) <= 6 + x.CombatReach) >= SamuraiSettings.Instance.AoeComboEnemies)
            {
                if (SamuraiSettings.Instance.AoeCombo)
                    return false;
            }

            if (Core.Me.ClassLevel >= 52 && !Core.Me.HasAura(Auras.Kaiten) && ActionResourceManager.Samurai.Kenki < 20)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 3)
                return false;

            if (Utilities.Routines.Samurai.SenCount != 1)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true, SamuraiSettings.Instance.HiganbanaRefreshTime * 1000))
                return false;

            if (Core.Me.ClassLevel >= 52 && !Core.Me.HasAura(Auras.Kaiten))
                return await Spells.HissatsuKaiten.Cast(Core.Me) || Casting.LastSpell == Spells.HissatsuKaiten;

            if (BaseSettings.Instance.DebugPlayerCasting) Logger.WriteInfo($"Already have Kaiten buff or too low level to cast, casting {Spells.Higanbana.LocalizedName} alone...");

            //If we get interrupted casting Higanbana and still have Kaiten up, wait until we either succeed casting it, or we lose Kaiten
            return await Spells.Higanbana.Cast(Core.Me.CurrentTarget) || Core.Me.HasAura(Auras.Kaiten);
        }

        // public static async Task<bool> KaeshiHiganbana()  DON'T USE KAESHIHIGANBANA EVER
        // {
        //     if (Core.Me.CurrentTarget == null)
        //         return false;
        //
        //     if (Core.Me.ClassLevel < 76)
        //         return false;
        //
        //     if (Core.Me.CurrentTarget.CombatTimeLeft() < 15)
        //         return false;
        //
        //     if (Utilities.Combat.Enemies.Count(x => x.InView() && x.Distance(Core.Me) <= 6 + x.CombatReach) >= SamuraiSettings.Instance.AoeComboEnemies)
        //    {
        //        if (SamuraiSettings.Instance.AoeCombo)
        //          return false;
        //}

        // if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 3)
        //   return false;

        // if (Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true))
        //   return false;

        //  return await Spells.KaeshiHiganbana.Cast(Core.Me.CurrentTarget) || Core.Me.HasAura(Auras.Kaiten);
        // }

        public static async Task<bool> Kasha()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Ka))
                return false;

            if (ActionManager.LastSpell != Spells.Shifu && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (!Core.Me.CurrentTarget.IsFlanking)
            {
                ViewModels.BaseSettings.Instance.PositionalStatus = "OutOfPosition";
                ViewModels.BaseSettings.Instance.PositionalText = "Move To Flank";
            }
            else
            {
                ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
                ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
            }

            // Normal spell cast if we don't have Meikyo
            if (!Core.Me.HasAura(Auras.MeikyoShisui) || !Utilities.Routines.Samurai.CastDuringMeikyo.Any())
            {
                if (!await Spells.Kasha.Cast(Core.Me.CurrentTarget))
                    return false;
            }
            else
            {
                // We have Meikyo so we wanna cast abilities in order
                var nextMeikyoSpell = Utilities.Routines.Samurai.CastDuringMeikyo.Peek();

                if (nextMeikyoSpell != Spells.Kasha)
                    return false;

                if (await nextMeikyoSpell.Cast(Core.Me.CurrentTarget))
                {
                    Logger.WriteInfo($@"Used Kasha With Meikyo");
                    Utilities.Routines.Samurai.CastDuringMeikyo.Dequeue();
                }
                else
                {
                    return false;
                }
            }

            ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
            ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
            return true;
        }

        public static async Task<bool> Shifu()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.Hakaze)
                return false;

            return await Spells.Shifu.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Gekko()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Getsu))
                return false;

            if (ActionManager.LastSpell != Spells.Jinpu && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (!Core.Me.CurrentTarget.IsBehind)
            {
                ViewModels.BaseSettings.Instance.PositionalStatus = "OutOfPosition";
                ViewModels.BaseSettings.Instance.PositionalText = "Move To Rear";
            }
            else
            {
                ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
                ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
            }

            // Normal spell cast if we don't have Meikyo
            if (!Core.Me.HasAura(Auras.MeikyoShisui) || !Utilities.Routines.Samurai.CastDuringMeikyo.Any())
            {
                if (!await Spells.Gekko.Cast(Core.Me.CurrentTarget))
                    return false;
            }
            else
            {
                // We have Meikyo so we wanna cast abilities in order
                var nextMeikyoSpell = Utilities.Routines.Samurai.CastDuringMeikyo.Peek();

                if (nextMeikyoSpell != Spells.Gekko)
                    return false;

                if (await nextMeikyoSpell.Cast(Core.Me.CurrentTarget))
                {
                    Logger.WriteInfo($@"Used Gekko With Meikyo");
                    Utilities.Routines.Samurai.CastDuringMeikyo.Dequeue();
                }
                else
                {
                    return false;
                }
            }

            ViewModels.BaseSettings.Instance.PositionalStatus = "InPosition";
            ViewModels.BaseSettings.Instance.PositionalText = "You're In Position";
            return true;
        }

        public static async Task<bool> Jinpu()
        {
            if (Core.Me.CurrentTarget == null) return false;
            if (ActionManager.LastSpell != Spells.Hakaze) return false;

            return await Spells.Jinpu.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Yukikaze()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            //If < 62 the only way to gain kenki is by completing combos
            if (ActionResourceManager.Samurai.Sen.HasFlag(Iaijutsu.Setsu) && Core.Me.ClassLevel > 62)
                return false;
            if (Core.Me.ClassLevel == 80 && Utilities.Routines.Samurai.SenCount != 2)
                return false;

            if (ActionManager.LastSpell != Spells.Hakaze && !Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (!Core.Me.HasAura(Auras.Jinpu, true, 4000) || !Core.Me.HasAura(Auras.Shifu, true, 4000))
                return false;

            if (Utilities.Routines.Samurai.SenCount == 1 && !Core.Me.CurrentTarget.HasAura(Auras.Higanbana, true, SamuraiSettings.Instance.HiganbanaRefreshTime))
                return false;

            // Normal spell cast if we don't have Meikyo
            if (!Core.Me.HasAura(Auras.MeikyoShisui) || !Utilities.Routines.Samurai.CastDuringMeikyo.Any())
            {
                if (!await Spells.Yukikaze.Cast(Core.Me.CurrentTarget))
                    return false;
            }
            else
            {
                // We have Meikyo so we wanna cast abilities in order
                var nextMeikyoSpell = Utilities.Routines.Samurai.CastDuringMeikyo.Peek();

                if (nextMeikyoSpell != Spells.Yukikaze)
                    return false;

                if (await nextMeikyoSpell.Cast(Core.Me.CurrentTarget))
                {
                    Logger.WriteInfo($@"Used Yukikaze With Meikyo");
                    Utilities.Routines.Samurai.CastDuringMeikyo.Dequeue();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static async Task<bool> Hakaze()
        {
            if (Core.Me.CurrentTarget == null) return false;
            return await Spells.Hakaze.Cast(Core.Me.CurrentTarget);
        }
    }
}