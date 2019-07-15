using System;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.Directors;
using ff14bot.Helpers;

namespace Magitek.Utilities
{
    public static class Duty
    {
        /// <summary>
        /// Provides Duty State information. InProgress, NotInDuty, NotStarted, and Ended.
        /// </summary>
        /// <returns></returns>
        public static States State()
        {
            if (Core.Me.InCombat) return States.InProgress; //If we're in Combat, don't even continue, it doesn't matter if we're in a duty or not, just unblock our actions and tell us we're in Progress

            if (DirectorManager.ActiveDirector == null) return States.NotInDuty;

            if (DirectorManager.ActiveDirector.DirectorType != DirectorType.InstanceContent) return States.NotInDuty;

            var instanceDirector = (InstanceContentDirector) DirectorManager.ActiveDirector;

            if (instanceDirector.InstanceEnded) return States.Ended;

            return instanceDirector.InstanceStarted ? States.InProgress : States.NotStarted;
        }

        public enum States
        {
            NotInDuty,
            NotStarted,
            InProgress,
            Ended
        }
    }
}
