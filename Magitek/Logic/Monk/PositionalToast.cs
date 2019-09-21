using ff14bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Models.QueueSpell;
using Magitek.Utilities.Routines;
using Magitek.Utilities;

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

        public static async Task<bool> PositionalLogic()
        {

            if (!MonkSettings.Instance.UsePositionalToasts)
                return false;

            if (Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if(Casting.LastSpell == Spells.Bootshine && !Core.Me.HasAura(Auras.OpoOpoForm) && Utilities.Routines.Monk.UseToast ==1)
                Utilities.Routines.Monk.UseToast = 0;

            if (Casting.LastSpell == Spells.TrueStrike && !Core.Me.HasAura(Auras.RaptorForm) && Utilities.Routines.Monk.UseToast == 2)
                Utilities.Routines.Monk.UseToast = 0;

            if (Casting.LastSpell == Spells.DragonKick && !Core.Me.HasAura(Auras.OpoOpoForm) && Utilities.Routines.Monk.UseToast == 3 || Core.Me.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000) && !Core.Me.HasAura(Auras.OpoOpoForm) && Utilities.Routines.Monk.UseToast == 3)
                Utilities.Routines.Monk.UseToast = 0;

            if (Casting.LastSpell == Spells.TwinSnakes && !Core.Me.HasAura(Auras.RaptorForm) && Utilities.Routines.Monk.UseToast == 4 || Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000) && !Core.Me.HasAura(Auras.RaptorForm) && Utilities.Routines.Monk.UseToast == 4)
                Utilities.Routines.Monk.UseToast = 0;

            if (Core.Me.HasAura(Auras.OpoOpoForm) && Core.Me.HasAura(Auras.LeadenFist) && Utilities.Routines.Monk.UseToast == 0)
            {
                SendToast("Bootshine: Get behind Enemy", 2);
                Utilities.Routines.Monk.UseToast = 1; //Bootshine
                return true;
            }

            if (Core.Me.HasAura(Auras.RaptorForm) && Core.Me.HasAura(Auras.TwinSnakes) && Utilities.Routines.Monk.UseToast == 0 && Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
            {
                SendToast("TrueStrike: Get behind Enemy", 2);
                Utilities.Routines.Monk.UseToast = 2; //TrueStrike
                return true;
            }

            if (Core.Me.HasAura(Auras.OpoOpoForm) && Utilities.Routines.Monk.UseToast == 0 && !Core.Me.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000))
            {
                SendToast("DragonKick: Side of Enemy", 2);
                Utilities.Routines.Monk.UseToast = 3; //DragonKick
                return true;
            }

            if (Core.Me.HasAura(Auras.RaptorForm) && Core.Me.HasAura(Auras.TwinSnakes) && Utilities.Routines.Monk.UseToast == 0 && !Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
            {
                SendToast("TwinSnakes: Side of Enemy", 2);
                Utilities.Routines.Monk.UseToast = 4; //TwinSnakes
                return true;
            }



            return false;
        }

    }
}
