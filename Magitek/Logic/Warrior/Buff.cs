﻿using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
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

            //Radius of WAR AoEs is 5y not 3y, adding combat reach should be okay
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < 1)
                return false;

            //Added level check as this skill is available as berserk at lvl 6 and AoE combo isnt until lvl 40
            if (!Core.Me.HasAura(Auras.SurgingTempest, true, 12000)
                && Core.Me.ClassLevel >= Spells.MythrilTempest.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.NascentChaos))
                return false;

            // We're assuming IR is usable from here. If we're on GCD with more than 800 milliseconds left
            if (Spells.HeavySwing.Cooldown.TotalMilliseconds > 800)
                return false;

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

        public static async Task<bool> UsePotion()
        {
            if (Spells.InnerRelease.IsKnown() && !Spells.InnerRelease.IsReady(3000))
                return false;

            return await Tank.UsePotion(WarriorSettings.Instance);
        }
    }
}