using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using SamuraiRoutine = Magitek.Utilities.Routines.Samurai;

namespace Magitek.Logic.Samurai
{
    internal static class Buff
    {
        public static async Task<bool> MeikyoShisuiNotInCombat()
        {
            if (Core.Me.InCombat)
                return false;

            if (!Core.Me.HasTarget)
                return false;

            if (!SamuraiSettings.Instance.UseMeikyoShisui)
                return false;

            if (Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (Core.Me.HasAura(Auras.Shifu, true))
                return false;

            if (Core.Me.HasAura(Auras.Jinpu, true))
                return false;

            if (SamuraiRoutine.SenCount > 0)
                return false;

            if (ActionResourceManager.Samurai.Kenki > 0)
                return false;

            return await Spells.MeikyoShisui.CastAura(Core.Me, Auras.MeikyoShisui);
        }

        public static async Task<bool> MeikyoShisui()
        {
            if (!SamuraiSettings.Instance.UseMeikyoShisui)
                return false;

            if (Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (SamuraiSettings.Instance.UseMeikyoShisuiOnlyWithZeroSen && SamuraiRoutine.SenCount > 0)
                return false;

            if (SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
            {
                if (SamuraiRoutine.SenCount == 3)
                    return false;

                if (Casting.LastSpell != Spells.KaeshiSetsugekka && Casting.LastSpell != Spells.KaeshiHiganbana && Casting.LastSpell != null)
                    return false;
            } 
            else
            {
                if (SamuraiSettings.Instance.UseAoe)
                {
                    if (SamuraiRoutine.SenCount == 2)
                        return false;

                    if (ActionManager.LastSpell == SamuraiRoutine.Fuko || Casting.LastSpell == SamuraiRoutine.Fuko)
                        return false;

                    if (Spells.HissatsuGuren.IsKnownAndReady())
                        return false;
                }
            }

            if (!await Spells.MeikyoShisui.CastAura(Core.Me, Auras.MeikyoShisui))
                return false;

            SamuraiRoutine.InitializeFillerVar(true, false); //Initialize Filler after burst

            return true;
        }

        public static async Task<bool> ThirdEye()
        {
            return await Spells.ThirdEye.Cast(Core.Me);
        }


        public static async Task<bool> Ikishoten()
        {
            if (!SamuraiSettings.Instance.UseIkishoten)
                return false;

            if (ActionResourceManager.Samurai.Kenki >= 50)
                return false;

            if (!Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (!await Spells.Ikishoten.Cast(Core.Me))
                return false;

            SamuraiRoutine.InitializeFillerVar(false, false); // Remove Filler after Even Minutes Burst

            return true;
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.HissatsuSenei.IsKnown() && !Spells.HissatsuSenei.IsReady(4000))
                return false;

            return await PhysicalDps.UsePotion(SamuraiSettings.Instance);
        }
    }
}