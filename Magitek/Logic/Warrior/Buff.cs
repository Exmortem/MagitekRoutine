using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;

namespace Magitek.Logic.Warrior
{
    internal static class Buff
    {
        public static async Task<bool> Defiance()
        {
            if (!WarriorSettings.Instance.UseDefiance)
            {
                if (Core.Me.HasAura(Auras.Defiance))
                {
                    return await Spells.Defiance.Cast(Core.Me);
                }

                return false;
            }

            if (Core.Me.HasAura(Auras.Defiance))
                return false;

            return await Spells.Defiance.Cast(Core.Me);
        }
        internal static async Task<bool> Equilibrium()
        {
            if (!WarriorRoutine.ToggleAndSpellCheck(WarriorSettings.Instance.UseEquilibrium, Spells.Equilibrium))
                return false; 

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.EquilibriumHealthPercent)
                return false;

            return await Spells.Equilibrium.Cast(Core.Me);
        }
        //Berserk Becomes Inner Release
        internal static async Task<bool> InnerRelease()
        {
            if (!WarriorRoutine.ToggleAndSpellCheck(WarriorSettings.Instance.UseInnerRelease, WarriorRoutine.InnerRelease))
                return false; 

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 3 + r.CombatReach) < 1)
                return false;

            if (!Core.Me.HasAura(Auras.SurgingTempest, true, 12000))
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            // We're assuming IR is usable from here. If we're on GCD with more than 800 milliseconds left
            if (Spells.HeavySwing.Cooldown.TotalMilliseconds > 800)
            {
                // Wait until the GCD has 800 or less remaining
                await Coroutine.Wait(3000, () => Spells.HeavySwing.Cooldown.TotalMilliseconds <= 800);
            }

            //Logger.WriteInfo($@"InnerRelease Ready");
            return await Utilities.Routines.Warrior.InnerRelease.Cast(Core.Me);
        }

        internal static async Task<bool> Infuriate()
        {
            if (!WarriorRoutine.ToggleAndSpellCheck(WarriorSettings.Instance.UseInfuriate, Spells.Infuriate))
                return false;

            if (Casting.LastSpell == Spells.InnerRelease)
                return false;

            if (Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge >= WarriorSettings.Instance.UseInfuriateAtBeastGauge)
                return false;

            Logger.WriteInfo($@"Infuriate Ready");
            return await Spells.Infuriate.Cast(Core.Me);
        }
    }
}