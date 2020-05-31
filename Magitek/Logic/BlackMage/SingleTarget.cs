using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.BlackMage
{
    internal static class SingleTarget
    {
        public static async Task<bool> Xenoglossy()
        {
            if (!ActionResourceManager.BlackMage.PolyglotStatus)
                return false;

            if (Casting.LastSpell == Spells.Xenoglossy)
                return false;

            //If we don't have Xeno, Foul is single target
            if (Core.Me.ClassLevel < 80
                && ActionResourceManager.BlackMage.UmbralStacks == 3)
                return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            // If we're moving in combat
            if (MovementManager.IsMoving)
            {
                // If we don't have any procs (while in movement), cast
                if (!Core.Me.HasAura(Auras.Swiftcast)
                    && !Core.Me.HasAura(Auras.Triplecast)
                    && !Core.Me.HasAura(Auras.FireStarter)
                    && !Core.Me.HasAura(Auras.ThunderCloud))
                    return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);
            }
            //If while in Umbral 3 and, we didn't use Thunder in the Umbral window
            if (ActionResourceManager.BlackMage.UmbralStacks == 3 && Casting.LastSpell != Spells.Thunder3)
            {
                //We don't have max mana
                if (Core.Me.CurrentMana < 10000 && Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, 5000))   
                    return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> Despair()
        {
            if (!Core.Me.HasEnochian())
                return false;

            if (Casting.LastSpell == Spells.Despair)
                return false;

            // If we're not in astral, stop
            if (ActionResourceManager.BlackMage.AstralStacks != 3)
                return false;

            // If our mana is lower than 2400, and we have despair unlocked cast
            if (Core.Me.ClassLevel > 71 && Core.Me.CurrentMana < 2400 && Core.Me.CurrentMana != 0)
                return await Spells.Despair.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Fire()
        {
            //Low level logic
            if (Core.Me.ClassLevel < 40)
            {
                if (Core.Me.CurrentMana > 1600)
                    return await Spells.Fire.Cast(Core.Me.CurrentTarget);
                
                return await Spells.Transpose.Cast(Core.Me);
            }
                

            if (Core.Me.CurrentMana < 1600)
                return false;
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
            //Use Fire early to force sharpcast if about to run out 
            if (Core.Me.HasAura(Auras.Sharpcast) && ActionResourceManager.BlackMage.UmbralHearts == 0)
                if (!Core.Me.HasAura(Auras.Sharpcast, true, 5500))
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
            //if we cast Manafont, make sure it doesn't skip ahead before we actually get mp
            if (Casting.LastSpell == Spells.ManaFont)
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
            //Low level logic
            if (Core.Me.ClassLevel < 40)
            {
                if (Casting.LastSpell != Spells.Thunder
                    && Casting.LastSpell == Spells.Transpose)
                    return await Spells.Thunder.Cast(Core.Me.CurrentTarget);

                return false;
            }
            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // If the last spell we cast is triple cast, stop
            if (Casting.LastSpell == Spells.Triplecast)
                return false;

            // If we have the triplecast aura, stop
            if (Core.Me.HasAura(Auras.Triplecast))
                return false;

            if (MovementManager.IsMoving && Core.Me.HasAura(Auras.ThunderCloud))
            {
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);
            }

            if (Casting.LastSpell == Spells.Thunder3)
                return false;

            // If we have thunder cloud, but we don't have at least 2 seconds of it left, use the proc
            if (Core.Me.HasAura(Auras.ThunderCloud) && !Core.Me.HasAura(Auras.ThunderCloud, true, 4000))
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);

            // Refresh thunder if it's about to run out
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, 4500)
                && Casting.LastSpell != Spells.Thunder3)
                await Spells.Thunder3.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Blizzard4()
        {
            if (Casting.LastSpell == Spells.Blizzard4)
                return false;

            if (!Core.Me.HasEnochian())
                return false;

            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // While in Umbral 
            if (ActionResourceManager.BlackMage.UmbralStacks > 0)
            {
                // If we have less than 3 hearts, cast
                if (ActionResourceManager.BlackMage.UmbralHearts < 3)
                    return await Spells.Blizzard4.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> Blizzard3()
        {

            if (Casting.LastSpell == Spells.Blizzard3)
                return false;

            // If we have no umbral or astral stacks, cast 
            if (ActionResourceManager.BlackMage.AstralStacks <= 0 && ActionResourceManager.BlackMage.UmbralStacks == 0)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);

            // If our mana is less than 800 while in astral
            if (ActionResourceManager.BlackMage.AstralStacks > 0 && Core.Me.CurrentMana < 800)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);
            //If we're low medium level, use it at a different time
            if (Core.Me.ClassLevel < 72 && Core.Me.CurrentMana < 1500)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);

            if (ActionResourceManager.BlackMage.AstralStacks <= 1 && ActionResourceManager.BlackMage.UmbralStacks <= 1)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Blizzard()
        {
            //stop being level 1, fool
            if (Core.Me.ClassLevel < 2)
                return await Spells.Blizzard.Cast(Core.Me.CurrentTarget);

            //Low level logic
            if (Core.Me.ClassLevel < 40)
            {
                if (Core.Me.CurrentMana == 10000)
                    return false;

                if (ActionResourceManager.BlackMage.UmbralStacks > 0)
                    return await Spells.Blizzard.Cast(Core.Me.CurrentTarget);

                return await Spells.Transpose.Cast(Core.Me);
            }
            return false;

        }
    }
}
