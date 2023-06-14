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
using Magitek.Logic.Roles;
using Magitek.Models.DarkKnight;

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

                    //Damage Buff
                    if (await Buff.FightOrFlight()) return true;

                    //oGCDS
                    if (await SingleTarget.Requiescat()) return true;
                    if (await Aoe.CircleOfScorn()) return true;
                    if (await Aoe.Expiacion()) return true;
                    if (await SingleTarget.Intervene()) return true; //dash
                }

                if (await SingleTarget.ShieldLobOnLostAggro()) return true;
                if (await SingleTarget.GoringBlade()) return true;

                //Combo AOE (Single Target or Multi Target)
                if (await Aoe.BladeOfValor()) return true;
                if (await Aoe.BladeOfTruth()) return true;
                if (await Aoe.BladeOfFaith()) return true;
                if (await Aoe.Confiteor()) return true;

                //Under Divine Might Aura to have no cast or stacks of Sword Oath
                if (await Aoe.HolyCircle()) return true;
                if (await SingleTarget.HolySpirit()) return true;
                if (await SingleTarget.Atonement()) return true;

                //Combo Action AOE
                if (await Aoe.Prominence()) return true;
                if (await Aoe.TotalEclipse()) return true;

                //Combo Action Single Target
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

            if (await Tank.Guard(PaladinSettings.Instance)) return true;
            if (await Tank.Purify(PaladinSettings.Instance)) return true;
            if (await Tank.Recuperate(PaladinSettings.Instance)) return true;

            if (await Pvp.PhalanxPvp()) return true;
            if (await Pvp.BladeofValorPvp()) return true;
            if (await Pvp.BladeofTruthPvp()) return true;
            if (await Pvp.BladeofFaithPvp()) return true;
            if (await Pvp.HolySheltronPvp()) return true;

            if (!Tank.GuardCheck())
            {
                if (await Pvp.IntervenePvp()) return true;
                if (await Pvp.AtonementPvp()) return true;
                if (await Pvp.ShieldBashPvp()) return true;
                if (await Pvp.ConfiteorPvp()) return true;
            }

            if (await Pvp.RoyalAuthorityPvp()) return true;
            if (await Pvp.RiotBladePvp()) return true;

            return (await Pvp.FastBladePvp());
        }
    }
}
