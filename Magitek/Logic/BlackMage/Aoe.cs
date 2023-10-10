using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.BlackMage
{
    internal static class Aoe
    {
        public static async Task<bool> Foul()
        {
            //requires Polyglot
            if (!ActionResourceManager.BlackMage.PolyglotStatus)
                return false;

            //Can't use whatcha don't have
            if (Core.Me.ClassLevel < 70)
                return false;

            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            //if you don't have Aspect Mastery, just SMASH THAT FOUL BUTTON
            if (Core.Me.ClassLevel < 80)
                if (ActionResourceManager.BlackMage.UmbralStacks == 3)
                    return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            //If at 2 stacks of polyglot and 5 seconds from another stack, cast
            if (ActionResourceManager.BlackMage.PolyglotCount == 2
                && MagitekActionResourceManager.BlackMage.PolyGlotTimer <= 5000)
                return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            // If we're moving in combat
            if (Core.Me.ClassLevel >= 80 && MovementManager.IsMoving)
            {
                // If we don't have any procs (while in movement), cast
                if (!Core.Me.HasAura(Auras.Swiftcast)
                    && !Core.Me.HasAura(Auras.Triplecast)
                    && !Core.Me.HasAura(Auras.FireStarter)
                    && !Core.Me.HasAura(Auras.ThunderCloud))
                    return await Spells.Foul.Cast(Core.Me.CurrentTarget);
            }
            
            //If at max polyglot stacks, cast
            if (ActionResourceManager.BlackMage.PolyglotCount == 2
                && Casting.LastSpell == Spells.Flare)
                return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            //Only use in Umbral 3
            if (ActionResourceManager.BlackMage.UmbralStacks != 3)
                return false;            

            //If we have Umbral hearts, Freeze has gone off
            if (ActionResourceManager.BlackMage.UmbralHearts >= 1)
                if (Casting.LastSpell != Spells.Foul)
                    return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Flare()
        {
            //Can't use in Umbral Ice anymore
            if (ActionResourceManager.BlackMage.UmbralStacks > 0)
                return false;

            if (Core.Me.ClassLevel < Spells.Flare.LevelAcquired)
                return false;

            if (Core.Me.ClassLevel < 72)
            {
                if (Core.Me.ClassLevel >= 50
                    && Core.Me.ClassLevel < 60
                    && Casting.LastSpell == Spells.Fire2)
                {
                    if (Core.Me.CurrentMana <= 3000)
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    //if (Core.Me.CurrentMana < 800)
                    //    return await Spells.Transpose.Cast(Core.Me);

                    return false;
                }
                if (Core.Me.ClassLevel >= 68)
                {
                    if ((Casting.LastSpell == Spells.Fire2)
                        && (Core.Me.CurrentMana <= 3000))
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    if (Core.Me.CurrentMana >= 800
                        && ActionResourceManager.BlackMage.AstralStacks == 0
                        && Core.Me.HasAura(Auras.EnhancedFlare, true))
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    //if (Core.Me.CurrentMana < 800)
                    //    return await Spells.Transpose.Cast(Core.Me);

                    return false;
                }
                return false;
            }

            //Wait until at least two high fire II have gone off first
            if (Core.Me.CurrentMana > 7000)
                return false;

            //Only cast Flare if you have enough mp
            if (Core.Me.CurrentMana < 800)
                return false;

            //Force flare after manafont
            if (Casting.LastSpell == Spells.ManaFont)
                return await Spells.Flare.Cast(Core.Me.CurrentTarget);

            return await Spells.Flare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Freeze()
        {
            //If we don't have Freeze, how can we cast it?
            if (Core.Me.ClassLevel < Spells.Freeze.LevelAcquired)
                return false;

            if (Casting.LastSpell == Spells.Freeze)
                return false;

            //Can only use in Umbral Ice
            if (ActionResourceManager.BlackMage.UmbralStacks < 1)
                return false;

            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // While in Umbral 
            if (ActionResourceManager.BlackMage.UmbralStacks > 0)
            {
                // If we have less than 3 hearts, cast
                if (ActionResourceManager.BlackMage.UmbralHearts < 3)
                    return await Spells.Freeze.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> Thunder4()
        {
            if (!BlackMageSettings.Instance.ThunderSingle)
                return false;
            
            // If we need to refresh stack timer, stop
            if (ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds <= 5000)
                return false;

            // If the last spell we cast is triple cast, stop
            if (Casting.LastSpell == Spells.Triplecast)
                return false;

            // If we have the triplecast aura, stop
            if (Core.Me.HasAura(Auras.Triplecast))
                return false;

            //Cast any time thundercloud procs - moved up as TC procs do full damage up front and it doesn't matter how much time in combat is left
            if (Core.Me.HasAura(Auras.ThunderCloud)
                //Also cast if we have Sharpcast active
                || Core.Me.HasAura(Auras.Sharpcast))
                return await Spells.Thunder2.Cast(Core.Me.CurrentTarget);

            // Don't dot if time in combat less than configured seconds left
            if (Combat.CombatTotalTimeLeft <= BlackMageSettings.Instance.ThunderTimeTillDeathSeconds)
                return false;

            //Only cast in Umbral 3 - should be cast in either if needed
            //if (ActionResourceManager.BlackMage.UmbralStacks != 3)
            //    return false;

            //If we don't need to refresh Thunder, skip
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder4, true, BlackMageSettings.Instance.ThunderRefreshSecondsLeft * 1000 + 500))
                return false;

            //Same for Thunder2
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder2, true, BlackMageSettings.Instance.ThunderRefreshSecondsLeft * 1000 + 500))
                return false;

            //Same for Thunder3
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, BlackMageSettings.Instance.ThunderRefreshSecondsLeft * 1000 + 500))
                return false;

            //Same for Thunder
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder, true, BlackMageSettings.Instance.ThunderRefreshSecondsLeft * 1000 + 500))
                return false;

            if (Core.Me.ClassLevel < 68)
                if (Casting.LastSpell != Spells.Thunder2)
                    if (Casting.LastSpell == Spells.Transpose
                        || Casting.LastSpell == Spells.Blizzard2
                        || Casting.LastSpell == Spells.Freeze)
                        return await Spells.Thunder2.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel < 72)
                if (Casting.LastSpell != Spells.Thunder4)
                {
                    if (Casting.LastSpell == Spells.Transpose
                        || Casting.LastSpell == Spells.Foul
                        || Casting.LastSpell == Spells.Freeze)
                        return await Spells.Thunder4.Cast(Core.Me.CurrentTarget);
                }

            if (Core.Me.ClassLevel >= 72)
            {
                if (Casting.LastSpell != Spells.Thunder4)
                        return await Spells.Thunder4.Cast(Core.Me.CurrentTarget);
                
            }
            return false;
        }

        public static async Task<bool> Fire2()
        {
            if (Core.Me.ClassLevel < Spells.Fire2.LevelAcquired)
                return false;

            //Low-level logic
            if (Core.Me.ClassLevel >= 18
                && Core.Me.ClassLevel < 35)
            {
                if (Core.Me.CurrentMana >= 3000)
                    return await Spells.Fire2.Cast(Core.Me.CurrentTarget);

                return false;
            }
            
            // level 35-90 logic
            if (Core.Me.ClassLevel >= 35)
                
            {
                if (ActionResourceManager.BlackMage.UmbralHearts == 1)
                    return false;

                // Use if we're in Umbral and we have 3 hearts and have max mp
                if (ActionResourceManager.BlackMage.UmbralHearts == 3 && ActionResourceManager.BlackMage.UmbralStacks == 3 && Core.Me.CurrentMana == 10000)
                    return await Spells.Fire2.Cast(Core.Me.CurrentTarget);

                //If we have flare and umbral hearts just cast twice
                if (Core.Me.ClassLevel >= 58)
                {
                    if (Core.Me.CurrentMana > 7000)
                        return await Spells.Fire2.Cast(Core.Me.CurrentTarget);

                    return false;
                }
                
                if (Core.Me.CurrentMana > 3000)
                    return await Spells.Fire2.Cast(Core.Me.CurrentTarget);

                return false;
            }
            return false;
        }

        public static async Task<bool> Blizzard2()
        {
            //Low-level logic
            if (Core.Me.ClassLevel >= 12
                && Core.Me.ClassLevel < 35)
            {
                if (Core.Me.CurrentMana < 3000)
                {
                    if (ActionResourceManager.BlackMage.AstralStacks > 0)
                      await Spells.Transpose.Cast(Core.Me);
                    
                    return await Spells.Blizzard2.Cast(Core.Me.CurrentTarget);
                }

                if (Core.Me.CurrentMana >= 9600 && ActionResourceManager.BlackMage.UmbralStacks < 0)
                    return await Spells.Transpose.Cast(Core.Me);

                if (ActionResourceManager.BlackMage.UmbralStacks < 0)
                    return await Spells.Blizzard2.Cast(Core.Me.CurrentTarget);

                return false;

            }

            if (Casting.LastSpell == Spells.Blizzard2
                || Casting.LastSpell == Spells.HighBlizzardII
                || Casting.LastSpell == Spells.ManaFont)
                return false;

            // If our mana is less than 3000 while in astral and can not cast flare
            if (ActionResourceManager.BlackMage.AstralStacks > 0 && Core.Me.CurrentMana < 3000 && Core.Me.ClassLevel < Spells.Flare.LevelAcquired)
                return await Spells.Blizzard2.Cast(Core.Me.CurrentTarget);

            // If our mana is 0 then we have completed rotation with flare
            if (ActionResourceManager.BlackMage.AstralStacks > 0 && Core.Me.CurrentMana == 0)
                return await Spells.Blizzard2.Cast(Core.Me.CurrentTarget);

            // If we have no umbral or astral stacks, cast 
            if (ActionResourceManager.BlackMage.AstralStacks <= 1 && ActionResourceManager.BlackMage.UmbralStacks <= 1)
                return await Spells.Blizzard2.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Fire4()
        {
            if (Core.Me.ClassLevel < Spells.Fire4.LevelAcquired)
                return false;

            if (Core.Me.ClassLevel < 60)
                return false;

            if (Core.Me.ClassLevel > 67)
                return false;

            if (Casting.LastSpell == Spells.Fire3)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentMana >= 2400
                && ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds >= 6000)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Fire3()
        {
            if (Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.ClassLevel > 71)
                return false;

            if (Casting.LastSpell == Spells.Thunder2
                || Casting.LastSpell == Spells.Thunder4)
                if (ActionResourceManager.BlackMage.UmbralStacks > 0)
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);
            return false;
        }


        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return MagicDps.ForceLimitBreak(Spells.Skyshard, Spells.Starstorm, Spells.Meteor, Spells.Blizzard);
        }
    }
}


