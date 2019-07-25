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

        //tbh i could have saved the last song used but that would be bat if we want to switch song rotation mid fight/dungeon
        //this is robust and wont break somehow
        public static async Task<bool> LetMeSingYouTheSongOfMyPeople()
        {
            if (!BardSettings.Instance.UseSongs)
                return false;

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

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

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds > 1000 * BardSettings.Instance.DefaultSongTransitionTime)
                        return false;
                    if (magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero)
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
                    if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
                    break;

                case SongStrategy.MB_WM_AP: // 3 - 6 Targets
                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds > 1000 * BardSettings.Instance.DefaultSongTransitionTime)
                        return false;

                    if (magesBallardCooldown == TimeSpan.Zero && (theWanderersMinuetCooldown == TimeSpan.Zero || armysPaeonCooldown != TimeSpan.Zero))
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
                    if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                        return await WanderersMinuet();
                    if (armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
                    break;

                case SongStrategy.MB_AP_WM: // 6+ Targets
                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds > 1000 * BardSettings.Instance.DefaultSongTransitionTime)
                        return false;

                    if (magesBallardCooldown == TimeSpan.Zero && (armysPaeonCooldown == TimeSpan.Zero || theWanderersMinuetCooldown != TimeSpan.Zero))
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
                    if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
                    if (theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown != TimeSpan.Zero)
                        return await WanderersMinuet();
                    break;
            }

            return false;
        }

        public static async Task<bool> WanderersMinuet()
        {
            //Cut AP Strat
            if (BardSettings.Instance.EndArmysPaeonEarly) 
            {
                if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.ArmysPaeon && ActionResourceManager.Bard.Timer.Seconds <= BardSettings.Instance.EndArmysPaeonEarlyWithXSecondsRemaining)
                    return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);
            }
            if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.None)
                return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}