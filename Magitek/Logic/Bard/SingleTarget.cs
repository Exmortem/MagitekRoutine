using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

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

            if (Core.Me.ClassLevel < 70 || !ActionManager.HasSpell(Spells.RefulgentArrow.Id))
                return await Spells.StraightShot.Cast(Core.Me.CurrentTarget);

            return await Spells.RefulgentArrow.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> StraightShotAfterBarrage()
        {
            if (!Core.Me.HasAura(Auras.Barrage))
                return false;

            if (!BardSettings.Instance.UseStraightShot)
                return false;


            if (Core.Me.ClassLevel < 70 || !ActionManager.HasSpell(Spells.RefulgentArrow.Id))
                if (await Spells.StraightShot.Cast(Core.Me.CurrentTarget))
                    return true;

            if (await Spells.RefulgentArrow.Cast(Core.Me.CurrentTarget))
                return true;

            return true; //We want to check for more oGCDs while waiting for our GCD
        }

        public static async Task<bool> LastPossiblePitchPerfectDuringWM()
        {
            if (!BardSettings.Instance.UsePitchPerfect)
                return false;

            if (Spells.PitchPerfect.Cooldown != TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.WanderersMinuet)
                return false;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() > 500)
                return false;

            return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> PitchPerfect()
        {
            if (!BardSettings.Instance.UsePitchPerfect)
                return false;

            if (Spells.PitchPerfect.Cooldown != TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.WanderersMinuet)
                return false;

            if (ActionResourceManager.Bard.Repertoire == 0)
                return false;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() < 550)
                return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);

            if (ActionResourceManager.Bard.Repertoire < BardSettings.Instance.UsePitchPerfectAtRepertoire)
                return false;

            return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BloodletterInMagesBallard()
        {
            if (!BardSettings.Instance.PrioritizeBloodletterDuringMagesBallard)
                return false;

            if (!ActionManager.HasSpell(Spells.Bloodletter.Id))
                return false;

            if (Spells.Bloodletter.Charges < 1)
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

            if (Spells.Bloodletter.Charges < 1)
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

                case ActionResourceManager.Bard.BardSong.WanderersMinuet:

                    if (ActionResourceManager.Bard.Repertoire == 3)
                        return false;

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds <= 2000 && ActionResourceManager.Bard.Repertoire == 0 && Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() > ActionResourceManager.Bard.Timer.TotalMilliseconds)
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