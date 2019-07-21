using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;

namespace Magitek.Logic.Warrior
{
    internal static class Buff
    {
        internal static async Task<bool> Stance()
        {
            if (WarriorSettings.Instance.IsMainTank && !Core.Me.HasAura(Auras.Defiance))
                return await Spells.Defiance.Cast(Core.Me);

            if (!WarriorSettings.Instance.IsMainTank && !Core.Me.HasAura(Auras.Defiance))
                return true;

            return false;
        }

        internal static async Task<bool> InnerReleaseMainTank()
        {
            if (!WarriorSettings.Instance.UseInnerReleaseDefiance)
                return false;

            if (!ActionManager.HasSpell(Spells.InnerRelease.Id))
                return false;

            if (!Core.Me.HasAura(Auras.Defiance))
                return false;

            if (ActionResourceManager.Warrior.BeastGauge < 50)
                return false;

            if (Combat.CombatTotalTimeLeft < 20)
                return false;

            return await Spells.InnerRelease.Cast(Core.Me);
        }

        internal static async Task<bool> InnerReleaseOffTank()
        {
            if (!ActionManager.HasSpell(Spells.InnerRelease.Id))
                return false;

            if (!Core.Me.HasAura(Auras.Defiance))
                return true;

            if (!Core.Me.HasAura(Auras.StormsEye, true, 17000))
                return false;

            if (Spells.InnerRelease.Cooldown != TimeSpan.Zero)
                return false;

            // We're assuming IR is usable from here

            // If we're on GCD with more than 825 milliseconds left
            if (Spells.HeavySwing.Cooldown.Milliseconds > 825)
            {
                // Wait until the GCD has 825 or less remaining
                await Coroutine.Wait(3000, () => Spells.HeavySwing.Cooldown.Milliseconds <= 825);
            }
  
            return await Spells.InnerRelease.Cast(Core.Me);
        }

        internal static async Task<bool> Beserk()
        {
            // Use on CD if below than max level
            if (Core.Me.ClassLevel < 70)
                return await Spells.Berserk.Cast(Core.Me);

            if (!WarriorSettings.Instance.IsMainTank && Spells.InnerRelease.Cooldown.TotalSeconds <= 63)
                return false;

            return await Spells.Berserk.Cast(Core.Me);
        }

        internal static async Task<bool> Infuriate()
        {
            if (ActionResourceManager.Warrior.BeastGauge > WarriorSettings.Instance.UIseInfurateAtBeastGauge)
                return false;

            return await Spells.Infuriate.Cast(Core.Me);
        }
    }
}