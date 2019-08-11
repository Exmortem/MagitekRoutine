using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;

namespace Magitek.Logic.BlackMage
{
    internal static class SingleTarget
    {
        public static async Task<bool> Xenoglossy()
        {
            if (Casting.LastSpell == Spells.Xenoglossy)
                return false; 

            // If we're moving in combat
            if (MovementManager.IsMoving)
            {
                // If we don't have procs (while in movement), cast
                if (!Core.Me.HasAura(Auras.FireStarter))
                    return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);
            }
            //If while in Umbral 3 and, we didn't use Thunder in the Umbral window
            if (ActionResourceManager.BlackMage.UmbralStacks == 3 && Casting.LastSpell != Spells.Thunder3)
            {
                //We don't have max mana
                if(Core.Me.CurrentMana < 10000)
                    return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);
            }
        return false;        
        }

        public static async Task<bool> Despair()
        {
            if (!Core.Me.HasEnochian())
                return false;

            // If we're not in astral, stop
            if (ActionResourceManager.BlackMage.AstralStacks != 3)
                return false;

            // If our mana is lower than 2400, and we have despair unlocked cast
            if (Core.Me.ClassLevel > 71 && Core.Me.CurrentMana < 2400)
                return await Spells.Despair.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Fire()
        {
            //test for no enochian
            if (Core.Me.ClassLevel < 56 && Core.Me.CurrentMana > 800)
                return await Spells.Fire.Cast(Core.Me.CurrentTarget);
            //only use in astral fire
            if (ActionResourceManager.BlackMage.AstralStacks != 3)
                return false;
            //refresh astral fire at 5s
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds < 5000)
                return await Spells.Fire.Cast(Core.Me.CurrentTarget);
            //If we don't have despair, use fire 1 to dump mana
            if (Core.Me.ClassLevel < 71 && Core.Me.CurrentMana < 2400)
                return await Spells.Fire.Cast(Core.Me.CurrentTarget);
            return false;

        }

        public static async Task<bool> Fire4()
        {
            if (!Core.Me.HasEnochian())
                return false;

            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // If we have 3 astral stacks and our mana is equal to or greater than 2400
            if (ActionResourceManager.BlackMage.AstralStacks == 3 && Core.Me.CurrentMana >= 2400)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Fire3()
        {
            // Use if we're in Umbral and we have 3 hearts and have max mp
            if (ActionResourceManager.BlackMage.UmbralHearts == 3 && ActionResourceManager.BlackMage.UmbralStacks == 3 && Core.Me.CurrentMana == 10000)
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

            // If we're moving in combat in Astral Fire
            if (MovementManager.IsMoving && ActionResourceManager.BlackMage.AstralStacks == 3)
            {
                // If we have the proc, cast
                if (Core.Me.HasAura(Auras.FireStarter))
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);
            }

            // Use if we're in Astral and we have Firestarter, but Firestarter has 3 seconds or less left
            if (ActionResourceManager.BlackMage.AstralStacks > 0 && Core.Me.HasAura(Auras.FireStarter) && !Core.Me.HasAura(Auras.FireStarter, true, 3000))
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

            //Use if we're at the end of Astral phase and we have a Fire3 proc
            if (ActionResourceManager.BlackMage.AstralStacks > 0 && Core.Me.HasAura(Auras.FireStarter) && Core.Me.CurrentMana <= 1200)
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);
                  
            return false;
        }

        public static async Task<bool> Thunder3()
        {

            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // If the last spell we cast is triple cast, stop
            if (Casting.LastSpell == Spells.Triplecast)
                return false;
            // It takes a second for thunder dot to actually hit the boss...
            if (Casting.LastSpell == Spells.Thunder3)
                return false;

            // If we have the triplecast aura, stop
            if (Core.Me.HasAura(Auras.Triplecast))
                return false;

            // save for Umbral Phase
            if (Core.Me.CurrentMana < 800)
                return false;

            // If we have thunder cloud, but we don't have at least 2 seconds of it left, use the proc
            if (Core.Me.HasAura(Auras.ThunderCloud) && !Core.Me.HasAura(Auras.ThunderCloud, true, 2000))
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);

            // Refresh thunder if it's about to run out
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, 3000))
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Blizzard4()
        {
            if (!Core.Me.HasEnochian())
                return false;

            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // While in Umbral 
            if (ActionResourceManager.BlackMage.UmbralStacks > 0)
            {
                // If we have less than 3 hearts, cast
                if (ActionResourceManager.BlackMage.UmbralHearts != 3)
                    return await Spells.Blizzard4.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> Blizzard3()
        {
            // If we have no umbral or astral stacks, cast 
            if (ActionResourceManager.BlackMage.AstralStacks == 0 && ActionResourceManager.BlackMage.UmbralStacks == 0)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);

            // If our mana is less than 800 while in astral
            if (ActionResourceManager.BlackMage.AstralStacks > 0 && Core.Me.CurrentMana < 800)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Blizzard()   
        {
           //this shit sucks if you're using it, buy a level skip
            if (Core.Me.ClassLevel < 56 && Core.Me.CurrentMana < 801)
            return await Spells.Blizzard.Cast(Core.Me.CurrentTarget);

            return false;

        }
    }
}
