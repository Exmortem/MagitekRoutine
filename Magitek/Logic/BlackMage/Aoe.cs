using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
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
            //Can't use whatcha don't have
            if (Core.Me.ClassLevel < 70)
                return false;

            //if you don't have Aspect Mastery, just SMASH THAT FOUL BUTTON
          if (Core.Me.ClassLevel < 80)
                if (ActionResourceManager.BlackMage.UmbralStacks == 3)
                    return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            //requires Polyglot
            if (!ActionResourceManager.BlackMage.PolyglotStatus)
                return false;
          
            //Only use in Astral 3
            if (ActionResourceManager.BlackMage.UmbralStacks != 3)
                return false;
          
            //Only use if you have no MP 
            if (Core.Me.CurrentMana != 0)
                return false;

            return await Spells.Foul.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Flare()
        {
            if (Core.Me.ClassLevel < 72)
                return false; 

            //Only cast Flare if you have enough mp
            if (Core.Me.CurrentMana < 800)
                return false;

            return await Spells.Flare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Freeze()
        {
            if (Core.Me.ClassLevel < 68)
                return false; 

            //Wait until we don't have enough mp to cast Flare  
            if (Core.Me.CurrentMana > 799)
                return false;

            return await Spells.Freeze.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Thunder4()
        {
            //If we aren't using Flare to dump MP, just refresh when needed
            if (Core.Me.ClassLevel < 72 && !Core.Me.CurrentTarget.HasAura(Auras.Thunder4, true, 4000))
                if (Casting.LastSpell != Spells.Thunder4)
                    return await Spells.Thunder4.Cast(Core.Me.CurrentTarget);

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
            //Never use this trash spell if we have Flare+Elemental Trait unlocked...
            if (Core.Me.ClassLevel > 72)
                return false;

            return await Spells.Fire2.Cast(Core.Me.CurrentTarget);
        }
    }
}
    

