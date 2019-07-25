using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;

namespace Magitek.Logic.BlackMage
{
    internal static class Aoe
    {
        public static async Task<bool> Foul()
        {
            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3)
                return false;
            //requires Polyglot
            if (!ActionResourceManager.BlackMage.PolyglotStatus)
                return false;
            //Only use in Astral 3
            if (ActionResourceManager.BlackMage.AstralStacks != 3)
                return false;
            //Only use if you have no MP 
            if (Core.Me.CurrentMana != 0)
                return false;

            return await Spells.Foul.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Flare()
        {
            if (Core.Me.CurrentTarget.EnemiesNearby(10).Count() < 2)
                return false;

            //Only cast Flare if you have enough mp
            if (Core.Me.CurrentMana < 800)
                return false;

            return await Spells.Flare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Freeze()
        {
            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3)
                return false;
            //Wait until we don't have enough mp to cast Flare  
            if (Core.Me.CurrentMana > 799)
                return false;

            return await Spells.Freeze.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Thunder4()
        {
            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3)
                return false;
            //Only use in Astral 3
            if (ActionResourceManager.BlackMage.AstralStacks != 3)
                return false;
            //Only use if you have no MP 
            if (Core.Me.CurrentMana != 0)
                return false;
            //If we have a polyglot, don't use T4
            if (ActionResourceManager.BlackMage.PolyglotStatus)
                return false;

            return await Spells.Thunder2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fire2()
        {
            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3)
                return false;
            //Never use this trash spell if we have Flare unlocked...
            if (Core.Me.ClassLevel > 49)
                return false;
            return await Spells.Fire2.Cast(Core.Me.CurrentTarget);
        }


    }
}
    

