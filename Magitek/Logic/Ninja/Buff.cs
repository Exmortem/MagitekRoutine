using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;


namespace Magitek.Logic.Ninja
{
    internal static class Buff
    {
        
        public static async Task<bool> Kassatsu()
        {

            if (Core.Me.ClassLevel < 50)
                return false;

            if (!Spells.Kassatsu.IsKnown())
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || NinjaRoutine.UsedMudras.Count() > 0)
                return false;

            return await Spells.Kassatsu.Cast(Core.Me);

        }

        public static async Task<bool> Bunshin()
        {

            if (Core.Me.ClassLevel < 80)
                return false;

            if (!Spells.Bunshin.IsKnown())
                return false;

            if (Spells.Mug.Cooldown == new TimeSpan(0, 0, 0))
                return false;

            return await Spells.Bunshin.Cast(Core.Me);

        }

        public static async Task<bool> Meisui()
        {

            if (Core.Me.ClassLevel < 72)
                return false;

            if (!Spells.Meisui.IsKnown())
                return false;

            if (MagitekActionResourceManager.Ninja.NinkiGauge + 50 > 100)
                return false;

            if (Spells.TrickAttack.Cooldown <= new TimeSpan(0, 0, 20))
                return false;

            if (Casting.SpellCastHistory.First().Spell == Spells.TrickAttack)
                return false;

            if (!NinjaRoutine.GlobalCooldown.IsWeaveWindow(1))
                return false;

            return await Spells.Meisui.Cast(Core.Me);

        }

        public static async Task<bool> Huraijin()
        {

            if (Core.Me.ClassLevel < 60)
                return false;

            if (!Spells.Huraijin.IsKnown())
                return false;

            if (ActionResourceManager.Ninja.HutonTimer > new TimeSpan(0))
                return false;

            return await Spells.Huraijin.Cast(Core.Me.CurrentTarget);

        }

    }
}