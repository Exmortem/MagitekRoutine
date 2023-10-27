using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using static ff14bot.Managers.ActionResourceManager.BlackMage;
using static Magitek.Utilities.MagitekActionResourceManager.BlackMage;

namespace Magitek.Logic.BlackMage
{
    internal static class SingleTarget
    {
        public static async Task<bool> Xenoglossy()
        {
            if (Core.Me.ClassLevel < Spells.Xenoglossy.LevelAcquired)
                return false;

            if (!PolyglotStatus)
                return false;

            // If we need to refresh stack timer, stop
            if (StackTimer.TotalMilliseconds <= 5000)
                return false;

            if (Casting.LastSpell == Spells.Xenoglossy)
                return false;
            
            //If we don't have Xeno, Foul is single target
            if (Core.Me.ClassLevel < 80
                && UmbralStacks == 3)
                return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            //If at 2 stacks of polyglot, cast
            if (PolyglotCount == 2
                && Casting.LastSpell == Spells.Despair)
                return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);

            //If at 2 stacks of polyglot and 5 seconds from another stack, cast
            if (PolyglotCount == 2
                && PolyGlotTimer <= 5000)
                return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);

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
            if (UmbralStacks == 3 && Casting.LastSpell != Spells.Thunder3)
            {
                //We don't have max mana
                if (Core.Me.CurrentMana < 10000 && Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, 5000))
                    return await Spells.Xenoglossy.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> Despair()
        {
            if (Core.Me.ClassLevel < Spells.Despair.LevelAcquired)
                return false;

            if (Casting.LastSpell == Spells.Despair)
                return false;

            // If we're not in astral, stop
            if (AstralStacks != 3)
                return false;

            // If our mana is lower than 2400
            if (Core.Me.CurrentMana < 2400 && Core.Me.CurrentMana != 0)
                return await Spells.Despair.Cast(Core.Me.CurrentTarget);
            
            return false;
        }

        public static async Task<bool> Fire()
        {
            //Low level logic
            if (Core.Me.ClassLevel < 35)
            {
                if (Casting.LastSpell == Spells.Transpose
                    && Core.Me.CurrentMana <= 9600)
                    return false;

                if (Core.Me.CurrentMana < 1600)
                {
                    if (Spells.Transpose.IsKnownAndReady())
                        return await Spells.Transpose.Cast(Core.Me);
                    else return false;

                }

                if (AstralStacks >= 0 && UmbralStacks == 0)
                    return await Spells.Fire.Cast(Core.Me.CurrentTarget);

                return false;
            }

            if (Core.Me.CurrentMana < 1600)
                return false;
            //Low level logic w/firestarter
            if (Core.Me.ClassLevel < 60
                && Core.Me.CurrentMana > 800
                && !Core.Me.HasAura(Auras.FireStarter)
                && (UmbralStacks == 0 || Core.Me.CurrentMana == 10000))
                return await Spells.Fire.Cast(Core.Me.CurrentTarget);

            //only use in astral fire
            if (AstralStacks != 3)
                return false;
            //refresh astral fire at 5s
            if (StackTimer.TotalMilliseconds < 5000)
                return await Spells.Fire.Cast(Core.Me.CurrentTarget);
            //If we don't have despair, use fire 1 to dump mana
            if (Core.Me.ClassLevel < 71 && Core.Me.CurrentMana < 2400)
                return await Spells.Fire.Cast(Core.Me.CurrentTarget);
            //Use Fire early to force sharpcast if about to run out 
            if (Core.Me.HasAura(Auras.Sharpcast) && UmbralHearts == 0)
                if (!Core.Me.HasAura(Auras.Sharpcast, true, 5500))
                    return await Spells.Fire.Cast(Core.Me.CurrentTarget);

            return false;

        }

