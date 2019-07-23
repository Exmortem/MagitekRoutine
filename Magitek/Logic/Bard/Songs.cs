using System;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Logic.Bard
{
    internal static class Songs
    {

        public static async Task<bool> LetMeSingYouTheSongOfMyPeopleAdvancedLogic()
        {
            if (!BardSettings.Instance.UseSongs)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None)
                return false;

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true)) return false;

            if (Casting.LastSpell == Spells.TheWanderersMinuet || Casting.LastSpell == Spells.MagesBallad || Casting.LastSpell == Spells.ArmysPaeon)
                return false;

            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            switch (BardSettings.Instance.CurrentSongPlaylist)
            {
                case SongStrategy.WM_MB_AP: // < 3 Targets

                    if (theWanderersMinuetCooldown == TimeSpan.Zero && (magesBallardCooldown == TimeSpan.Zero || armysPaeonCooldown != TimeSpan.Zero))
                        return await WanderersMinuet();

                    if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None)
                        return false;
                    if (magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero)
                        return await MagesBallad();
                    if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                        return await ArmysPaeon();
                    break;

                case SongStrategy.MB_WM_AP: // 3 - 6 Targets
                    if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None)
                        return false;

                    if (magesBallardCooldown == TimeSpan.Zero && (theWanderersMinuetCooldown == TimeSpan.Zero || armysPaeonCooldown != TimeSpan.Zero))
                        return await MagesBallad();
                    if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                        return await WanderersMinuet();
                    if (armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero)
                        return await ArmysPaeon();
                    break;

                case SongStrategy.MB_AP_WM: // 6+ Targets
                    if (await ArmysPaeon()) return true;
                    if (await MagesBallad()) return true;
                    if (await WanderersMinuet()) return true;
                    break;
            }

            return false;
        }

        public static async Task<bool> LetMeSingYouTheSongOfMyPeople()
        {
            if (!BardSettings.Instance.UseSongs)
                return false;

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            if (Casting.LastSpell == Spells.TheWanderersMinuet || Casting.LastSpell == Spells.MagesBallad || Casting.LastSpell == Spells.ArmysPaeon)
                return false;

            switch (BardSettings.Instance.CurrentSongPlaylist)
            {
                case SongStrategy.WM_MB_AP: // < 3 Targets
                    if (await WanderersMinuet()) return true;
                    if (await MagesBallad()) return true;
                    if (await ArmysPaeon()) return true;
                    break;
                case SongStrategy.MB_WM_AP: // 3 - 6 Targets
                    if (await MagesBallad()) return true;
                    if (await WanderersMinuet()) return true;
                    if (await ArmysPaeon()) return true;
                    break;
                case SongStrategy.MB_AP_WM: // 6+ Targets
                    if (await ArmysPaeon()) return true;
                    if (await MagesBallad()) return true;
                    if (await WanderersMinuet()) return true;
                    break;
            }

            return false;
        }

        public static async Task<bool> WanderersMinuet()
        {
            //Cut AP Strat
            if (BardSettings.Instance.EndArmysPaeonEarly /* && BardSettings.Instance.SongOrderStrategy != SongStrategy.MB_AP_WM */) //FlexibleCode vs UserResponsibility
            {
                if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.ArmysPaeon && ActionResourceManager.Bard.Timer.Seconds <= BardSettings.Instance.EndArmysPaeonEarlyWithXSecondsRemaining)
                    return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);

                return false;
            }

            return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MagesBallad()
        {
            if (Spells.MagesBallad.Cooldown != TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.None)
                return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> ArmysPaeon()
        {
            if (Spells.ArmysPaeon.Cooldown != TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.None)
                return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
            
            return false;
        }
    }
}