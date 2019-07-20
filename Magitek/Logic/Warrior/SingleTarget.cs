using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Warrior
{
    internal static class SingleTarget
    {
        public static async Task<bool> Tomahawk()
        {
            if (WarriorSettings.Instance.UseTomahawkToPullExtraEnemies && !BotManager.Current.IsAutonomous)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                {
                    return await Spells.Tomahawk.Cast(pullTarget);
                }
            }

            if (!WarriorSettings.Instance.UseTomahawk)
            {
                if (WarriorSettings.Instance.UseTomahawkToPull && !Core.Me.InCombat)
                {
                    return await Spells.Tomahawk.Cast(Core.Me.CurrentTarget);
                }

                return false;
            }

            return await Spells.Tomahawk.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TomahawkOnLostAggro()
        {
            if (Core.Me.OnPvpMap())
                return false;

            if (!WarriorSettings.Instance.UseTomahawkOnLostAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            if (!DutyManager.InInstance)
                return false;

            var tomahawkTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) > 5 + r.CombatReach && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.Distance(Core.Me) <= 15 + r.CombatReach && r.TargetGameObject != Core.Me);

            if (tomahawkTarget == null)
                return false;

            if (tomahawkTarget.TargetGameObject == null)
                return false;

            if (!await Spells.Tomahawk.Cast(tomahawkTarget))
                return false;

            Logger.Write($@"[Magitek] Tomahawk On {tomahawkTarget.Name} To Pull Aggro");
            return true;
        }

        internal static async Task<bool> Upheaval()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            // If we have Inner Release
            if (ActionManager.HasSpell(Spells.InnerRelease.Id))
            {
                // If Inner Release has less than 22 seconds off cooldown
                if (Spells.InnerRelease.Cooldown.Milliseconds < 22000)
                {
                    // If we're within melee range
                    if (Core.Me.CurrentTarget.Distance(Core.Me.Location) <= 4)
                    {
                        // Cast Onslaught
                        return await Spells.Onslaught.Cast(Core.Me.CurrentTarget);
                    }
                }
            }

            return await Spells.Upheaval.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> InnerBeast()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (!Core.Me.HasAura(Auras.Defiance))
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease))
            {
                if (ActionResourceManager.Warrior.BeastGauge < 50)
                    return false;

                if (Spells.Berserk.Cooldown.TotalSeconds < 5 && Core.Me.CurrentHealthPercent >= 50)
                    return false;
            }

			return await Spells.FellCleave.Cast(Core.Me.CurrentTarget);
		}

        internal static async Task<bool> FellCleave()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (Core.Me.ClassLevel < 54)
                return false;

            if (!Core.Me.HasAura(Auras.Deliverance))
                return false;

            // We don't have Inner Release
            if (!Core.Me.HasAura(Auras.InnerRelease))
            {
                // Our Beast Gauge is 90 or higher
                if (ActionResourceManager.Warrior.BeastGauge >= 90)
                {
                    // Storm's Eye has at least 9 seconds left on it
                    if (Core.Me.HasAura(Auras.StormsEye, true, 9000))
                    {
                        // Use Fell Cleave
                        return await Spells.FellCleave.Cast(Core.Me.CurrentTarget);
                    }
                }

                if (Spells.Infuriate.Cooldown.Milliseconds < 6000)
                {
                    return await Spells.FellCleave.Cast(Core.Me.CurrentTarget);
                }
            }
            

            if (Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < 25)
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge < 50)
                return false;

            return await Spells.FellCleave.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> StormsPath()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.Maim)
                return false;

            return await Spells.StormsPath.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> StormsEye()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.Maim)
                return false;

            // If we have Inner Release
            if (ActionManager.HasSpell(Spells.InnerRelease.Id))
            {
                // If Inner Release as 10 seconds or less left on cooldown
                if (Spells.InnerRelease.Cooldown.Milliseconds <= 10000)
                {
                    // If we don't have Storm's Eye aura for at least 17 seconds + Inner Release cooldown time
                    if (!Core.Me.HasAura(Auras.StormsEye, true, 17000 + Spells.InnerRelease.Cooldown.Milliseconds))
                    {
                        // Use Storm's Eye
                        return await Spells.StormsEye.Cast(Core.Me.CurrentTarget);
                    }
                }
            }

            if (Core.Me.HasAura(Auras.StormsEye, true, 9000))
                return false;

            return await Spells.StormsEye.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> Maim()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (ActionManager.LastSpell != Spells.HeavySwing)
                return false;

            return await Spells.Maim.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> HeavySwing()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            return await Spells.HeavySwing.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> LowBlow()
        {
            if (!WarriorSettings.Instance.UseLowBlow)
                return false;

            var currentTargetAsCharacter = Core.Me.CurrentTarget as Character;

            if (currentTargetAsCharacter == null)
                return false;

            if (!currentTargetAsCharacter.IsCasting)
                return false;

            if (!InterruptsAndStunsManager.AllStuns.Contains(currentTargetAsCharacter.CastingSpellId))
                return false;

            return await Spells.LowBlow.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Interject()
        {
            if (!WarriorSettings.Instance.UseInterject)
                return false;

            var currentTargetAsCharacter = Core.Me.CurrentTarget as Character;

            if (currentTargetAsCharacter == null)
                return false;

            if (!currentTargetAsCharacter.IsCasting)
                return false;

            if (!InterruptsAndStunsManager.AllInterrupts.Contains(currentTargetAsCharacter.CastingSpellId))
                return false;

            return await Spells.Interject.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Onslaught()
        {
            if (!WarriorSettings.Instance.UseOnslaught)
                return false;

            if (!Utilities.Routines.Warrior.OnGcd)
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.UseOnslaughtMinBeastGauge)
                return false;

            return await Spells.Onslaught.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> InnerReleaseFellCleaveSpam()
        {
            if (!Core.Me.HasAura(Auras.InnerRelease))
                return false;
 
            if (Casting.LastSpell == Spells.FellCleave)
            {
                if (await Spells.Onslaught.Cast(Core.Me.CurrentTarget)) return true;
                if (await Spells.Upheaval.Cast(Core.Me.CurrentTarget)) return true;
            }

            await Spells.FellCleave.Cast(Core.Me.CurrentTarget);

            // Keep returning true as long as we have Inner Release
            return true;       
        }
    }
}