using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using System;
using System.Linq;

namespace Magitek.Utilities
{
    internal static class Globals
    {
        public static bool InParty => PartyManager.IsInParty;
        public static bool PartyInCombat => Core.Me.InCombat || Combat.Enemies.Any(/*r => r.TaggerType == 2*/);
        public static bool InGcInstance => RaptureAtkUnitManager.Controls.Any(r => r.Name == "GcArmyOrder");
        public static bool OnPvpMap => Core.Me.OnPvpMap();
        public static bool InActiveDuty => DutyManager.InInstance && Duty.State() == Duty.States.InProgress;
        public static GameObject HealTarget;
        public static int AnimationLockMs = 770;
        public static TimeSpan AnimationLockTimespan = TimeSpan.FromMilliseconds(AnimationLockMs);
    }
}
