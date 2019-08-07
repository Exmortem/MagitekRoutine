using System;
using System.Linq;
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

        //tbh i could have saved the last song used but that would be bad if we want to switch song rotation mid fight/dungeon
        //this is robust and wont break somehow
        public static async Task<bool> LetMeSingYouTheSongOfMyPeople()
        {
            if (!BardSettings.Instance.UseSongs)
                return false;

            if (Casting.LastSpell == Spells.TheWanderersMinuet || Casting.LastSpell == Spells.MagesBallad || Casting.LastSpell == Spells.ArmysPaeon)
                return false;

            if (BardSettings.Instance.CheckDotsBeforeSinging)
                switch (BardSettings.Instance.AmmountOfDotsBeforeSinging)
                {
                    case 0:
                        break;
                    case 1:
                        if (Core.Me.ClassLevel < 64)
                        {
                            if (Combat.Enemies.Count(x => x.HasAura(Auras.Windbite, true) || x.HasAura(Auras.VenomousBite, true)) >= 1)
                                break;
                            return false;
                        }
                        else
                        {
                            if (Combat.Enemies.Count(x => x.HasAura(Auras.StormBite, true) || x.HasAura(Auras.CausticBite, true)) >= 1)
                                break;
                            return false;
                        }
                    case 2:
                        if (Core.Me.ClassLevel < 64)
                        {
                            if (Combat.Enemies.Count(x => x.HasAura(Auras.Windbite, true)) + Combat.Enemies.Count(x => x.HasAura(Auras.VenomousBite, true))  >= 2)
                                break;
                            return false;
                        }
                        else
                        {
                            if (Combat.Enemies.Count(x => x.HasAura(Auras.StormBite, true)) + Combat.Enemies.Count(x => x.HasAura(Auras.CausticBite, true)) >= 2)
                                break;
                            return false;
                        }
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

        public static async Task<bool> SongOrder_WanderersMinuet_MagesBallard_ArmysPaeon()
        {
            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            if (theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero || theWanderersMinuetCooldown == TimeSpan.Zero && magesBallardCooldown.TotalSeconds < 30)
                return await WanderersMinuet();

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds > 1000 * BardSettings.Instance.DefaultSongTransitionTime)
                return false;

            if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero 
                || theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown == TimeSpan.Zero && armysPaeonCooldown.TotalSeconds < 30)   
                return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None)
                return false;

            if (theWanderersMinuetCooldown != TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero && armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown.TotalSeconds < 30)
                return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> SongOrder_MagesBallard_WanderersMinuet_ArmysPaeon()
        {
            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds > 1000 * BardSettings.Instance.DefaultSongTransitionTime)
                return false;

            if (magesBallardCooldown == TimeSpan.Zero && (theWanderersMinuetCooldown == TimeSpan.Zero || armysPaeonCooldown != TimeSpan.Zero))
                return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
            if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                return await WanderersMinuet();
            if (armysPaeonCooldown == TimeSpan.Zero && theWanderersMinuetCooldown != TimeSpan.Zero)
                return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> SongOrder_MagesBallard_ArmysPaeon_WanderersMinuet()
        {
            TimeSpan theWanderersMinuetCooldown = Spells.TheWanderersMinuet.Cooldown;
            TimeSpan magesBallardCooldown = Spells.MagesBallad.Cooldown;
            TimeSpan armysPaeonCooldown = Spells.ArmysPaeon.Cooldown;

            if (ActionResourceManager.Bard.Timer.TotalMilliseconds > 1000 * BardSettings.Instance.DefaultSongTransitionTime)
                return false;

            if (magesBallardCooldown == TimeSpan.Zero && (armysPaeonCooldown == TimeSpan.Zero || theWanderersMinuetCooldown != TimeSpan.Zero))
                return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
            if (armysPaeonCooldown == TimeSpan.Zero && magesBallardCooldown != TimeSpan.Zero)
                return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
            if (theWanderersMinuetCooldown == TimeSpan.Zero && armysPaeonCooldown != TimeSpan.Zero)
                return await WanderersMinuet();

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