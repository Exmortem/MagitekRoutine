using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Summoner;
using Magitek.Utilities;

namespace Magitek.Rotations
{
    public static class Summoner
    {
        public static async Task<bool> Rest()
        {
            if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 4)
                return false;

            return await Spells.Physick.Heal(Core.Me);
        }

        public static async Task<bool> PreCombatBuff()
        {
            

            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();
            SpellQueueLogic.SpellQueue.Clear();

            return await Pets.Summon();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }

                return await Spells.SmnBio.CastAura(Core.Me.CurrentTarget, Auras.Bio);
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

            return await Logic.Summoner.Heal.Physick();
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
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

            //Logger.Write("Aetherflow Count: " + MagitekActionResourceManager.Arcanist.Aetherflow);
            //Logger.Write("Can Trance: " + MagitekActionResourceManager.Arcanist.CanTrance);
            //Logger.Write("In Trance: " + MagitekActionResourceManager.Arcanist.CanTrance);

            if (await CustomOpenerLogic.Opener()) return true;

            if (!SpellQueueLogic.SpellQueue.Any()) SpellQueueLogic.InSpellQueue = false;

            if (SpellQueueLogic.SpellQueue.Any()) if (await SpellQueueLogic.SpellQueueMethod()) return true;

            if (Core.Me.CurrentTarget.HasAura(Auras.MagicResistance))
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await SingleTarget.Ruin4MaxStacks()) return true;
            if (Utilities.Routines.Summoner.OnGcd || !ActionResourceManager.Summoner.DreadwyrmTrance)
            {
                if (await SingleTarget.Ruin4()) return true;
                if (await Aoe.Bane()) return true;
                if (await Buff.DreadwyrmTrance()) return true;
                if (await SingleTarget.EnkindleBahamut()) return true;
                if (await Pets.SummonBahamut()) return true;
                if (await SingleTarget.Deathflare()) return true;
                if (await SingleTarget.TriDisaster()) return true;
                if (await Pets.Summon()) return true;
                if (await Buff.LucidDreaming()) return true;
                if (await SingleTarget.Enkindle()) return true;
                if (await SingleTarget.EgiAssault2()) return true;
                if (await SingleTarget.EgiAssault()) return true;
                if (await Aoe.Painflare()) return true;
                if (await SingleTarget.Fester()) return true;
                if (await Aoe.EnergySiphon()) return true;
                if (await SingleTarget.EnergyDrain()) return true;
            }

            if (await Buff.DreadwyrmTrance()) return true;
            if (await SingleTarget.TriDisaster()) return true;
            if (await SingleTarget.Miasma()) return true;
            if (await SingleTarget.Bio()) return true;
            if (await Aoe.Outburst()) return true;
            return await SingleTarget.Ruin();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
