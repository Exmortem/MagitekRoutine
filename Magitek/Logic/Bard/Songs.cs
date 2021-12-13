using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class Songs
    {

        //tbh i could have saved the last song used but that would be bad if we want to switch song rotation mid fight/dungeon
        //this is robust and wont break somehow
        public static async Task<bool> LetMeSingYouTheSongOfMyPeople()
        {
            if (!BardSettings.Instance.UseSongs)
                return false;

            if (Casting.LastSpell == Spells.TheWanderersMinuet || Casting.LastSpell == Spells.MagesBallad || Casting.LastSpell == Spells.ArmysPaeon)
                return false;

            if (!ActionManager.HasSpell(Spells.TheWanderersMinuet.Id) || !ActionManager.HasSpell(Spells.MagesBallad.Id)
                                                                      || !ActionManager.HasSpell(Spells.ArmysPaeon.Id))
                return await PlsEndMe();

            if (BardSettings.Instance.CheckDotsBeforeSinging)
                switch (BardSettings.Instance.AmmountOfDotsBeforeSinging)
                {
                    case 0:
                        break;
                    case 1:
                        if (Combat.Enemies.Count(x => x.HasAura(Utilities.Routines.Bard.Windbite, true) || x.HasAura(Utilities.Routines.Bard.VenomousBite, true)) >= 1)
                            break;
                        return false;
                    case 2:
                        if (Combat.Enemies.Count(x => x.HasAura(Utilities.Routines.Bard.Windbite, true)) + Combat.Enemies.Count(x => x.HasAura(Utilities.Routines.Bard.VenomousBite, true)) >= 2)
                            break;
                        return false;
                }

            switch (BardSettings.Instance.CurrentSongPlaylist)
            {
                case SongStrategy.WM_MB_AP: // < 3 Targets
                    return await SongOrder_WanderersMinuet_MagesBallard_ArmysPaeon();
                case SongStrategy.MB_WM_AP: // 3 - 6 Targets
                    return await SongOrder_MagesBallard_WanderersMinuet_ArmysPaeon();
                case SongStrategy.MB_AP_WM: // 6+ Targets
                    return await SongOrder_MagesBallard_ArmysPaeon_WanderersMinuet();
                default:
                    return false;
            }
        }

        public static async Task<bool> PlsEndMe()
        {
            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None)
                return false;

            return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget) || await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
        }

        #region WanderersMinuet->MagesBallad->ArmysPaeon

        public static async Task<bool> SongOrder_WanderersMinuet_MagesBallard_ArmysPaeon()
        {
            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            switch (ActionResourceManager.Bard.ActiveSong)
            {
                case ActionResourceManager.Bard.BardSong.None:

                    if (theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero || theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown.TotalMilliseconds <= 45000)
                        return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);

                    if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero
                        || theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalMilliseconds < 45000)
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);

                    if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalMilliseconds < 45000
                        || theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown.TotalMilliseconds > 45000)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
                    break;

                case ActionResourceManager.Bard.BardSong.WanderersMinuet:

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() > 400)
                        return false;

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Spells.HeavyShot.Cooldown.TotalMilliseconds > 600)
                        return false;

                    if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero
                        || theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalMilliseconds < 45000)
                        return await EarlyMagesBallad();

                    break;

                case ActionResourceManager.Bard.BardSong.MagesBallad:

                    if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalMilliseconds < 45000
                        || theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown.TotalMilliseconds > 45000)
                        return await EarlyArmysPaeon();

                    break;

                case ActionResourceManager.Bard.BardSong.ArmysPaeon:

                    if (Spells.HeavyShot.Cooldown.TotalMilliseconds > Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds - 500)
                        return false;

                    if (theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero || theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown.TotalMilliseconds <= 45000)
                        return await EarlyWanderersMinuet();

                    break;
            }

            return false;
        }

        #endregion

        #region MagesBallad->WanderersMinuet->ArmysPaeon

        public static async Task<bool> SongOrder_MagesBallard_WanderersMinuet_ArmysPaeon()
        {
            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            switch (ActionResourceManager.Bard.ActiveSong)
            {
                case ActionResourceManager.Bard.BardSong.None:

                    if (magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero || magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalSeconds < 45)
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);

                    if (magesBallardCooldown != TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero
                        || magesBallardCooldown != TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalSeconds < 45)
                        return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);

                    if (magesBallardCooldown != TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown.TotalSeconds <= 45
                        || magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalSeconds > 45)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
                    break;

                case ActionResourceManager.Bard.BardSong.WanderersMinuet:

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() > 400)
                        return false;

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Spells.HeavyShot.Cooldown.TotalMilliseconds > 600)
                        return false;

                    if (magesBallardCooldown != TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown.TotalSeconds <= 45
                        || magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalSeconds > 45)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);

                    break;

                case ActionResourceManager.Bard.BardSong.MagesBallad:

                    if (Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() < ActionResourceManager.Bard.Timer.TotalMilliseconds)
                        return false;

                    if (magesBallardCooldown != TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero
                        || magesBallardCooldown != TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalSeconds < 45)
                        return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);

                    break;

                case ActionResourceManager.Bard.BardSong.ArmysPaeon:

                    if (Spells.HeavyShot.Cooldown.TotalMilliseconds > Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds - 500)
                        return false;

                    if (magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero || magesBallardCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalSeconds < 45)
                        return await EarlyMagesBallad();
                    break;
            }

            return false;
        }

        #endregion

        #region MagesBallad->ArmysPaeon->WanderersMinuet

        public static async Task<bool> SongOrder_MagesBallard_ArmysPaeon_WanderersMinuet()
        {
            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            switch (ActionResourceManager.Bard.ActiveSong)
            {
                case ActionResourceManager.Bard.BardSong.None:

                    if (magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero || magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalMilliseconds < 45000)
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);

                    if (magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero
                        || magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalMilliseconds < 45000)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);

                    if (magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown != TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown.TotalMilliseconds <= 45000
                        || magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown != TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalMilliseconds > 45000)
                        return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);
                    break;

                case ActionResourceManager.Bard.BardSong.WanderersMinuet:

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() > 400)
                        return false;

                    if (ActionResourceManager.Bard.Timer.TotalMilliseconds - Spells.HeavyShot.Cooldown.TotalMilliseconds > 600)
                        return false;

                    if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero
                        || theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalMilliseconds < 45000)
                        return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
                    break;

                case ActionResourceManager.Bard.BardSong.MagesBallad:

                    if (Utilities.Routines.Bard.TimeUntilNextPossibleDoTTick() < ActionResourceManager.Bard.Timer.TotalMilliseconds)
                        return false;

                    if (magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown == TimeSpan.Zero
                        || magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalMilliseconds < 45000)
                        return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
                    break;

                case ActionResourceManager.Bard.BardSong.ArmysPaeon:

                    if (Spells.HeavyShot.Cooldown.TotalMilliseconds > Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds - 500)
                        return false;

                    if (theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero || theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown.TotalMilliseconds < 45000)
                        return await EarlyWanderersMinuet();
                    break;
            }

            return false;
        }

        #endregion

        public static async Task<bool> EarlyWanderersMinuet()
        {
            //Cut AP Strat
            if (!BardSettings.Instance.EndArmysPaeonEarly)
                return false;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds < 1000 * BardSettings.Instance.EndArmysPaeonEarlyWithXSecondsRemaining)
                return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);


            return false;
        }

        public static async Task<bool> EarlyMagesBallad()
        {
            //Cut WM Strat
            if (!BardSettings.Instance.EndWanderersMinuetEarly)
                return false;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds < 1000 * BardSettings.Instance.EndWanderersMinuetEarlyWithXSecondsRemaining)
                return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);


            return false;
        }

        public static async Task<bool> EarlyArmysPaeon()
        {
            //Cut MB Strat
            if (!BardSettings.Instance.EndMagesBalladEarly)
                return false;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds < 1000 * BardSettings.Instance.EndMagesBalladEarlyWithXSecondsRemaining)
                return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);

            return false;
        }
    }
}