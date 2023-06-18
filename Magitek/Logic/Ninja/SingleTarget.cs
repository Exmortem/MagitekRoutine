using System.Threading.Tasks;
using System.Linq;
using System;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class SingleTarget
    {

        #region Base Combo

        public static async Task<bool> SpinningEdge()
        {
            if (!Spells.SpinningEdge.CanCast(Core.Me.CurrentTarget))
                return false;


            return await Spells.SpinningEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GustSlash()
        {

            if (Core.Me.ClassLevel < 4)
                return false;

            if (ActionManager.LastSpell != Spells.SpinningEdge)
                return false;

            if (!Spells.GustSlash.CanCast(Core.Me.CurrentTarget))
                return false;

            return await Spells.GustSlash.Cast(Core.Me.CurrentTarget);

        }

        //Rear Modifier
        public static async Task<bool> AeolianEdge()
        {

            if (Core.Me.ClassLevel < 26)
                return false;

            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;

            if (!Spells.AeolianEdge.CanCast(Core.Me.CurrentTarget))
                return false;

            return await Spells.AeolianEdge.Cast(Core.Me.CurrentTarget);

        }

        //Flank Modifier
        //should be used over aeolian edge if no true north or not in rear
        public static async Task<bool> ArmorCrush()
        {

            if (Core.Me.ClassLevel < 54 || !Spells.ArmorCrush.IsKnown())
                return false;

            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;
            
            //Dont cast if current huton timer plus 30 seconds is greater than 60 seconds
            if (ActionResourceManager.Ninja.HutonTimer.Add(new TimeSpan(0, 0, 30)) >  new TimeSpan(0,1,0) )
                return false;

            if (!Spells.ArmorCrush.CanCast(Core.Me.CurrentTarget))
                return false;

            return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);

        }

        #endregion
        
        //Missing logic for st and mt
        public static async Task<bool> Bhavacakra()
        {

            if (Core.Me.ClassLevel < 68)
                return false;

            if (!Spells.Bhavacakra.IsKnown())
                return false;

            //dumping Bhavacakra during Burst Window is missing
            if (MagitekActionResourceManager.Ninja.NinkiGauge < 90 || (Spells.Mug.Cooldown > new TimeSpan(0, 0, 7) && MagitekActionResourceManager.Ninja.NinkiGauge + 40 < 90 ))
                return false;

            //Smart Target Logic needs to be addded
            return await Spells.Bhavacakra.Cast(Core.Me.CurrentTarget);
        }

        //Missing range check
        public static async Task<bool> FleetingRaiju()
        {

            if (Core.Me.ClassLevel < 90)
                return false;

            if (!Spells.FleetingRaiju.IsKnown())
                return false;

            if (!Core.Me.HasMyAura(Auras.RaijuReady))
                return false;

            return await Spells.FleetingRaiju.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> ForkedRaiju()
        {

            if (Core.Me.ClassLevel < 90)
                return false;

            if (!Spells.ForkedRaiju.IsKnown())
                return false;

            if (!Core.Me.HasMyAura(Auras.RaijuReady))
                return false;

            return await Spells.ForkedRaiju.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> Mug()
        {

            if (Core.Me.ClassLevel < 15)
                return false;

            if (!Spells.Mug.IsKnown())
                return false;

            if (MagitekActionResourceManager.Ninja.NinkiGauge + 40 > 100)
                return false;

            return await Spells.Mug.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> TrickAttack()
        {

            if (Core.Me.ClassLevel < 18)
                return false;

            if (!Spells.TrickAttack.IsKnown())
                return false;

            //Missing logic for weaving late

            if (Spells.Mug.Cooldown == new TimeSpan(0, 0, 0))
                return false;

            return await Spells.TrickAttack.Cast(Core.Me.CurrentTarget);
        }

    }
}
