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
using Magitek.Utilities.GamelogManager;
using ff14bot.Helpers;
using System.Windows.Forms;
using System;

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

            Utilities.Routines.Ninja.RefreshVars();

            if (await Ninjutsu.PrePullHutonRamp()) return true;
            if (await Ninjutsu.PrePullHutonUse()) return true;
            if (await Utility.PrePullHide()) return true;
            
            if (await Ninjutsu.PrePullSuitonRamp()) return true;
            if (await Ninjutsu.PrePullSuitonUse()) return true;

            if (GamelogManagerCountdown.IsCountdownRunning())
                return true;

            return false;
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

            if (GamelogManagerCountdown.IsCountdownRunning())
                return true;

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

            if (NinjaRoutine.GlobalCooldown.CountOGCDs() < 2 && Spells.SpinningEdge.Cooldown.TotalMilliseconds >= 770
                && DateTime.Now >= NinjaRoutine.oGCD)
            {

                bool usedOGCD = false;

                if (!usedOGCD && await Buff.Kassatsu()) usedOGCD = true;
                if (!usedOGCD && await Cooldown.Mug()) usedOGCD = true;
                if (!usedOGCD && await Cooldown.TrickAttack()) usedOGCD = true;
                if (!usedOGCD && await Ninjutsu.TenChiJin()) usedOGCD = true;
                if (!usedOGCD && await Cooldown.DreamWithinaDream()) usedOGCD = true;
                if (!usedOGCD && await Buff.Meisui()) usedOGCD = true;
                if (!usedOGCD && await SingleTarget.Bhavacakra()) usedOGCD = true;
                if (!usedOGCD && await Aoe.HellfrogMedium()) usedOGCD = true;
                if (!usedOGCD && await Buff.Bunshin()) usedOGCD = true;

                if (usedOGCD)
                {

                    NinjaRoutine.oGCD = DateTime.Now.AddMilliseconds(770);
                    return true;

                }
            }

            if (await Ninjutsu.TenChiJin_FumaShuriken()) return true;
            if (await Ninjutsu.TenChiJin_Raiton()) return true;
            if (await Ninjutsu.TenChiJin_Suiton()) return true;

            if (await Ninjutsu.Huton()) return true;
            if (await Ninjutsu.HyoshoRanryu()) return true;
            if (await Ninjutsu.Suiton()) return true;
            if (await Ninjutsu.Raiton()) return true;

            if (await SingleTarget.FleetingRaiju()) return true;
            if (await SingleTarget.ForkedRaiju()) return true;

            if (await Aoe.PhantomKamaitachi()) return true;

            if (await Buff.Huraijin()) return true;

            if (await SingleTarget.ArmorCrush()) return true;
            if (await SingleTarget.AeolianEdge()) return true;
            if (await SingleTarget.GustSlash()) return true;
            if (await SingleTarget.SpinningEdge()) return true;

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