        public static async Task<bool> Fire4()
        {
            if (Core.Me.ClassLevel < Spells.Fire4.LevelAcquired)
                return false;

            // If we need to refresh stack timer, stop
            if (StackTimer.TotalMilliseconds <= 5000)
                return false;

            // If we have 3 astral stacks and our mana is equal to or greater than 2400
            if (AstralStacks == 3 && Core.Me.CurrentMana >= 2400)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            //if we cast Manafont, make sure it doesn't skip ahead before we actually get mp
            if (Casting.LastSpell == Spells.ManaFont
                && AstralStacks > 0
                && Core.Me.CurrentTarget.EnemiesNearby(10).Count() < BlackMageSettings.Instance.AoeEnemies)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Fire3()
        {
            if (Core.Me.ClassLevel < Spells.Fire3.LevelAcquired)
                return false;

            //Level 35-59 logic
            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel <= 59)
            {
                if (UmbralStacks == 3
                    && Core.Me.CurrentMana == 10000)
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

                if (UmbralStacks == 0
                    && AstralStacks == 0)
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

                if (Core.Me.HasAura(Auras.FireStarter))
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

                return false;
            }

            // Use if we're in Umbral and we have 3 hearts and have max mp
            if (UmbralHearts == 3 && UmbralStacks == 3 && Core.Me.CurrentMana == 10000)
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

            // If we're moving in combat in Astral Fire
            if (MovementManager.IsMoving && AstralStacks == 3)
            {
                // If we have the proc, cast
                if (Core.Me.HasAura(Auras.FireStarter))
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);
            }

