using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Bard;
using Magitek.Logic.Roles;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Rotations.Bard
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            Group.UpdateAllies();

            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2);
            Utilities.Routines.Bard.RefreshVars();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            //This will still result in tripple weaves i guess
            if (Spells.HeavyShot.Cooldown.TotalMilliseconds > 850 /* + BardSettings.Instance.UserPingOffset */)
            {
                if (await Songs.LetMeSingYouTheSongOfMyPeopleAdvancedLogic()) return true;
                if (await Cooldowns.BattleVoice()) return true;
                if (await Cooldowns.RagingStrikes()) return true;
                if (await Cooldowns.RefulgentBarrage()) return true;
                if (await SingleTarget.PitchPerfect()) return true;
                if (await SingleTarget.BloodletterInMagesBallard()) return true;
                if (await SingleTarget.EmpyrealArrow()) return true;
                if (await SingleTarget.Sidewinder()) return true;
                if (await SingleTarget.Bloodletter()) return true;
            }

            if (await Dot.HandleDots()) return true;
            if (await Dot.HandleMultiDots()) return true;
            if (await Aoe.ApexArrow()) return true;
            if (await SingleTarget.StraightShot()) return true;
            return (await SingleTarget.HeavyShot());

            //if (await PhysicalDps.SecondWind(BardSettings.Instance)) return true;
            //if (await Buff.NaturesMinne()) return true;
            //if (await Dispel.Execute()) return true;

            //if (await Dot.IronJaws()) return true;
            //if (await Buff.BattleVoice()) return true;
            //if (await Buff.RagingStrikes()) return true;
            //if (await SingleTarget.RefulgentBarrage()) return true;
            //if (await SingleTarget.StraightShot()) return true;
            //if (await SingleTarget.EmpyrealArrow()) return true;
            //if (await SingleTarget.PitchPerfect()) return true;

            //if (Utilities.Routines.Bard.OnGcd)
            //{
            //    if (await Songs.Sing()) return true;
            //    if (await SingleTarget.SidewinderAndShadowbite()) return true;
            //    if (await Aoe.RainOfDeath()) return true;
            //    if (await SingleTarget.Bloodletter()) return true;
            //    if (await SingleTarget.RepellingShot()) return true;
            //}

            //if (await Aoe.ApexArrow()) return true;
            //if (await Aoe.QuickNock()) return true;
            //if (await Dot.Windbite()) return true;
            //if (await Dot.VenomousBite()) return true;
            //if (await Dot.DotMultipleTargets()) return true;
            //return await SingleTarget.HeavyShot();
        }
    }
}