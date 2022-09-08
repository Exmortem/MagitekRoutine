using ff14bot;
using ff14bot.Managers;
using BardSong = ff14bot.Managers.ActionResourceManager.Bard.BardSong;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Bard;
using Magitek.Utilities;
using BardRoutine = Magitek.Utilities.Routines.Bard;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class Songs
    {
        public static async Task<bool> LetMeSingYouTheSongOfMyPeople()
        {
            if (!BardSettings.Instance.UseSongs)
                return false;

            if (Casting.LastSpell == Spells.TheWanderersMinuet || Casting.LastSpell == Spells.MagesBallad || Casting.LastSpell == Spells.ArmysPaeon)
                return false;

            List<SpellData> songSpellsOrder = GetOrderedSongSpellList(ActionResourceManager.Bard.ActiveSong);
            if (songSpellsOrder?.Any() != true)
                return false;

            if (BardSong.None.Equals(ActionResourceManager.Bard.ActiveSong))
            {
                if (songSpellsOrder[0].IsKnownAndReady() 
                    && (!songSpellsOrder[1].IsKnown() || songSpellsOrder[1].IsReady(BardRoutine.SongMaxDuration)))
                    return await songSpellsOrder[0].Cast(Core.Me.CurrentTarget);

                if (songSpellsOrder[1].IsKnownAndReady() 
                    && (!songSpellsOrder[2].IsKnown() || songSpellsOrder[2].IsReady(BardRoutine.SongMaxDuration))
                    && (!songSpellsOrder[0].IsKnown() || !songSpellsOrder[0].IsReady(BardRoutine.SongMaxDuration)))
                    return await songSpellsOrder[1].Cast(Core.Me.CurrentTarget);

                if (songSpellsOrder[2].IsKnownAndReady() 
                    && (!songSpellsOrder[0].IsKnown() || songSpellsOrder[0].IsReady(BardRoutine.SongMaxDuration)))
                    return await songSpellsOrder[2].Cast(Core.Me.CurrentTarget);
            } else
            {
                /*if (BardSong.ArmysPaeon.Equals(ActionResourceManager.Bard.ActiveSong) && Spells.HeavyShot.Cooldown.TotalMilliseconds > Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds - 500)
                    return false;*/

                List<BardSong> songListOrder = SongStrategy.GetSongOrderFromSongStrategy();
                if (songListOrder.IndexOf(ActionResourceManager.Bard.ActiveSong) == 1)
                {
                    if (songSpellsOrder[0].IsKnownAndReady()
                        && songSpellsOrder[1].IsKnown() && !songSpellsOrder[1].IsReady() && songSpellsOrder[1].IsReady(BardRoutine.SongMaxDuration)
                        && songSpellsOrder[2].IsKnown() && !songSpellsOrder[2].IsReady())
                        return await EndCurrentSong(ActionResourceManager.Bard.ActiveSong, songSpellsOrder[0], songListOrder);

                    if (songSpellsOrder[0].IsKnownAndReady()
                        && songSpellsOrder[1].IsKnownAndReady()
                        && songSpellsOrder[2].IsKnown() && !songSpellsOrder[2].IsReady() && !songSpellsOrder[2].IsReady(BardRoutine.SongMaxDuration))
                        return await EndCurrentSong(ActionResourceManager.Bard.ActiveSong, songSpellsOrder[0], songListOrder);
                } else
                {
                    if (songSpellsOrder[0].IsKnownAndReady() 
                        && (!songSpellsOrder[1].IsKnown() || songSpellsOrder[1].IsKnownAndReady(BardRoutine.SongMaxDuration)))
                        return await EndCurrentSong(ActionResourceManager.Bard.ActiveSong, songSpellsOrder[0], songListOrder);
                }   
            }
            return false;
        }

        private static async Task<bool> EndCurrentSong(BardSong currentSong, SpellData nextSong, List<BardSong> songListOrder)
        {
            var armysPaeonCheck = BardSong.ArmysPaeon.Equals(currentSong) && BardSettings.Instance.EndArmysPaeonEarly 
                && (ActionResourceManager.Bard.Timer.TotalMilliseconds <= BardSettings.Instance.EndArmysPaeonEarlyWithXMilliSecondsRemaining 
                || (nextSong.IsReady() && songListOrder.IndexOf(ActionResourceManager.Bard.ActiveSong) == 2));
            var magesBalladCheck = BardSong.MagesBallad.Equals(currentSong) && BardSettings.Instance.EndMagesBalladEarly 
                && (ActionResourceManager.Bard.Timer.TotalMilliseconds <= BardSettings.Instance.EndMagesBalladEarlyWithXMilliSecondsRemaining
                || (nextSong.IsReady() && songListOrder.IndexOf(ActionResourceManager.Bard.ActiveSong) == 2));
            var wanderersMinuetCheck = BardSong.WanderersMinuet.Equals(currentSong) && BardSettings.Instance.EndWanderersMinuetEarly 
                && (ActionResourceManager.Bard.Timer.TotalMilliseconds <= BardSettings.Instance.EndWanderersMinuetEarlyWithXMilliSecondsRemaining
                || (nextSong.IsReady() && songListOrder.IndexOf(ActionResourceManager.Bard.ActiveSong) == 2));

            if (armysPaeonCheck || magesBalladCheck || wanderersMinuetCheck)
            {
                //Force delay WM
                if (Spells.TheWanderersMinuet.Equals(nextSong) && Spells.HeavyShot.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset + 100)
                    return false;

                Logger.WriteInfo($@"[Cast {nextSong}] Time remaining for {ActionResourceManager.Bard.ActiveSong} : {ActionResourceManager.Bard.Timer.TotalMilliseconds}");
                return await nextSong.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        private static List<SpellData> GetOrderedSongSpellList(BardSong currentSong)
        {
            if (SongStrategyEnum.MB_WM_AP.Equals(BardSettings.Instance.CurrentSongPlaylist))
            {
                if (BardSong.MagesBallad.Equals(currentSong))
                    return new List<SpellData>() { Spells.TheWanderersMinuet, Spells.ArmysPaeon, Spells.MagesBallad };
                if (BardSong.WanderersMinuet.Equals(currentSong))
                    return new List<SpellData>() { Spells.ArmysPaeon, Spells.MagesBallad, Spells.TheWanderersMinuet };

                return new List<SpellData>() { Spells.MagesBallad, Spells.TheWanderersMinuet, Spells.ArmysPaeon };
            }
            else if (SongStrategyEnum.MB_AP_WM.Equals(BardSettings.Instance.CurrentSongPlaylist))
            {
                if (BardSong.MagesBallad.Equals(currentSong))
                    return new List<SpellData>() { Spells.ArmysPaeon, Spells.TheWanderersMinuet, Spells.MagesBallad };
                if (BardSong.ArmysPaeon.Equals(currentSong))
                    return new List<SpellData>() { Spells.TheWanderersMinuet, Spells.MagesBallad, Spells.ArmysPaeon };

                return new List<SpellData>() { Spells.MagesBallad, Spells.ArmysPaeon, Spells.TheWanderersMinuet };
            }
            else
            {
                if (BardSong.WanderersMinuet.Equals(currentSong))
                    return new List<SpellData>() { Spells.MagesBallad, Spells.ArmysPaeon, Spells.TheWanderersMinuet };
                if (BardSong.MagesBallad.Equals(currentSong))
                    return new List<SpellData>() { Spells.ArmysPaeon, Spells.TheWanderersMinuet, Spells.MagesBallad };

                return new List<SpellData>() { Spells.TheWanderersMinuet, Spells.MagesBallad, Spells.ArmysPaeon };
            }
        }
    }
}