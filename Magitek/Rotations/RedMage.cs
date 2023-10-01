using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.RedMage;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.BlackMage;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using RedMageRoutine = Magitek.Utilities.Routines.RedMage;
using Magitek.Utilities.Routines;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{

    public static class RedMage
    {
        static RedMage()
        {
            //StateMachineManager.RegisterStateMachine(RdmStateMachine.StateMachine);
        }

        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {


            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();
            if (WorldManager.InSanctuary)
                return false;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, (Core.Me.ClassLevel < 2 ? 3 : 20) + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Logic.RedMage.Heal.Verraise()) return true;
            return await Logic.RedMage.Heal.Vercure();
        }

        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> Combat()
        {
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, (Core.Me.ClassLevel < 2 ? 3 : 20) + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener())
                return true;

            //LimitBreak
            if (Aoe.ForceLimitBreak()) return true;

            if (RedMageRoutine.GlobalCooldown.CanWeave())
            {
                //Buffs
                if (await Buff.Acceleration()) return true;
                if (await Buff.Embolden()) return true;
                if (await Buff.Manafication()) return true;
                if (await Buff.Swiftcast()) return true;
                if (await Buff.LucidDreaming()) return true;

                //oGCD Abilities
                if (await Aoe.ContreSixte()) return true;
                if (await SingleTarget.Fleche()) return true;

                //Movement Abilities
                if (await SingleTarget.Engagement()) return true;
                if (await SingleTarget.Displacement()) return true;
                if (await SingleTarget.CorpsACorps()) return true;

                //TODO: implement this at some point
                //if (await Buff.MagickBarrier()) return true;
            }                

            //Melee
            if (await SingleTarget.Riposte()) return true;
            if (await SingleTarget.Zwerchhau()) return true;
            if (await SingleTarget.Redoublement()) return true;
            if (await SingleTarget.Reprise()) return true;
            
            if (RedMageSettings.Instance.UseAoe && Core.Me.CurrentTarget.EnemiesNearby(10).Count() >= RedMageSettings.Instance.AoeEnemies)
            {
                if (await Aoe.Veraero2()) return true;
                if (await Aoe.Verthunder2()) return true;
                if (await Aoe.Moulinet()) return true;
                if (await Aoe.Impact()) return true;
                if (await Aoe.Scatter()) return true;                
            }
            if (await SingleTarget.Verstone()) return true;
            if (await SingleTarget.Verthunder()) return true;
            if (await SingleTarget.Verfire()) return true;            
            if (await SingleTarget.Veraero()) return true;
            if (await SingleTarget.Verflare()) return true;
            if (await SingleTarget.Verholy()) return true;
            if (await SingleTarget.Scorch()) return true;
            if (await SingleTarget.Resolution ()) return true;
            if (await SingleTarget.Jolt()) return true;
            return false;
            //return await RdmStateMachine.StateMachine.Pulse();
        }

        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            if (await MagicDps.Guard(BlackMageSettings.Instance)) return true;
            if (await MagicDps.Purify(BlackMageSettings.Instance)) return true;
            if (await MagicDps.Recuperate(BlackMageSettings.Instance)) return true;

            if (await Pvp.DisplacementPvp()) return true;
            if (!MagicDps.GuardCheck())
            {
                if (await Pvp.SouthernCrossWhitePvp()) return true;
                if (await Pvp.VerHolyPvp()) return true;
                if (await Pvp.EnchantedRedoublementWhitePvp()) return true;
                if (await Pvp.EnchantedZwerchhauWhitePvp()) return true;
                if (await Pvp.EnchantedRiposteWhitePvp()) return true;

                if (await Pvp.SouthernCrossBlackPvp()) return true;
                if (await Pvp.VerFlarePvp()) return true;
                if (await Pvp.EnchantedRedoublementBlackPvp()) return true;
                if (await Pvp.EnchantedZwerchhauBlackPvp()) return true;
                if (await Pvp.EnchantedRiposteBlackPvp()) return true;

                if (await Pvp.CorpsacorpsPvp()) return true;

                if (await Pvp.ResolutionWhitePvp()) return true;
                if (await Pvp.ResolutionBlackPvp()) return true;
            }

            if (await Pvp.MagickBarrierPvp()) return true;
            if (await Pvp.FazzlePvp()) return true;

            return (await Pvp.VerstonePvp());
        }

        public static void RegisterCombatMessages()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                CombatMessages.RegisterCombatMessages(RdmStateMachine.StateMachine);
        }
    }
}
