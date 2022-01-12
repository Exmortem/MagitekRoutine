using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Summoner;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Summoner
    {
        public static async Task<bool> Rest()
        {
            if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 4)
                return false;

            return await Spells.SmnPhysick.Heal(Core.Me);
        }

        public static async Task<bool> PreCombatBuff()
        {


            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();
            SpellQueueLogic.SpellQueue.Clear();

            return await Pets.SummonCarbuncle();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }

                return await Spells.Ruin.Cast(Core.Me.CurrentTarget);
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit()) return true;
            if (await Logic.Summoner.Heal.Resurrection()) return true;
            #region Force Toggles
            if (await Logic.Summoner.Heal.ForceRaise()) return true;
            if (await Logic.Summoner.Heal.ForceHardRaise()) return true;
            #endregion

            if (await Logic.Summoner.Heal.RadiantAegis()) return true;
            return await Logic.Summoner.Heal.Physick();

        }
        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }
        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;


            if (await CustomOpenerLogic.Opener()) return true;

            if (Core.Me.CurrentTarget.HasAura(Auras.MagicResistance))
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await Buff.LucidDreaming()) return true;
            if (await Pets.SummonCarbuncleOrEgi()) return true;
            if (await Aoe.EnergySiphon()) return true;
            if (await SingleTarget.EnergyDrain()) return true;
            if (await SingleTarget.Enkindle()) return true;
            if (await Aoe.AstralFlow()) return true;
            if (await Aoe.CrimsonStrike()) return true;
            if (await Aoe.Painflare()) return true;
            if (await SingleTarget.Fester()) return true;
            if (await Buff.Aethercharge()) return true;
            if (await Aoe.Ruin4()) return true;
            if (await Aoe.Outburst()) return true;
            return await SingleTarget.Ruin();
        }
        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }
    }
}
