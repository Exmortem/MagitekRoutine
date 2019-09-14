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
            if (!WarriorSettings.Instance.UseEquilibrium)
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.EquilibriumHealthPercent)
                return false;

            return await Spells.Equilibrium.Cast(Core.Me);
        }
        //Berserk Becomes Inner Release
        internal static async Task<bool> InnerRelease()
        {
            if (!WarriorSettings.Instance.UseInnerRelease)
                return false;

            if (!ActionManager.HasSpell(Spells.InnerRelease.Id))
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 3 + r.CombatReach) < 1)
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            if (Combat.CombatTotalTimeLeft < 12)
                return false;
            //Only use Inner Release after we have Storm's Eye
            if (!Core.Me.HasAura(Auras.StormsEye, true, 12000))
                return false;
            // We're assuming IR is usable from here
            // If we're on GCD with more than 800 milliseconds left
            if (Spells.HeavySwing.Cooldown.TotalMilliseconds > 800)
            {
                // Wait until the GCD has 800 or less remaining
                await Coroutine.Wait(3000, () => Spells.HeavySwing.Cooldown.TotalMilliseconds <= 800);
            }

            return await Spells.InnerRelease.Cast(Core.Me);
        }
        internal static async Task<bool> Beserk()
        {
            if (!WarriorSettings.Instance.UseInnerRelease)
                return false;

            if (ActionManager.HasSpell(Spells.InnerRelease.Id))
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Spells.Berserk.Cooldown != TimeSpan.Zero)
                return false;

            return await Spells.Berserk.Cast(Core.Me);
        }

        internal static async Task<bool> Infuriate()
        {
            if (!WarriorSettings.Instance.UseInfuriate)
                return false;

            if (ActionResourceManager.Warrior.BeastGauge > WarriorSettings.Instance.UseInfuriateAtBeastGauge)
                return false;
            //Save at least 1 Infuriate for when you want Inner Chaos  / Chaos Cyclone (I will add in a buff check for this later.)
            if (Core.Me.ClassLevel >= 72 && Spells.Infuriate.Cooldown > TimeSpan.Zero)
                return false;
            if (Casting.LastSpell == Spells.InnerRelease)
                return false;
            //If we are in Inner Release and lv 72+, don't use Infuriate
            if (Core.Me.ClassLevel > 72 && Core.Me.HasAura(Auras.InnerRelease))
                return false;
            //If we are lv 72+ and Inner Release comes off CD in 10 or less seconds don't use Infuriate
            if (Core.Me.ClassLevel > 72 && Spells.InnerRelease.Cooldown.Seconds < 10)
                return false;
            //Buff Check Logic here

            return await Spells.Infuriate.Cast(Core.Me);
        }
    }
}