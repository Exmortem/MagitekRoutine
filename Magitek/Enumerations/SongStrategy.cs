using BardSong = ff14bot.Managers.ActionResourceManager.Bard.BardSong;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System.Collections.Generic;

namespace Magitek.Enumerations
{
    public enum SongStrategyEnum
    {
        WM_MB_AP,
        MB_WM_AP,
        MB_AP_WM
    }
    
    public static class SongStrategy
    {
        public static List<BardSong> GetSongOrderFromSongStrategy()
        {
            if (SongStrategyEnum.MB_WM_AP.Equals(BardSettings.Instance.CurrentSongPlaylist))
                return new List<BardSong>() { BardSong.MagesBallad, BardSong.WanderersMinuet, BardSong.ArmysPaeon };

            if (SongStrategyEnum.MB_AP_WM.Equals(BardSettings.Instance.CurrentSongPlaylist))
                return new List<BardSong>() { BardSong.MagesBallad, BardSong.ArmysPaeon, BardSong.WanderersMinuet };

            return new List<BardSong>() { BardSong.WanderersMinuet, BardSong.MagesBallad, BardSong.ArmysPaeon };
        }
    }
}
