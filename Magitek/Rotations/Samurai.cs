using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Roles;
using Magitek.Logic.Samurai;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Samurai
    {
        public static async Task<bool> Rest()
        {
            return Core.Me.CurrentHealthPercent < 75 || Core.Me.CurrentManaPercent < 50;
        }

        public static async Task<bool> PreCombatBuff()
        {


            await Casting.CheckForSuccessfulCast();


            return false;
        }

        public static async Task<bool> Pull()
        {
            if (!BotManager.Current.IsAutonomous)
            {
                return await Combat();
            }

            Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);

            return await Combat();
        }
        public static async Task<bool> Heal()
        {


            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            return await GambitLogic.Gambit();
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            Utilities.Routines.Samurai.RefreshVars();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await GambitLogic.Gambit()) return true;

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;
            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }
            // Shoha must come out before kaeshi or you overcap

            if (await SingleTarget.KaeshiSetsugekka()) return true;
            if (await Aoe.KaeshiGoken()) return true;

            if (Utilities.Routines.Samurai.OnGcd)
            {
                //Only cast spells that are instant/off gcd
                if (await PhysicalDps.Interrupt(SamuraiSettings.Instance)) return true;
                if (await SingleTarget.MidareSetsugekka()) return true;
                if (await Aoe.TenkaGoken()) return true;
                if (await SingleTarget.Higanbana()) return true;
                if (await Buff.BloodBath()) return true;
                if (await Aoe.HissatsuKyuten()) return true;
                if (await Aoe.HissatsuGuren()) return true;
                if (await SingleTarget.HissatsuSenei()) return true;
                if (await SingleTarget.HissatsuSeigan()) return true;
                if (await SingleTarget.HissatsuShinten()) return true;
                if (await SingleTarget.HissatsuGuren()) return true;
                if (await Buff.Ikishoten()) return true;
                if (await Buff.SecondWind()) return true;
                if (await Buff.TrueNorth()) return true;
                if (await Buff.MeikyoShisui()) return true;

            }

            if (await SingleTarget.MidareSetsugekka()) return true;
            if (await SingleTarget.Shoha()) return true;
            if (await Aoe.TenkaGoken()) return true;
            if (await Aoe.Oka()) return true;
            if (await Aoe.Mangetsu()) return true;
            if (await Aoe.Fuga()) return true;

            if (await SingleTarget.Higanbana()) return true;



            if (await SingleTarget.Gekko()) return true;
            if (await SingleTarget.Kasha()) return true;
            if (await SingleTarget.Yukikaze()) return true;

            #region Last Spell Hakaze

            if (ActionManager.LastSpell == Spells.Hakaze)
            {
                if(Core.Me.ClassLevel < 18)
                {
                    if (await SingleTarget.Jinpu()) return true;
                }

                var hasKa = ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Ka);
                var hasGetsu = ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Getsu);
                var hasSetsu = ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Setsu);

                if (hasGetsu && !hasKa || !hasGetsu && hasKa)
                {
                    if (!Core.Me.HasAura(Auras.Shifu, true, 5000) || !ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Ka))
                    {
                        if (await SingleTarget.Shifu())
                        {
                            ViewModels.BaseSettings.Instance.PositionalText = "Flank Position Next";
                            return true;
                        }
                    }
                    if (!Core.Me.HasAura(Auras.Jinpu, true, 5000) || !ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Getsu))
                    {
                        if (await SingleTarget.Jinpu())
                        {
                            ViewModels.BaseSettings.Instance.PositionalText = "Rear Position Next";
                            return true;
                        }
                    }
                }

                var shifuAuraTimeleft = Core.Me.GetAuraById(Auras.Shifu)?.TimeLeft;
                var jinpuAuraTimeleft = Core.Me.GetAuraById(Auras.Jinpu)?.TimeLeft;

                if (!shifuAuraTimeleft.HasValue)
                {
                    if (await SingleTarget.Shifu())
                    {
                        ViewModels.BaseSettings.Instance.PositionalText = "Flank Position Next";
                        return true;
                    }
                }
                else if (!jinpuAuraTimeleft.HasValue)
                {
                    if (await SingleTarget.Jinpu())
                    {
                        ViewModels.BaseSettings.Instance.PositionalText = "Rear Position Next";
                        return true;
                    }
                }
                else if (shifuAuraTimeleft.Value > jinpuAuraTimeleft.Value)
                {
                    if (await SingleTarget.Jinpu())
                    {
                        ViewModels.BaseSettings.Instance.PositionalText = "Rear Position Next";
                        return true;
                    }
                }
                else
                {
                    if (await SingleTarget.Shifu())
                    {
                        ViewModels.BaseSettings.Instance.PositionalText = "Flank Position Next";
                        return true;
                    }
                }
            }

            #endregion

            if (await SingleTarget.Hakaze()) return true; ;

            return await SingleTarget.Enpi();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
