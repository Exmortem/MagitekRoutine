using ff14bot;
using ff14bot.Managers;
using Magitek.Models.Account;
using System;

namespace Magitek.Utilities
{
    internal static class ZoomHack
    {
        private static bool _isEnabled;

        public static void Toggle()
        {
            if (_isEnabled == BaseSettings.Instance.ZoomHack)
                return;
            try
            {
                Core.Memory.Write(CameraManager.CameraPtr + 0x11c, BaseSettings.Instance.ZoomHack ? 200f : 20f);
                _isEnabled = BaseSettings.Instance.ZoomHack;
            }
            catch (Exception)
            {
                Logger.Write($@"[Magitek] ZoomHack Failed due to FFXIV Update");
            }
        }
    }
}