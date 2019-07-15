using System;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Logic.Bard
{
    internal static class Songs
    {
        public static async Task<bool> Sing()
        {
            if (!BardSettings.Instance.PlaySongs)
                return false;
            
            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            // Cut Army's Paeon short if we're able to go into Wanderer's Minuet
            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.None)
            {
                if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.ArmysPaeon && ActionResourceManager.Bard.Timer.Seconds <= 10 && Spells.TheWanderersMinuet.Cooldown == TimeSpan.Zero)
                    return await WanderersMinuet();
                else
                    return false;
            }

            if (await WanderersMinuet()) return true;
            if (await MagesBallad()) return true;
            return await ArmysPaeon();
        }

        private static async Task<bool> MagesBallad()
        {
            if (Casting.LastSpell == Spells.ArmysPaeon || Casting.LastSpell == Spells.TheWanderersMinuet)
                return false;

            return await Spells.MagesBallad.Cast(Core.Me.CurrentTarget);
        }

        private static async Task<bool> WanderersMinuet()
        {
            if (Casting.LastSpell == Spells.ArmysPaeon || Casting.LastSpell == Spells.MagesBallad)
                return false;

            return await Spells.TheWanderersMinuet.Cast(Core.Me.CurrentTarget);
        }

        private static async Task<bool> ArmysPaeon()
        {
            if (Casting.LastSpell == Spells.TheWanderersMinuet || Casting.LastSpell == Spells.MagesBallad)
                return false;

            return await Spells.ArmysPaeon.Cast(Core.Me.CurrentTarget);
        }
    }
}