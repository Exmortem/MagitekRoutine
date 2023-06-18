using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Threading.Tasks;
using System.Linq;
using System;

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

    }
}
