using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Roles;
using Magitek.Logic.Samurai;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using SamuraiRoutine = Magitek.Utilities.Routines.Samurai;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{

    public static class Samurai
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(Core.Me.CurrentHealthPercent < 75 || Core.Me.CurrentManaPercent < 50);
        }

        public static async Task<bool> PreCombatBuff()
        {
            await Casting.CheckForSuccessfulCast();

            if (WorldManager.InSanctuary)
                return false;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.CurrentTarget.CombatReach);

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) 
                return true;

            await Casting.CheckForSuccessfulCast();

            return await GambitLogic.Gambit();
        }

        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> Combat()
        {
            SamuraiRoutine.RefreshVars();

            if (BotManager.Current.IsAutonomous)
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await GambitLogic.Gambit()) 
                return true;

            if (!SpellQueueLogic.SpellQueue.Any())
                SpellQueueLogic.InSpellQueue = false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener()) 
                return true;

            if (SpellQueueLogic.SpellQueue.Any())
                if (await SpellQueueLogic.SpellQueueMethod()) 
                    return true;

            //Buff for opener
            if (await Buff.MeikyoShisuiNotInCombat()) return true;

            //Utility
            if (await PhysicalDps.Interrupt(SamuraiSettings.Instance)) return true;
            if (await PhysicalDps.SecondWind(SamuraiSettings.Instance)) return true;
            if (await PhysicalDps.Bloodbath(SamuraiSettings.Instance)) return true;
            if (Utility.ForceLimitBreak()) return true;

            if (SamuraiRoutine.GlobalCooldown.CanWeave())
            {
                //Utility
                if (await Utility.TrueNorth()) return true;
                if (await Utility.Hagakure()) return true;

                //Buffs
                if (await Buff.MeikyoShisui()) return true;
                if (await Buff.Ikishoten()) return true;

                //oGCD Meditation
                if (await Aoe.ShohaII()) return true; 
                if (await SingleTarget.Shoha()) return true;

                //oGCD Kenki - AOE
                if (await Aoe.HissatsuGuren()) return true; //share recast time with Senei
                if (await Aoe.HissatsuKyuten()) return true;

                //oGCD Kenki - SingleTarget
                if (await SingleTarget.HissatsuShinten()) return true; 
                if (await SingleTarget.HissatsuSenei()) return true; //share recast time with Guren
                if (await SingleTarget.HissatsuGyoten()) return true; //dash forward
                //if (await SingleTarget.HissatsuYaten()) return true; //dash backward
            }

            //manage case when midare cast is stopped... dont want to use another GCD instead

            //Namikiri
            if (await Aoe.OgiNamikiri()) return true;
            if (await Aoe.KaeshiNamikiri()) return true;

            //Tsubame Gaeshi
            if (await SingleTarget.KaeshiSetsugekka()) return true;
            if (await Aoe.KaeshiGoken()) return true;
            if (await SingleTarget.KaeshiHiganbana()) return true;

            //Iaijutsu
            if (await SingleTarget.MidareSetsugekka()) return true;
            if (await Aoe.TenkaGoken()) return true;
            if (await SingleTarget.Higanbana()) return true;

            if (SamuraiRoutine.iaijutsuSuccessful)
            {
                //Combo AOE
                if (await Aoe.Mangetsu()) return true;
                if (await Aoe.Oka()) return true;
                if (await Aoe.Fuko()) return true;

                //3 Combos Single Target
                if (await SingleTarget.Gekko()) return true;
                if (await SingleTarget.Kasha()) return true;
                if (await SingleTarget.Yukikaze()) return true;
                if (await SingleTarget.Jinpu()) return true;
                if (await SingleTarget.Shifu()) return true;
                if (await SingleTarget.Hakaze()) return true;

                if (await SingleTarget.Enpi()) return true;

            }

            return false;
        }

        public static void RegisterCombatMessages()
        {
            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat || !Core.Me.HasTarget));

            //Second priority: Don't show anything if positional requirements are Nulled
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "",
                                          () => SamuraiSettings.Instance.HidePositionalMessage || Core.Me.HasAura(Auras.TrueNorth) || SamuraiSettings.Instance.EnemyIsOmni));

            //Third priority : Positional
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Gekko => BEHIND !!",
                                          "/Magitek;component/Resources/Images/General/ArrowDownHighlighted.png",
                                          () => !Core.Me.CurrentTarget.IsBehind && (ActionManager.LastSpell == Spells.Jinpu
                                                    || (Core.Me.HasAura(Auras.MeikyoShisui) && Casting.LastSpell == Spells.MeikyoShisui))));

            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Kasha => SIDE !!!",
                                          "/Magitek;component/Resources/Images/General/ArrowSidesHighlighted.png",
                                          () => !Core.Me.CurrentTarget.IsFlanking && (ActionManager.LastSpell == Spells.Shifu
                                                    || (Core.Me.HasAura(Auras.MeikyoShisui) && ActionManager.LastSpell == Spells.Gekko))));
        }

        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }
    }
}
