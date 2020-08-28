using ff14bot;
using Magitek.Models.Account;
using System;

namespace Magitek.Utilities
{
    internal static class ZoomHack
    {
        private static bool _isEnabled = false;

        private static bool _canZoomHack = true;

        public static void Toggle()
        {
            if (_isEnabled == BaseSettings.Instance.ZoomHack)
                return;

            if (!_canZoomHack)
                return;

            try
            {
                Core.Memory.Write(Core.Memory.Read<IntPtr>(Core.Memory.Process.MainModule.BaseAddress + 0x1D08BA0) + 0x11c, BaseSettings.Instance.ZoomHack ? 200f : 20f);
                _isEnabled = BaseSettings.Instance.ZoomHack;
            }
            catch (Exception)
            {
                Logger.Write($@"[Magitek] ZoomHack Failed due to FFXIV Update");
            }
        }
    }
}
