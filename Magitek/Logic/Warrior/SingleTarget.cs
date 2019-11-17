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
            if (Globals.OnPvpMap)
                return false;

            if (!WarriorSettings.Instance.UseDefiance)
                return false;

            if (!WarriorSettings.Instance.UseTomahawkOnLostAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
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

            if (!Core.Me.HasAura(Auras.StormsEye))
                return false;

            if (Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (!WarriorSettings.Instance.UseUpheaval)
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 20)
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            // If we have Inner Release & CD for it is 25s or less, don't cast upheaval
            if (ActionManager.HasSpell(Spells.InnerRelease.Id) && Spells.InnerRelease.Cooldown.TotalMilliseconds < 25000)
            {
                return false;
            }

            return await Spells.Upheaval.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> InnerBeast()//Becomes Fell Cleave
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (Core.Me.ClassLevel > 54)
                return false;

            if (!Core.Me.HasAura(Auras.StormsEye))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50)
                return false;

            return await Spells.InnerBeast.Cast(Core.Me.CurrentTarget);
        }

        internal static async Task<bool> FellCleave()
        {
            if (Core.Me.CurrentTarget == null)
                return false;

            if (Core.Me.ClassLevel < 54)
                return false;

            if (Core.Me.ClassLevel < 80 && Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (!Core.Me.HasAura(Auras.StormsEye))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 50)
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
                    // If we don't have Storm's Eye aura for at least 10 seconds + Inner Release cooldown time
                    if (!Core.Me.HasAura(Auras.StormsEye, true, 10000 + Spells.InnerRelease.Cooldown.Milliseconds))
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
        
        public static async Task<bool> Onslaught()
        {
            if (!WarriorSettings.Instance.UseOnslaught)
                return false;

            if (Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < WarriorSettings.Instance.KeepAtLeastXBeastGauge + 20)
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (ActionManager.HasSpell(Spells.InnerRelease.Id))
            {
                // If Inner Release as 10 seconds or less left on cooldown
                if (Spells.InnerRelease.Cooldown.Milliseconds <= 10000)
                {//Don't cast Onslaught
                    return false;
                }
            }

            return await Spells.Onslaught.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> InnerReleaseFellCleaveSpam()
        {
            if (!Core.Me.HasAura(Auras.InnerRelease))
                return false;
 
            if (Casting.LastSpell == Spells.FellCleave)
            {   //If Onslaught is allowed
                if (WarriorSettings.Instance.UseOnslaught && await Spells.Onslaught.Cast(Core.Me.CurrentTarget)) return true;
                if (WarriorSettings.Instance.UseUpheaval && await Spells.Upheaval.Cast(Core.Me.CurrentTarget)) return true;
            }

            await Spells.FellCleave.Cast(Core.Me.CurrentTarget);

            // Keep returning true as long as we have Inner Release
            return true;       
        }
    }
}