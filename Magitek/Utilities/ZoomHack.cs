﻿using System;
using ff14bot;
using Magitek.Models.Account;

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
            
            Core.Memory.Write(Core.Memory.Read<IntPtr>(Core.Memory.Process.MainModule.BaseAddress + 0x1BFB7A0) + 0x120, BaseSettings.Instance.ZoomHack ? 200f : 20f);
            _isEnabled = BaseSettings.Instance.ZoomHack;
        }
    }
}