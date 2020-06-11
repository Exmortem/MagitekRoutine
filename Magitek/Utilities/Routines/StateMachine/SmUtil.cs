using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
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

        public static async Task<bool> Swiftcast(SpellData spell, GameObject target)
        {
            if (spell.LevelAcquired > SmUtil.SyncedLevel)
                return false;
            if (await SmUtil.SyncedCast(Spells.Swiftcast, Core.Me))
            {
                await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                await Coroutine.Wait(2000, () => ActionManager.CanCast(spell, target));
                return await spell.Cast(target);
            }
            return false;
        }
    }
}
