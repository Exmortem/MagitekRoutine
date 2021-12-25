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
    //Some utility functions for use with the combat routine state machine
    public static class SmUtil
    {
        //A no-op function to use when you want to transition states without taking any action
        public static async Task<bool> NoOp()
        {
            return true;
        }

        #region Forced Level Sync
        //Fake a level synchronization for development purposes. This will let you test the rotation at level lower than the character you're developing on.
        //Call SetSynchedLevel() to set the level, and use SyncedCast() to prevent the state machine from casting any spells above that level
        //If you also need to ignore auras that appear above a certain level, see the "Aura level sync" region in Utilities\Routines\Redmage.cs for an example of how to handle it
        public static int SyncedLevel => Math.Min(mForcedSyncLevel, (int)Core.Me.ClassLevel);

        private static int mForcedSyncLevel = 90;
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

        //Swiftcast the specified spell. Magitek nevers swiftcast as having been correctly cast, so this workaround is necessary.
        //This method first casts swiftcast, then the specified spell. It is compatible with SyncedCast.
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
