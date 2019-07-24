using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using ff14bot;
using QuickGraph;

namespace Magitek.Logic.Bard
{
    internal static class SingleTarget
    {

        public static async Task<bool> HeavyShot()
        {
            if (!BardSettings.Instance.UseHeavyShot)
                return false;

            if (Core.Me.ClassLevel < 76)
                return await Spells.HeavyShot.Cast(Core.Me.CurrentTarget);

            return await Spells.BurstShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> StraightShot()
        {
            if (!BardSettings.Instance.UseStraightShot)
                return false;

            if (!Core.Me.HasAura(Auras.StraighterShot))
                return false;

            if (Core.Me.ClassLevel < 70)
                return await Spells.StraightShot.Cast(Core.Me.CurrentTarget);

            return await Spells.RefulgentArrow.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PitchPerfect()
        {
            if (!BardSettings.Instance.UsePitchPerfect)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.WanderersMinuet)
                return false;
            var repertoire = ActionResourceManager.Bard.Repertoire;

            if (repertoire == 0)
                return false;

            if (repertoire == 3)
                return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);

            if (repertoire >= BardSettings.Instance.UsePitchPerfectAtRepertoire)
                return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);

            //Logic to catch the last possible PP
            if (BardSettings.Instance.UsePitchPerfectAtTheEndOfWanderersMinuet && ActionResourceManager.Bard.Timer.TotalMilliseconds < (BardSettings.Instance.UsePitchPerfectWithinTheLastXSecondsOfWanderersMinuet * 1000))
                return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> BloodletterInMagesBallard()
        {
            if (!BardSettings.Instance.PrioritizeBloodletterDuringMagesBallard)
                return false;

            if (!ActionManager.HasSpell(Spells.Bloodletter.Id))
                return false;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.MagesBallad)
                return false;

            return await Spells.Bloodletter.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Bloodletter()
        {
            if (!BardSettings.Instance.UseBloodletter)
                return false;

            if (!ActionManager.HasSpell(Spells.Bloodletter.Id))
                return false;

            return await Spells.Bloodletter.Cast(Core.Me.CurrentTarget);
        }

        //Add Advanced Logic for end of AP need to be added
        public static async Task<bool> EmpyrealArrow()
        {
            if (!BardSettings.Instance.UseEmpyrealArrow)
                return false;

            if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.None)
                return false;

            return await Spells.EmpyrealArrow.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Sidewinder()
        {
            if (!BardSettings.Instance.UseSidewinder)
                return false;

            if (!ActionManager.HasSpell(Spells.Sidewinder.Id))
                return false;

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            return await Spells.Sidewinder.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RepellingShot()
        {
            if (!BardSettings.Instance.RepellingShot)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (BardSettings.Instance.RepellingShotOnlyWhenTargeted)
            {
                var repellingShotTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) <= 5 + r.CombatReach && 
                                                                             r.InView() && 
                                                                             ActionManager.CanCast(Spells.RepellingShot.Id, r));

                if (repellingShotTarget == null)
                    return false;

                return await Spells.RepellingShot.Cast(repellingShotTarget);
            }
            else
            {
                if (Core.Me.CurrentTarget == null)
                    return false;

                if (Core.Me.CurrentTarget.Distance(Core.Me) > 5 + Core.Me.CurrentTarget.CombatReach)
                    return false;

                return await Spells.RepellingShot.Cast(Core.Me.CurrentTarget);
            }
        }

        public static async Task<bool> SidewinderAndShadowbite()
        {
            if (!ActionManager.HasSpell(Spells.Sidewinder.Id))
                return false;

            // Make sure both DOTs are on target
            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            // Use only on Trick Attack if the setting is enable and we're in a party
            if (BardSettings.Instance.UseSideWinderOnlyOnTrick && PartyManager.IsInParty)
            {
                if (!Core.Me.CurrentTarget.HasAura(638) || Spells.Sidewinder.Cooldown.Milliseconds != 0)
                    return false;

                Logger.WriteInfo("Using Sidewinder with Trick Attack");
                return await Spells.Sidewinder.Cast(Core.Me.CurrentTarget);
            }

            return await HandleCast();

            async Task<bool> HandleCast()
            {
                // At level 63, Shadowbite becomes available
                if (Core.Me.ClassLevel < 63)
                    return await Spells.Sidewinder.Cast(Core.Me.CurrentTarget);

                // If there's multiple targets around our target, use Shadowbite
                if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() > 1)
                    return await Spells.Shadowbite.Cast(Core.Me.CurrentTarget);

                return await Spells.Sidewinder.Cast(Core.Me.CurrentTarget);
            }
        }
    }
}