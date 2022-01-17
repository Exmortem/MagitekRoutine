using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;
using Auras = Magitek.Utilities.Auras;
using Magitek.Utilities.Managers;
using Magitek.Models.Account;

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

        //Berserk Becomes Inner Release
        public static async Task<bool> InnerRelease()
        {
            if (!WarriorSettings.Instance.UseInnerRelease)
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
            return await WarriorRoutine.InnerRelease.Cast(Core.Me);
        }

        public static async Task<bool> Infuriate()
        {
            if (!WarriorSettings.Instance.UseInfuriate)
                return false;

            if (Casting.LastSpell == Spells.InnerRelease)
                return false;

            if (Core.Me.HasAura(Auras.InnerRelease))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge >= WarriorSettings.Instance.UseInfuriateAtBeastGauge)
                return false;

            return await Spells.Infuriate.Cast(Core.Me);
        }

        public static async Task<bool> NascentFlash()
        {
            if (!WarriorSettings.Instance.UseNascentFlash)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Spells.NascentFlash.IsReady())
                return false;


            var canNascentTargets = Group.CastableAlliesWithin30.Where(CanNascentFlash);

            if (!BaseSettings.Instance.UseWeightedHealingPriority)
                canNascentTargets = canNascentTargets.OrderByDescending(DispelManager.GetWeight).ThenBy(c => c.CurrentHealthPercent);

            var nascentTarget = canNascentTargets.FirstOrDefault();

            if (nascentTarget == null)
                return false;

            return await Spells.NascentFlash.Cast(nascentTarget);

            bool CanNascentFlash(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.IsMe)
                    return false;

                if (unit.HasAura(Auras.NascentGlint))
                    return false;

                if (unit.CurrentHealthPercent > WarriorSettings.Instance.NascentFlashHealthPercent)
                    return false;

                if (WarriorSettings.Instance.NascentFlashTank && unit.IsTank())
                    return true;

                if (WarriorSettings.Instance.NascentFlashHealer && unit.IsHealer())
                    return true;

                if (WarriorSettings.Instance.NascentFlashDps && unit.IsDps())
                    return true;

                return false;
            }
        }
    }
}