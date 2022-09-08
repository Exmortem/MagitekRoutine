using ff14bot;
using ff14bot.Managers;
using BardSong = ff14bot.Managers.ActionResourceManager.Bard.BardSong;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using BardRoutine = Magitek.Utilities.Routines.Bard;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class SingleTarget
    {

        public static async Task<bool> HeavyShot()
        {
            if (!BardSettings.Instance.UseHeavyShot)
                return false;

            return await BardRoutine.BurstShot.Cast(Core.Me.CurrentTarget);
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

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() >= BardSettings.Instance.ShadowBiteAfterBarrageEnemies)
                return false;

            if (Core.Me.ClassLevel < 70 || !Spells.RefulgentArrow.IsKnown())
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

            if (!Spells.PitchPerfect.IsReady())
                return false;

            if (!BardSong.WanderersMinuet.Equals(ActionResourceManager.Bard.ActiveSong))
                return false;

            if (BardRoutine.NextTickUnderCurrentSong() > 500)
                return false;

            return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PitchPerfect()
        {
            if (!BardSettings.Instance.UsePitchPerfect)
                return false;

            if (!Spells.PitchPerfect.IsReady())
                return false;

            if (ActionResourceManager.Bard.ActiveSong != BardSong.WanderersMinuet)
                return false;

            if (ActionResourceManager.Bard.Repertoire == 0)
                return false;

            if (BardRoutine.NextTickUnderCurrentSong() < 550)
                return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);

            if (ActionResourceManager.Bard.Repertoire < BardSettings.Instance.UsePitchPerfectAtRepertoire)
                return false;

            return await Spells.PitchPerfect.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BloodletterInMagesBallard()
        {
            if (!BardSettings.Instance.PrioritizeBloodletterDuringMagesBallard)
                return false;

            if (!Spells.Bloodletter.IsKnown())
                return false;

            if (Spells.Bloodletter.Charges < 1)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != BardSong.MagesBallad)
                return false;

            return await Spells.Bloodletter.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Bloodletter()
        {
            if (!BardSettings.Instance.UseBloodletter)
                return false;

            if (!Spells.Bloodletter.IsKnown())
                return false;

            if (Spells.Bloodletter.Charges < 1)
                return false;

            if (BardSong.ArmysPaeon.Equals(ActionResourceManager.Bard.ActiveSong) && Spells.Bloodletter.Charges < 2.8f)
                return false;

            if (BardSong.WanderersMinuet.Equals(ActionResourceManager.Bard.ActiveSong)) {
                if (Spells.Bloodletter.Charges >= 3)
                    return await Spells.Bloodletter.Cast(Core.Me.CurrentTarget);

                if (Spells.RagingStrikes.IsKnownAndReady() || Spells.BattleVoice.IsKnownAndReady() || Spells.RadiantFinale.IsKnownAndReady())
                    return false;
            }


            return await Spells.Bloodletter.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EmpyrealArrow()
        {
            if (!BardSettings.Instance.UseEmpyrealArrow)
                return false;

            if (!Spells.EmpyrealArrow.IsKnown())
                return false;

            switch (ActionResourceManager.Bard.ActiveSong)
            {
                case BardSong.None:
                    return false;

                case BardSong.MagesBallad:
                    if (Spells.Bloodletter.Cooldown == TimeSpan.Zero)
                        return false;
                    break;

                case BardSong.WanderersMinuet:
                    if (ActionResourceManager.Bard.Repertoire == 3)
                        return false;

                    if (ActionResourceManager.Bard.Repertoire == 0 
                        && BardRoutine.CurrentSongDuration() <= 1000 
                        && BardRoutine.NextTickUnderCurrentSong() <= 0)
                        return false;

                    if (Spells.RagingStrikes.IsKnownAndReady())
                        return false;
                    break;

                case BardSong.ArmysPaeon:
                    if (BardSettings.Instance.CurrentSongPlaylist == SongStrategyEnum.WM_MB_AP)
                    {
                        if (BardRoutine.CurrentSongDuration() <= BardSettings.Instance.DontUseEmpyrealArrowWhenSongEndsInXSeconds)
                            return false;
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

            if (!Spells.Sidewinder.IsKnown())
                return false;
            
            if (BardSong.WanderersMinuet.Equals(ActionResourceManager.Bard.ActiveSong) && !BardRoutine.IsUnderBuffWindow)
                return false;

            if (!BardSong.WanderersMinuet.Equals(ActionResourceManager.Bard.ActiveSong) && Spells.RagingStrikes.IsKnown() && (Spells.RagingStrikes.IsReady(5000) || !Spells.RagingStrikes.IsReady(61000)))
                return false;

            return await Spells.Sidewinder.Cast(Core.Me.CurrentTarget);
        }
    }
}