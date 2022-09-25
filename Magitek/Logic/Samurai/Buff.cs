using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
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
        public static async Task<bool> MeikyoShisui()
        {
            if (!SamuraiSettings.Instance.UseMeikyoShisui)
                return false;

            if (Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (SamuraiSettings.Instance.UseMeikyoShisuiOnlyWithZeroSen && SamuraiRoutine.SenCount > 0)
                return false;

            if (!Core.Me.HasAura(Auras.Jinpu, true, 7000) || !Core.Me.HasAura(Auras.Shifu, true, 7000))
                return false;

            if (SamuraiRoutine.AoeEnemies5Yards < SamuraiSettings.Instance.AoeEnemies)
            {
                if (ActionManager.LastSpell == Spells.Hakaze || ActionManager.LastSpell == Spells.Shifu || ActionManager.LastSpell == Spells.Jinpu
                || Casting.LastSpell == Spells.Hakaze || Casting.LastSpell == Spells.Shifu || Casting.LastSpell == Spells.Jinpu)
                    return false;

                if (SamuraiRoutine.SenCount == 3)
                    return false;

                if (Spells.HissatsuSenei.IsKnownAndReady())
                    return false;

                if (Casting.LastSpell != Spells.KaeshiSetsugekka && Casting.LastSpell != Spells.KaeshiHiganbana)
                    return false;
            } else
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
    }
}