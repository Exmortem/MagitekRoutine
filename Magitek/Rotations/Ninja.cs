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
using Magitek.Models.Samurai;
using Magitek.Models.Monk;

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

            if (!NinjaSettings.Instance.UseHutonOutsideOfCombat)
                return false;

            if (NinjaSettings.Instance.UseHutonOutsideOfCombat)
            {
                if (WorldManager.InSanctuary)
                    return false;
            }

            return Ninjutsu.Huton();
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

            //LimitBreak
            if (SingleTarget.ForceLimitBreak()) return true;

            //GCD Huton
            if (await SingleTarget.Huraijin()) return true;

            //Buff
            if (Ninjutsu.Huton()) return true;
            if (Ninjutsu.Suiton()) return true;

            //Utility
            if (await Utility.ForceShadeShift()) return true;
            if (await Utility.ForceTrueNorth()) return true;
            if (await Utility.ForceSecondWind()) return true;
            if (await Utility.ForceBloodBath()) return true;
            if (await Utility.ForceFeint()) return true;

            if (await PhysicalDps.Interrupt(NinjaSettings.Instance)) return true;
            if (await PhysicalDps.SecondWind(NinjaSettings.Instance)) return true;
            if (await PhysicalDps.Bloodbath(NinjaSettings.Instance)) return true;

            if (NinjaRoutine.GlobalCooldown.CanWeave(1))
            {
                if (await Utility.ShadeShift()) return true;
                if (await Utility.TrueNorth()) return true;
                if (await Buff.UsePotion()) return true;

                if (await Buff.Bunshin()) return true;
                if (await Buff.Meisui()) return true;
                if (await Buff.Kassatsu()) return true;

                if (await SingleTarget.Mug()) return true;
                if (await SingleTarget.TrickAttack()) return true;
            }

            //AOE
            if (await Aoe.PhantomKamaitachi()) return true;
            if (await Aoe.HellfrogMedium()) return true;
            if (Ninjutsu.GokaMekkyaku()) return true;
            if (Ninjutsu.Doton()) return true;
            if (Ninjutsu.Katon()) return true;

            //Single Target
            if (await SingleTarget.DreamWithinADream()) return true;
            if (await SingleTarget.FleetingRaiju()) return true;
            if (await SingleTarget.Bhavacakra()) return true;
            if (Ninjutsu.HyoshoRanryu()) return true;
            if (Ninjutsu.Raiton()) return true;
            if (Ninjutsu.FumaShuriken()) return true;

            //TCJ
            if (await Ninjutsu.TenChiJin()) return true;

            if (await Aoe.HakkeMujinsatsu()) return true;
            if (await Aoe.DeathBlossom()) return true;

            //Melee Combo
            if (await SingleTarget.ArmorCrush()) return true;
            if (await SingleTarget.AeolianEdge()) return true;
            if (await SingleTarget.GustSlash()) return true;
            return await SingleTarget.SpinningEdge();
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
