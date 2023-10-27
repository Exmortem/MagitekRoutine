using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.BlackMage;

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
            if (UmbralStacks > 0)
                return false;

            // Why cast after bliz 3? Should only be used in AF
            if (UmbralHearts == 3)
            {
                if (Casting.LastSpell == Spells.Fire3
                    || Casting.LastSpell == Spells.HighFireII)
                    return await Spells.Triplecast.Cast(Core.Me);

                return false;
            }

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
                && UmbralStacks > 0)
                return await Spells.Sharpcast.Cast(Core.Me);
                        
            if (!Paradox
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
            if (AstralStacks != 3
                || UmbralStacks > 0)
                return false;

            // Do not Ley Lines if we don't have any umbral hearts (roundabout check to see if we're at the begining of astral)
            if (Casting.LastSpell == Spells.Fire3
                && (UmbralHearts == 3
                || Core.Me.HasAura(Auras.Triplecast)))
                // Fire 3 is always used at the start of Astral
                return await Spells.LeyLines.Cast(Core.Me);
            // If we used something that opens the GCD
            // Fire3 caused this to go off at the beginning of astral anyway
            //if (Casting.LastSpell == Spells.Blizzard3)// Thunder3 only opens up the GCD if it is using Thundercloud || Casting.LastSpell == Spells.Thunder3 || Core.Me.HasAura(Auras.Triplecast) || Casting.LastSpell == Spells.Xenoglossy)
            //return await Spells.LeyLines.Cast(Core.Me);

            //Use in AoE rotation as well
            if (Casting.LastSpell == Spells.HighFireII
                && (UmbralHearts == 3
                || Core.Me.HasAura(Auras.Triplecast)))
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
            if (UmbralStacks == 0)
                return false;

            if (Core.Me.CurrentTarget != null)
                return false;

            if (!Core.Me.InCombat
                && UmbralStacks > 0)
                return await Spells.UmbralSoul.Cast(Core.Me);

            if (Core.Me.InCombat
                && UmbralStacks > 0
                && StackTimer.TotalMilliseconds <= 2000)
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
            //Swapped mana check to be first as this was going off before we had 0 mana
            if (Core.Me.CurrentMana == 0
                && (Casting.LastSpell == Spells.Flare
                || Casting.LastSpell == Spells.Foul))
                //&& Spells.Fire.Cooldown.TotalMilliseconds > Globals.AnimationLockMs                
            {
                Logger.WriteInfo($@"[Debug] If we get to this point we should have cast flare and have 0 mana - actual last spell is {Casting.LastSpell} and we have {Core.Me.CurrentMana} mana.");

                return await Spells.ManaFont.Cast(Core.Me);

            }

            if (!BlackMageSettings.Instance.ConvertAfterFire3)
                return false;

            // Don't use if time in combat less than 30 seconds
            if (Combat.CombatTotalTimeLeft <= 30)
                return false;

            if (Core.Me.CurrentMana >= 7000)
                return false;

            Logger.WriteInfo($@"[Debug] If we get to this point we should have less than 7000 mana - actual current mana is {Core.Me.CurrentMana}.");

            if (Core.Me.CurrentMana == 0
                && (Casting.LastSpell == Spells.Despair
                || Casting.LastSpell == Spells.Xenoglossy))
            {
                Logger.WriteInfo($@"[Debug] If we get to this point we should have cast xeno or despair - actual last spell is {Casting.LastSpell}.");

                return await Spells.ManaFont.Cast(Core.Me);
            }   
            if (Casting.LastSpell == Spells.Fire3
                //&& Spells.Fire.Cooldown.TotalMilliseconds > Globals.AnimationLockMs
                && BlackMageSettings.Instance.ConvertAfterFire3
                && Core.Me.CurrentMana < 7000)
            {
                Logger.WriteInfo($@"[Debug] If we get to this point we should have cast fire III - actual last spell is {Casting.LastSpell}.");

                return await Spells.ManaFont.Cast(Core.Me);

            }

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
                && AstralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            if (Core.Me.ClassLevel < 40
                && Core.Me.CurrentMana == 10000
                && UmbralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            if (!Core.Me.InCombat
                && AstralStacks > 0)
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

            if (AstralStacks == 0
                && UmbralStacks == 0)
                return false;

            return await Spells.Amplifier.Cast(Core.Me);
        }
        public static async Task<bool> UseEther()
        {
            if (!BlackMageSettings.Instance.QuadFlare)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.ManaFont.Cooldown == TimeSpan.Zero)
                return false;

            if (Core.Me.CurrentMana != 0)
                return false;

            if (Casting.LastSpell != Spells.Flare)
                return false;

            var etherItem = InventoryManager.FilledSlots.FirstOrDefault(s => s.RawItemId == Ether 
            || s.RawItemId == HiEther 
            || s.RawItemId == XEther 
            || s.RawItemId == MegaEther 
            || s.RawItemId == SuperEther);

            while (etherItem.CanUse())
            {
                Logger.WriteInfo($@"Use ether : {etherItem.Name}");
                etherItem.UseItem(Core.Me);
                await Coroutine.Wait(1500, () => false);

                if (etherItem == null || !etherItem.CanUse())
                    return true;
            }
            return false;
        }
        public static readonly uint Ether = 4555;
        public static readonly uint HiEther = 4556;
        public static readonly uint XEther = 4558;
        public static readonly uint MegaEther = 13638;
        public static readonly uint SuperEther = 23168;

    }

}
