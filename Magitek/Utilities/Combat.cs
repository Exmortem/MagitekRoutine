using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Utilities
{
    internal static class Combat
    {
        public static readonly List<BattleCharacter> Enemies = new List<BattleCharacter>();
        public static readonly Stopwatch CombatTime = new Stopwatch();
        public static readonly Stopwatch OutOfCombatTime = new Stopwatch();
        public static readonly Stopwatch MovingInCombatTime = new Stopwatch();
        public static readonly Stopwatch NotMovingInCombatTime = new Stopwatch();
        public static int CombatTotalTimeLeft;
        public static readonly Stopwatch DutyTime = new Stopwatch();
        public static double CurrentTargetCombatTimeLeft;

        public static bool IsMoving(GameObject target)
        {
            //if (!Tracking.EnemyInfos.Any(r => r.Unit == target && r.IsMoving)) return false;

            var movingTarget = Tracking.EnemyInfos.FirstOrDefault(r => r.Unit == target && r.IsMoving);

            return movingTarget != null && movingTarget.IsMovingChange.ElapsedMilliseconds > 2000;
        }

        public static void AdjustCombatTime()
        {
            //General Combat Status Check
            if (Core.Me.InCombat)
            {
                AdjustInCombatTimers();
                return;
            }

            //If Our Party Has Tagged Something We're Also InCombat
            if (Globals.InParty && Globals.PartyInCombat)
            {
                AdjustInCombatTimers();
                return;
            }

            AdjustOutOfCombatTimers();

            // Private methods
            void AdjustInCombatTimers()
            {
                if (!CombatTime.IsRunning)
                {
                    CombatTime.Start();
                }

                if (OutOfCombatTime.IsRunning)
                {
                    OutOfCombatTime.Reset();
                }

                if (MovementManager.IsMoving)
                {
                    if (!MovingInCombatTime.IsRunning)
                    {
                        MovingInCombatTime.Restart();
                    }

                    if (NotMovingInCombatTime.IsRunning)
                    {
                        NotMovingInCombatTime.Reset();
                    }
                }
                else
                {
                    if (MovingInCombatTime.IsRunning)
                    {
                        MovingInCombatTime.Reset();
                    }

                    if (!NotMovingInCombatTime.IsRunning)
                    {
                        NotMovingInCombatTime.Restart();
                    }
                }
            }

            void AdjustOutOfCombatTimers()
            {
                if (CombatTime.IsRunning)
                {
                    CombatTime.Reset();
                }

                if (!OutOfCombatTime.IsRunning)
                {
                    OutOfCombatTime.Start();
                }
            }
        }

        public static void AdjustDutyTime()
        {
            var inDuty = DutyManager.InInstance;

            if (inDuty && !DutyTime.IsRunning)
                DutyTime.Start();

            if (inDuty || !DutyTime.IsRunning)
                return;

            DutyTime.Reset();
            DutyTime.Stop();
        }
    }
}
