using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;
using System;
using System.Threading;

namespace Magitek.Logic.Monk
{
    class PositionalToast
    {

        public static void SendToast(string message, int displaySeconds)
        {

            Core.OverlayManager.AddToast(() => message,
            TimeSpan.FromSeconds(displaySeconds),
            System.Windows.Media.Colors.Red,
            System.Windows.Media.Colors.Black,
            new System.Windows.Media.FontFamily("Consolas"));

        }

        public static void PositionalLogic()
        {
            while (MonkSettings.Instance.UsePositionalToasts)
            {
                if (Core.Me.HasAura(Auras.OpoOpoForm) && Core.Me.HasAura(Auras.LeadenFist) && !Core.Me.HasAura(Auras.PerfectBalance))
                {
                    SendToast("Bootshine: Get behind Enemy", 1);
                }

                if (Core.Me.HasAura(Auras.RaptorForm) && !Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1100) && !Core.Me.HasAura(Auras.PerfectBalance))
                {
                    SendToast("TwinSnakes: Side of Enemy", 1);
                }

                if (Core.Me.HasAura(Auras.RaptorForm) && Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000) && !Core.Me.HasAura(Auras.PerfectBalance))
                {
                    SendToast("TrueStrike: Get behind Enemy", 1);
                }

                if (Core.Me.HasAura(Auras.OpoOpoForm) && !Core.Me.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000) && !Core.Me.HasAura(Auras.PerfectBalance))
                {
                    SendToast("DragonKick: Side of Enemy", 1);
                }

                Thread.Sleep(1850);
            }
        }
    }
}
