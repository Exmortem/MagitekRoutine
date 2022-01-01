using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using DragoonRoutine = Magitek.Utilities.Routines.Dragoon;

namespace Magitek.Logic.Dragoon
{
    internal static class Aoe
    {
        public static async Task<bool> DoomSpike()
        {
            if (!DragoonSettings.Instance.UseAoe)
                return false;

            if (Core.Me.HasAura(Auras.DraconianFire))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 10 + x.CombatReach) < DragoonSettings.Instance.AoeEnemies)
                return false;

            return await Spells.DoomSpike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DraconianFury()
        {
            if (!DragoonSettings.Instance.UseAoe)
                return false;

            if (!Core.Me.HasAura(Auras.DraconianFire))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 10 + x.CombatReach) < DragoonSettings.Instance.AoeEnemies)
                return false;

            return await Spells.DraconianFury.Cast(Core.Me.CurrentTarget);
        }


        /***************************************************************************
         *                           Combo 1
         * *************************************************************************/
        public static async Task<bool> SonicThrust()
        {
            if (!DragoonSettings.Instance.UseAoe)
                return false;

            if (!DragoonRoutine.CanContinueComboAfter(Spells.DoomSpike) && !DragoonRoutine.CanContinueComboAfter(Spells.DraconianFury))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 10 + x.CombatReach) < DragoonSettings.Instance.AoeEnemies)
                return false;

            return await Spells.SonicThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CoerthanTorment()
        {
            if (!DragoonSettings.Instance.UseAoe)
                return false;

            if (!DragoonRoutine.CanContinueComboAfter(Spells.SonicThrust))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 10 + x.CombatReach) < DragoonSettings.Instance.AoeEnemies)
                return false;

            return await Spells.CoerthanTorment.Cast(Core.Me.CurrentTarget);
        }


        /****************************************************************************************************
         *                                             oGCD
         * Below are AOE which are part of Single Target Rotation, they are not part of UseAOE Toggle
         * **************************************************************************************************/
        public static async Task<bool> Geirskogul()
        {
            if (!DragoonSettings.Instance.UseGeirskogul)
                return false;
          
            //Geirskogul should only happen with 2 eyes. The only moment when Geirskogul is not executed at 2 eyes is during EW Opener
            if (ActionResourceManager.Dragoon.DragonGaze < 2)
                return false;

            return await Spells.Geirskogul.Cast(Core.Me.CurrentTarget);
        }
        
        
        public static async Task<bool> Nastrond()
        {
            if (!DragoonSettings.Instance.UseGeirskogul)
                return false;

            if (!DragoonSettings.Instance.UseNastrond)
                return false;

            if (!Spells.Nastrond.IsReady())
                return false;

            return await Spells.Nastrond.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WyrmwindThrust()
        {
            if (!DragoonSettings.Instance.UseWyrmwindThrust)
                return false;

            return await Spells.WyrmwindThrust.Cast(Core.Me.CurrentTarget);
        }
    }
}
