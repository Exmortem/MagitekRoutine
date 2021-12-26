using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;

namespace Magitek.Logic.Warrior
{
    internal static class SingleTarget
    {

        /*************************************************************************************
         *                                    Aggro
         * ***********************************************************************************/
        public static async Task<bool> Tomahawk()
        {
            if (WarriorSettings.Instance.UseTomahawkToPullExtraEnemies && !BotManager.Current.IsAutonomous)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                    return await Spells.Tomahawk.Cast(pullTarget);
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

        /*************************************************************************************
         *                                    Combo
         * ***********************************************************************************/
        public static async Task<bool> StormsPath()
        {
            if (!ActionManager.HasSpell(Spells.StormsPath.Id))
                return false;

            if (!WarriorRoutine.CanContinueComboAfter(Spells.Maim))
                return false;

            return await Spells.StormsPath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> StormsEye()
        {
            if (!ActionManager.HasSpell(Spells.StormsEye.Id))
                return false;

            if (!WarriorRoutine.CanContinueComboAfter(Spells.Maim))
                return false;

            // If Inner Release has less than 8 seconds on cooldown
            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.InnerRelease, 10000))
            {
                int refreshTime = 20000 + (int)Spells.InnerRelease.Cooldown.TotalMilliseconds;
                Aura SurgingTempestAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.SurgingTempest);
                if (Core.Me.HasAura(Auras.SurgingTempest) && SurgingTempestAura.TimespanLeft.TotalMilliseconds > refreshTime)
                    return false;
                else
                    return await Spells.StormsEye.Cast(Core.Me.CurrentTarget);
            }

            //No need to refresh SurgingTempest
            if (Core.Me.HasAura(Auras.SurgingTempest, true, 6500))
                return false;

            return await Spells.StormsEye.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Maim()
        {
            if (!ActionManager.HasSpell(Spells.Maim.Id))
                return false;

            if (!WarriorRoutine.CanContinueComboAfter(Spells.HeavySwing))
                return false;

            return await Spells.Maim.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeavySwing()
        {
            if (!ActionManager.HasSpell(Spells.HeavySwing.Id))
                return false;

            if (Core.Me.HasAura(Auras.InnerRelease))
                return false;

            return await Spells.HeavySwing.Cast(Core.Me.CurrentTarget);
        }


        /*************************************************************************************
         *                                    oGCD
         * ***********************************************************************************/
        public static async Task<bool> Upheaval()
        {
            if (!WarriorRoutine.ToggleAndSpellCheck(WarriorSettings.Instance.UseUpheaval, Spells.Upheaval))
                return false;

            if (!Spells.IsAvailableAndReady(Spells.Upheaval))
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            if (WarriorSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= WarriorSettings.Instance.OrogenyMinimumEnemies)
                return false;

            return await Spells.Upheaval.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Onslaught()
        {
            if (!WarriorRoutine.ToggleAndSpellCheck(WarriorSettings.Instance.UseOnslaught, Spells.Onslaught))
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 3 + r.CombatReach) > 1)
                return false;

            if (Weaving.GetCurrentWeavingCounter() > 0)
                return false;

            return await Spells.Onslaught.Cast(Core.Me.CurrentTarget);
        }

        /*************************************************************************************
         *                                Special GCD
         * ***********************************************************************************/
        public static async Task<bool> FellCleave()
        {
            if (!ActionManager.HasSpell(Spells.FellCleave.Id))
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            if (!Core.Me.HasAura(Auras.InnerRelease) && ActionResourceManager.Warrior.BeastGauge <= WarriorSettings.Instance.KeepAtLeastXBeastGauge)
                return false;

            return await WarriorRoutine.FellCleave.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> InnerChaos()
        {
            if (!WarriorRoutine.ToggleAndSpellCheck(WarriorSettings.Instance.UseInnerChaos, Spells.InnerChaos))
                return false;

            if (!Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest))
                return false;

            return await Spells.InnerChaos.Cast(Core.Me.CurrentTarget);
        }
    }
}