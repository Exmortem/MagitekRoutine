using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Ninja;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Ninja
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {


            await Casting.CheckForSuccessfulCast();

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            return true;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            return await GambitLogic.Gambit();
        }
        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }
        public static async Task<bool> Combat()
        {

            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            Utilities.Routines.Ninja.RefreshVars();
            
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!SpellQueueLogic.SpellQueue.Any())
                SpellQueueLogic.InSpellQueue = false;

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) 
                return true;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            #region Ninjustsus

            #region TenChiJin Nunjutsus

            if (await Ninjutsu.TenChiJin_FumaShuriken()) return true;
            if (await Ninjutsu.TenChiJin_Raiton()) return true;
            if (await Ninjutsu.TenChiJin_Suiton()) return true;

            #endregion

            #region Kassatsu Ninjutsus

            if (await Ninjutsu.HyoshoRanryu()) return true;

            #endregion

            if (await Ninjutsu.Huton()) return true;
            if (await Ninjutsu.Raiton()) return true;

            #endregion

            #region oGCD

            if (NinjaRoutine.GlobalCooldown.CanWeave())
            {
                if (await Ninjutsu.TenChiJin()) return true;
                if (await Buff.Kassatsu()) return true;
                //Ninki Spender
                if (await Buff.Bunshin()) return true;
            }

            #endregion

            #region GCD

            if (await SingleTarget.FleetingRaiju()) return true;
            if (await SingleTarget.ForkedRaiju()) return true;

            //Ninki Spender
            //Both missing logic for target count
            if (await SingleTarget.Bhavacakra()) return true;
            if (await Aoe.HellfrogMedium()) return true;

            //Non Ninki
            if (await Aoe.PhantomKamaitachi()) return true;

            if (await SingleTarget.ArmorCrush()) return true;
            if (await SingleTarget.AeolianEdge()) return true;
            if (await SingleTarget.GustSlash()) return true;
            if (await SingleTarget.SpinningEdge()) return true;

            #endregion

            return false;
            
        }

        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            if (await PhysicalDps.Guard(NinjaSettings.Instance)) return true;
            if (await PhysicalDps.Purify(NinjaSettings.Instance)) return true;
            if (await PhysicalDps.Recuperate(NinjaSettings.Instance)) return true;

            if (!PhysicalDps.GuardCheck()) { 
                if (await Pvp.SeitonTenchuPvp()) return true;
                if (await Pvp.AssassinatePvp()) return true;
                if (await Pvp.FleetingRaijuPvp()) return true;

                if (await Pvp.BunshinPvp()) return true;
                if (await Pvp.ShukuchiPvp()) return true;
                if (await Pvp.MugPvp()) return true;
                if (await Pvp.FumaShurikenPvp()) return true;

                if (await Pvp.HutonPvp()) return true;
                if (await Pvp.MeisuiPvp()) return true;

                if (await Pvp.DotonPvp()) return true;
                if (await Pvp.GokaMekkyakuPvp()) return true;

                if (await Pvp.HyoshoRanryuPvp()) return true;
                if (await Pvp.ForkedRaijuPvp()) return true;
                if (await Pvp.ThreeMudraPvp()) return true;
            }

            if (await Pvp.AeolianEdgePvp()) return true;
            if (await Pvp.GustSlashPvp()) return true;

            return (await Pvp.SpinningEdgePvp());

        }
    }
}
