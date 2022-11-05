using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Paladin;
using Magitek.Models.Account;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;
using Healing = Magitek.Logic.Paladin.Heal;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Rotations
{
    public static class Paladin
    {
        public static Task<bool> Rest()
        {
            var needRest = Core.Me.CurrentHealthPercent < PaladinSettings.Instance.RestHealthPercent;
            return Task.FromResult(needRest);
        }

        public static async Task<bool> PreCombatBuff()
        {
            await Casting.CheckForSuccessfulCast();

            if (Core.Me.IsMounted)
                return false;

            if (WorldManager.InSanctuary)
                return false;

            return await Buff.IronWill();
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
            if (Core.Me.IsMounted)
                return true;

            if (await GambitLogic.Gambit())
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Healing.Clemency();
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
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener())
                return true;

            //LimitBreak
            if (Defensive.ForceLimitBreak()) return true;

            if (!Core.Me.HasAura(Auras.PassageOfArms))
            {
                //Utility
                if (await SingleTarget.Interrupt()) return true;
                if (await Buff.IronWill()) return true;

                if (PaladinRoutine.GlobalCooldown.CanWeave())
                {
                    //Potion
                    if (await Buff.UsePotion()) return true;

                    //Defensive Buff
                    if (await Defensive.HallowedGround()) return true;
                    if (await Defensive.Sentinel()) return true;
                    if (await Defensive.Rampart()) return true;
                    if (await Defensive.Reprisal()) return true;
                    if (await Defensive.Sheltron()) return true;
                    if (await Defensive.DivineVeil()) return true;

                    //Cover
                    if (await Defensive.Intervention()) return true;
                    if (await Defensive.Cover()) return true;

                    //oGCDS
                    if (await SingleTarget.Intervene()) return true; //dash
                    if (await Aoe.CircleOfScorn()) return true;
                    if (await Aoe.Expiacion()) return true;
                    if (await SingleTarget.Requiescat()) return true;

                    //Damage Buff (was before oGCD)
                    if (await Buff.FightOrFlight()) return true;
                }

                if (await SingleTarget.ShieldLobOnLostAggro()) return true;

                //Combo AOE (Single Target or Multi Target)
                if (await Aoe.BladeOfValor()) return true;
                if (await Aoe.BladeOfTruth()) return true;
                if (await Aoe.BladeOfFaith()) return true;
                if (await Aoe.Confiteor()) return true;

                //Requiescat
                if (await Aoe.HolyCircle()) return true;
                if (await SingleTarget.HolySpirit()) return true;

                //Combo AOE (Multi Target only)
                if (await Aoe.Prominence()) return true;
                if (await Aoe.TotalEclipse()) return true;

                //Combo
                if (await SingleTarget.Atonement()) return true;
                if (await SingleTarget.GoringBlade()) return true;
                if (await SingleTarget.RoyalAuthority()) return true;
                if (await SingleTarget.RiotBlade()) return true;
                if (await SingleTarget.FastBlade()) return true;

                return await SingleTarget.ShieldLob();
            }
            else
            {
                return false;
            }
        }
        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            return false;
        }
    }
}