            // Use if we're in Astral and we have Firestarter, but Firestarter has 3 seconds or less left
            if (AstralStacks > 0 && Core.Me.HasAura(Auras.FireStarter) && !Core.Me.HasAura(Auras.FireStarter, true, 3000))
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);

            //Use if we're at the end of Astral phase and we have a Fire3 proc
            //We actually want to save this for a quick UI > AF transition
            /*if (AstralStacks > 0 && Core.Me.HasAura(Auras.FireStarter) && Core.Me.CurrentMana < 2400)
                return await Spells.Fire3.Cast(Core.Me.CurrentTarget);
            */
            return false;
        }

        public static async Task<bool> Thunder3()
        {
            if (!BlackMageSettings.Instance.ThunderSingle)
                return false;

            if (Combat.CombatTotalTimeLeft <= BlackMageSettings.Instance.ThunderTimeTillDeathSeconds)
                return false;

            // Try to keep from double-casting thunder
            if (Casting.LastSpell == Spells.Thunder
                || Casting.LastSpell == Spells.Thunder3)
                return false;

            // If we have thunder cloud, but we don't have at least 5 seconds of it left, use the proc
            if (Core.Me.HasAura(Auras.ThunderCloud) && !Core.Me.HasAura(Auras.ThunderCloud, true, 5500))
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);
            
            // If we need to refresh stack timer, stop
            if (StackTimer.TotalMilliseconds <= 5500)
                return false;

            //Low level logic
            if (Core.Me.ClassLevel < 35)
            {
                if (Casting.LastSpell != Spells.Thunder
                        && Casting.LastSpell == Spells.Transpose
                        && UmbralStacks > 0
                        && !Core.Me.CurrentTarget.HasAura(Auras.Thunder, true, BlackMageSettings.Instance.ThunderRefreshSecondsLeft * 1000 + 500))
                    return await Spells.Thunder.Cast(Core.Me.CurrentTarget);

                if (Core.Me.HasAura(Auras.ThunderCloud))
                    return await Spells.Thunder.Cast(Core.Me.CurrentTarget);

                return false;
            }

            //Level 35-59 logic
            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel <= 59)
            {
                if (Casting.LastSpell != Spells.Thunder3
                        && Casting.LastSpell == Spells.Blizzard3
                        && UmbralStacks == 3)
                    return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);
                
                if (Core.Me.HasAura(Auras.ThunderCloud))
                    return await Spells.Thunder.Cast(Core.Me.CurrentTarget);

                return false;
            }

            // If the last spell we cast is triple cast, stop
            if (Casting.LastSpell == Spells.Triplecast)
                return false;

            // If we have the triplecast aura, stop
            if (Core.Me.HasAura(Auras.Triplecast))
                return false;
            
            //Moved this up to see if it stops the doublecast
            if (Casting.LastSpell == Spells.Thunder3)
                return false;

            //Let's not wait for sharpcast to almost run out before trying to cast. Just use it right away in UI
            if (Core.Me.HasAura(Auras.Sharpcast)// && !Core.Me.HasAura(Auras.Sharpcast, true, 5000))
                && UmbralStacks > 0)
            {
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);
            }

            // Refresh thunder if it's about to run out
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, BlackMageSettings.Instance.ThunderRefreshSecondsLeft * 1000 + 500)
                && Casting.LastSpell != Spells.Thunder3)
                return await Spells.Thunder3.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Blizzard4()
        {
            if (Casting.LastSpell == Spells.Blizzard4)
                return false;

            if (Core.Me.ClassLevel < Spells.Blizzard4.LevelAcquired)
                return false;

            // If we need to refresh stack timer, stop
            if (StackTimer.TotalMilliseconds <= 5000)
                return false;

            // While in Umbral 
            if (UmbralStacks > 0)
            {
                // If we have less than 3 hearts, cast
                if (UmbralHearts < 3)
                    return await Spells.Blizzard4.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> Blizzard3()
        {

            if (Casting.LastSpell == Spells.Blizzard3
                || Casting.LastSpell == Spells.ManaFont)
                return false;

            if (Core.Me.ClassLevel < Spells.Blizzard3.LevelAcquired)
                return false;

            if (BlackMageSettings.Instance.UseAoe
                && Core.Me.CurrentTarget.EnemiesNearby(10).Count() >= BlackMageSettings.Instance.AoeEnemies)
                return false;
            
            //35-59 logic
            if (Core.Me.ClassLevel >= 35
               && Core.Me.ClassLevel <= 59)
            {
                if (AstralStacks > 0 && Core.Me.CurrentMana < 1600)
                    return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);
            }
                // If we have no umbral or astral stacks, cast 
                if (AstralStacks <= 0 && UmbralStacks == 0)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);
            
            //Post 72 logic
            if (Core.Me.ClassLevel > 71)
            {
                // If our mana is 0 then we have completed rotation with despair
                if (AstralStacks > 0 && Core.Me.CurrentMana == 0)
                    return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);
            }

            // If our mana is below 1600 in AF then we have no more mana for fire spells
            if (AstralStacks > 0 && Core.Me.CurrentMana < 1600)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);

            if (AstralStacks <= 1 && UmbralStacks <= 1)
                return await Spells.Blizzard3.Cast(Core.Me.CurrentTarget);           

            return false;
        }
        public static async Task<bool> Blizzard()
        {
            //stop being level 1, fool
            if (Core.Me.ClassLevel < 2)
                return await Spells.Blizzard.Cast(Core.Me.CurrentTarget);

            //Low level logic
            if (Core.Me.ClassLevel < 35)
            {
                if (Casting.LastSpell == Spells.Transpose && AstralStacks > 0)
                    return false;
                
                if (Core.Me.CurrentMana >= 9600 && UmbralStacks > 0)
                    return await Spells.Transpose.Cast(Core.Me);

                if (Core.Me.CurrentMana < 1600 || (AstralStacks == 0 && UmbralStacks >= 0))
                    return await Spells.Blizzard.Cast(Core.Me.CurrentTarget);

                if (Core.Me.CurrentMana < 9600 && AstralStacks == 0 && UmbralStacks > 0)
                    return await Spells.Blizzard.Cast(Core.Me.CurrentTarget);

                return false;
            }
             
            return false;
        }
        public static async Task<bool> ParadoxUmbral()
        {
            if (Core.Me.ClassLevel < Spells.Paradox.LevelAcquired)
                return false;

            if (PolyglotStatus)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, 4500))
                return false;

            //if (!Spells.Paradox.IsReady())
            //    return false;
            
            //Better check would be to look for the paradox marker
            if (!Paradox)
                return false;

            //Should be cast after bliz 4 to ensure we can get sharpcast off
            if (UmbralStacks > 0
                && Casting.LastSpell == Spells.Blizzard4)
                return await Spells.Paradox.Cast(Core.Me.CurrentTarget);

            return false;
        }

    }
}
