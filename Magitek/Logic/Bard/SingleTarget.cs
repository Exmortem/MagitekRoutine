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
using Magitek.Enumerations;
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

        public static async Task<bool> EmpyrealArrow()
        {
            if (!BardSettings.Instance.UseEmpyrealArrow)
                return false;

            if (!ActionManager.HasSpell(Spells.EmpyrealArrow.Id))
                return false;

            switch (ActionResourceManager.Bard.ActiveSong)
            {
                case ActionResourceManager.Bard.BardSong.None:
                    return false;

                case ActionResourceManager.Bard.BardSong.MagesBallad:
                    if (Spells.Bloodletter.Cooldown == TimeSpan.Zero)
                        return false;
                    break;

                //Cant use PP outside of WM so why would we want to waste a EA here
                //Maybe go even further and add a repertoire condition so we wont use EA when < x repertoire in the last y seconds
                case ActionResourceManager.Bard.BardSong.WanderersMinuet:
                    if (ActionResourceManager.Bard.Repertoire == 3 || ActionResourceManager.Bard.Timer.TotalMilliseconds <= 1000)
                        return false;
                    break;

                case ActionResourceManager.Bard.BardSong.ArmysPaeon:
                    if (BardSettings.Instance.CurrentSongPlaylist == SongStrategy.WM_MB_AP)
                    {
                        if (BardSettings.Instance.EndArmysPaeonEarly)
                        {
                            if ((ActionResourceManager.Bard.Timer.TotalSeconds - BardSettings.Instance.EndArmysPaeonEarlyWithXSecondsRemaining) < BardSettings.Instance.DontUseEmpyrealArrowWhenSongEndsInXSeconds)
                                return false;
                        }
                    }
                    break;

                default:
                    break;
            }

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
    }
}