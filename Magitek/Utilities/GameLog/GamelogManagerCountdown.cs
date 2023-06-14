using System;
using System.Timers;

namespace Magitek.Utilities.GamelogManager
{
    public class GamelogManagerCountdown
    {
        private static Timer CountdownTimer;
        private static int CountdownRemaining = -1;
        private static bool IsRunning = false;

        public static void RegisterAndStartCountdown(int countdownTime)
        {
            if (!IsRunning)
            {
                StopCooldown();
                CountdownRemaining = countdownTime;

                CountdownTimer = new Timer(1000);
                CountdownTimer.Interval = 1000;
                CountdownTimer.Elapsed += OnTimedEvent;
                CountdownTimer.AutoReset = true;
                CountdownTimer.Enabled = true;
                CountdownTimer.Start();

                IsRunning = true;
            }
        }

        public static int GetCurrentCooldown()
        {
            return CountdownRemaining;
        }

        public static bool IsCountdownRunning()
        {
            return IsRunning;
        }

        public static void StopCooldown()
        {
            if (CountdownTimer != null)
            {
                CountdownTimer.Stop();
                CountdownTimer.Enabled = false;
                CountdownTimer.AutoReset = false;
            }
            CountdownRemaining = -1;
            IsRunning = false;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //Logger.WriteInfo($@"[Countdown] Remaining = {CountdownRemaining}");
            if (CountdownRemaining < 0)
                StopCooldown();
            else
                CountdownRemaining--;
        }
    }
}
