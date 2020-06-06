using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;

namespace Magitek.Utilities.Routines
{
    public static class SmUtil
    {
        public static async Task<bool> NoOp()
        {
            return true;
        }

        #region Forced Level Sync
        public static int SyncedLevel => Math.Min(mForcedSyncLevel, (int)Core.Me.ClassLevel);

        private static int mForcedSyncLevel = 80;
        public static void SetSyncedLevel(int level)
        {
            mForcedSyncLevel = level;
        }

        public static async Task<bool> SyncedCast(SpellData spell, GameObject target)
        {
            if (spell.LevelAcquired > SyncedLevel)
                return false;

            return await spell.Cast(target);
        }
        #endregion
    }
}
