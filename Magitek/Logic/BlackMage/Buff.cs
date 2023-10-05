using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using System;
using System.Threading.Tasks;

namespace Magitek.Logic.BlackMage
{
    internal static class Buff
    {
        public static async Task<bool> Triplecast()
        {
            if (Core.Me.ClassLevel < Spells.Triplecast.LevelAcquired)
                return false;

            if (!BlackMageSettings.Instance.TripleCast)
                return false;

            // Don't dot if time in combat less than 30 seconds
            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            // Add check for charges with new update
            if (Spells.Triplecast.Cooldown != TimeSpan.Zero
                && Spells.Triplecast.Charges == 0)
                return false;

            // Check to see if triplecast is already up
            if (Core.Me.HasAura(Auras.Triplecast))
                return false;

            // Do not use in Umbral
            if (ActionResourceManager.BlackMage.UmbralStacks > 0)
                return false;

            // Why cast after bliz 3? Should only be used in AF
            if (ActionResourceManager.BlackMage.UmbralHearts == 3 && Casting.LastSpell == Spells.Fire3)
                return await Spells.Triplecast.Cast(Core.Me);

            // Add new condition for AoE rotation
            if (ActionResourceManager.BlackMage.UmbralHearts == 3 && Casting.LastSpell == Spells.HighFireII)
                return await Spells.Triplecast.Cast(Core.Me);

            return false;
        }
        public static async Task<bool> Sharpcast()
        {
            if (Core.Me.ClassLevel < Spells.Sharpcast.LevelAcquired)
                return false;

            // Don't use if time in combat less than 30 seconds
            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (Spells.Sharpcast.Cooldown != TimeSpan.Zero
                //Sharpcast now has charges after level 88
                && Spells.Sharpcast.Charges == 0)
                return false;

            if (!BlackMageSettings.Instance.Sharpcast)
                return false;

            if (Core.Me.HasAura(Auras.Sharpcast))
                return false;

            //Check to see if we already have both buffs
            if (Core.Me.HasAura(Auras.ThunderCloud)
                && Core.Me.HasAura(Auras.FireStarter))
                return false;

            //Let's start planning our uses of Sharpcast better
            if (Casting.LastSpell == Spells.Paradox
                && ActionResourceManager.BlackMage.UmbralStacks > 0)
                return await Spells.Sharpcast.Cast(Core.Me);
                        
            if (!ActionResourceManager.BlackMage.Paradox
                && Casting.LastSpell == Spells.Blizzard4)
                return await Spells.Sharpcast.Cast(Core.Me);

            if (Casting.LastSpell == Spells.Blizzard2
                || Casting.LastSpell == Spells.HighBlizzardII
                || Casting.LastSpell == Spells.Freeze)
                return await Spells.Sharpcast.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> LeyLines()
        {
            if (Core.Me.ClassLevel < Spells.LeyLines.LevelAcquired)
                return false;

            if (Spells.LeyLines.Cooldown != TimeSpan.Zero)
                return false;

            if (!BlackMageSettings.Instance.LeyLines)
                return false;

            if (BlackMageSettings.Instance.LeyLinesBossOnly
                && !Core.Me.CurrentTarget.IsBoss())
                return false;

            // Don't use if time in combat less than 30 seconds
            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            //Don't use while moving
            if (MovementManager.IsMoving)
                return false;

            // Do not Ley Lines if we don't have 3 astral stacks
            if (ActionResourceManager.BlackMage.AstralStacks != 3
                || ActionResourceManager.BlackMage.UmbralStacks > 0)
                return false;

            // Do not Ley Lines if we don't have any umbral hearts (roundabout check to see if we're at the begining of astral)
            if (Casting.LastSpell == Spells.Fire3
                && ActionResourceManager.BlackMage.UmbralHearts == 3
                || Core.Me.HasAura(Auras.Triplecast))
                // Fire 3 is always used at the start of Astral
                return await Spells.LeyLines.Cast(Core.Me);
            // If we used something that opens the GCD
            // Fire3 caused this to go off at the beginning of astral anyway
            //if (Casting.LastSpell == Spells.Blizzard3)// Thunder3 only opens up the GCD if it is using Thundercloud || Casting.LastSpell == Spells.Thunder3 || Core.Me.HasAura(Auras.Triplecast) || Casting.LastSpell == Spells.Xenoglossy)
            //return await Spells.LeyLines.Cast(Core.Me);

            //Use in AoE rotation as well
            if (Casting.LastSpell == Spells.HighFireII
                && ActionResourceManager.BlackMage.UmbralHearts == 3
                || Core.Me.HasAura(Auras.Triplecast))
                // High Fire II is always used at the start of Astral
                return await Spells.LeyLines.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> UmbralSoul()
        {
            if (Core.Me.ClassLevel < Spells.UmbralSoul.LevelAcquired)
                return false;

            if (Spells.UmbralSoul.Cooldown != TimeSpan.Zero)
                return false;

            // Do not Umbral Soul unless we have 1 umbral stack
            if (ActionResourceManager.BlackMage.UmbralStacks == 0)
                return false;

            if (Core.Me.CurrentTarget != null)
                return false;

            if (!Core.Me.InCombat
                && ActionResourceManager.BlackMage.UmbralStacks > 0)
                return await Spells.UmbralSoul.Cast(Core.Me);

            return false;

        }

        public static async Task<bool> ManaFont()
        {
            if (Core.Me.ClassLevel < Spells.ManaFont.LevelAcquired)
                return false;

            if (Spells.ManaFont.Cooldown != TimeSpan.Zero)
                return false;
            
            //Moved this up as it should go off regardless of toggle
            if (Casting.LastSpell == Spells.Flare
                && Spells.Fire.Cooldown.TotalMilliseconds > Globals.AnimationLockMs
                && Core.Me.CurrentMana == 0)
                return await Spells.ManaFont.Cast(Core.Me);
            
            if (!BlackMageSettings.Instance.ConvertAfterFire3)
                return false;

            // Don't use if time in combat less than 30 seconds
            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (Core.Me.CurrentMana >= 7000)
                return false;

            if (Casting.LastSpell == Spells.Despair)
                //&& Spells.Fire.Cooldown.TotalMilliseconds > Globals.AnimationLockMs)
                return await Spells.ManaFont.Cast(Core.Me);

            if (Casting.LastSpell == Spells.Fire3
                //&& Spells.Fire.Cooldown.TotalMilliseconds > Globals.AnimationLockMs
                && BlackMageSettings.Instance.ConvertAfterFire3
                && Core.Me.CurrentMana < 7000)
                return await Spells.ManaFont.Cast(Core.Me);

            return false;
        }
        public static async Task<bool> Transpose()
        {
            if (Core.Me.ClassLevel < Spells.Transpose.LevelAcquired)
                return false;

            if (Spells.Transpose.Cooldown != TimeSpan.Zero)
                return false;
            
            if (Core.Me.ClassLevel < 40
                && Core.Me.CurrentMana < 1600
                && ActionResourceManager.BlackMage.AstralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            if (Core.Me.ClassLevel < 40
                && Core.Me.CurrentMana == 10000
                && ActionResourceManager.BlackMage.UmbralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            if (!Core.Me.InCombat
                && ActionResourceManager.BlackMage.AstralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            return false;
        }
        public static async Task<bool> Amplifier()
        {
            if (Core.Me.ClassLevel < Spells.Amplifier.LevelAcquired)
                return false;

            if (Spells.Amplifier.Cooldown != TimeSpan.Zero)
                return false;

            if (ActionResourceManager.BlackMage.PolyglotCount > 0)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (ActionResourceManager.BlackMage.AstralStacks == 0
                && ActionResourceManager.BlackMage.UmbralStacks == 0)
                return false;

            return await Spells.Amplifier.Cast(Core.Me);
        }

    }

}
