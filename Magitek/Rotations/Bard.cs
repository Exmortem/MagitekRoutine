using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Bard;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Bard
    {
        public static async Task<bool> Rest()
        {
            return Core.Me.CurrentHealthPercent < BardSettings.Instance.RestHealthPercent;
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            //Openers.OpenerCheck();

            if (Core.Me.HasTarget && Core.Me.CurrentTarget.CanAttack)
                return false;


            if (Globals.OnPvpMap)
                return false;

            return await PhysicalDps.Peloton(BardSettings.Instance);
        }

        public static async Task<bool> Pull()
        {
            Utilities.Routines.Bard.RefreshVars();

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            return await GambitLogic.Gambit();
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

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

            if (Weaving.GetCurrentWeavingCounter() < 2 && Spells.HeavyShot.Cooldown.TotalMilliseconds >
                650 + BaseSettings.Instance.UserLatencyOffset)
            {
                // Utility
                if (await Utility.RepellingShot()) return true;
                if (await Utility.WardensPaean()) return true;
                if (await Utility.NaturesMinne()) return true;
                if (await Utility.Troubadour()) return true;
                if (await PhysicalDps.ArmsLength(BardSettings.Instance)) return true;
                if (await PhysicalDps.SecondWind(BardSettings.Instance)) return true;
                if (await PhysicalDps.Interrupt(BardSettings.Instance)) return true;

                // Damage
                if (await SingleTarget.LastPossiblePitchPerfectDuringWM()) return true;
                if (await Songs.LetMeSingYouTheSongOfMyPeople()) return true;
                if (await Cooldowns.RagingStrikes()) return true;
                if (await Cooldowns.RadiantFinale()) return true;
                if (await Cooldowns.BattleVoice()) return true;
                if (await Cooldowns.Barrage()) return true;
                if (await SingleTarget.PitchPerfect()) return true;
                if (await Aoe.RainOfDeathDuringMagesBallard()) return true;
                if (await SingleTarget.BloodletterInMagesBallard()) return true;
                if (await SingleTarget.EmpyrealArrow()) return true;
                if (await Aoe.ShadowBite()) return true;
                if (await SingleTarget.Sidewinder()) return true;
                if (await Aoe.RainOfDeath()) return true;
                if (await SingleTarget.Bloodletter()) return true;
            }

            if (await SingleTarget.StraightShotAfterBarrage()) return true;
            if (await DamageOverTime.IronJawsOnCurrentTarget()) return true;
            if (await DamageOverTime.SnapShotIronJawsOnCurrentTarget()) return true;
            if (await DamageOverTime.WindbiteOnCurrentTarget()) return true;
            if (await DamageOverTime.VenomousBiteOnCurrentTarget()) return true;
            if (await Aoe.BlastArrow()) return true;
            if (await Aoe.ApexArrow()) return true;
            if (await DamageOverTime.IronJawsOnOffTarget()) return true;
            if (await DamageOverTime.WindbiteOnOffTarget()) return true;
            if (await DamageOverTime.VenomousBiteOnOffTarget()) return true;
            if (await Aoe.LadonsBite()) return true;
            if (await SingleTarget.StraightShot()) return true;
            return (await SingleTarget.HeavyShot());

        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}


