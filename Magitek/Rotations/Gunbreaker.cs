using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Gunbreaker;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using GunbreakerRoutine = Magitek.Utilities.Routines.Gunbreaker;
using System.Threading.Tasks;
using Healing = Magitek.Logic.Gunbreaker.Heal;

namespace Magitek.Rotations
{
    public static class Gunbreaker
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {
            await Casting.CheckForSuccessfulCast();
            return await Buff.RoyalGuard();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
                }
            }

            if (GunbreakerSettings.Instance.PullWithLightningShot)
                await Spells.LightningShot.Cast(Core.Me.CurrentTarget);

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            return await Healing.Aurora();
        }

        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);

            //Utility
            if (await Tank.Interrupt(GunbreakerSettings.Instance)) return true;
            if (await Buff.RoyalGuard()) return true;

            if (GunbreakerRoutine.GlobalCooldown.CountOGCDs() < 2 && Spells.KeenEdge.Cooldown.TotalMilliseconds > 650 + BaseSettings.Instance.UserLatencyOffset)
            {
                //Defensive Buff
                if (await Defensive.Superbolide()) return true;
                if (await Healing.Aurora()) return true;
                if (await Defensive.Nebula()) return true;
                if (await Defensive.Rampart()) return true;
                if (await Defensive.Camouflage()) return true;
                if (await Defensive.Reprisal()) return true;
                if (await Defensive.HeartofLight()) return true;
                if (await Defensive.HeartofCorundum()) return true;

                //oGCD to use with BurstStrike
                if (await SingleTarget.Hypervelocity()) return true;

                //oGCD to use inside Combo 2
                if (await SingleTarget.EyeGouge()) return true;
                if (await SingleTarget.AbdomenTear()) return true;
                if (await SingleTarget.JugularRip()) return true;

                //oGCD - Buffs
                if (await Buff.NoMercy()) return true;
                if (await Buff.Bloodfest()) return true;

                //oGCD - Damage
                if (await SingleTarget.RoughDivide()) return true;
                if (await SingleTarget.BlastingZone()) return true;
                if (await Aoe.BowShock()) return true;
            }

            //Pull or get back aggro with LightningShot
            if (await SingleTarget.LightningShot()) return true;

            //AOE
            if (await Aoe.FatedCircle()) return true;
            if (await Aoe.DemonSlaughter()) return true;
            if (await Aoe.DemonSlice()) return true;

            //Combo 2 - 1st step
            if (await SingleTarget.GnashingFang()) return true;

            //Apply DOT / Burst
            if (await Aoe.DoubleDown()) return true;
            if (await SingleTarget.SonicBreak()) return true;

            //Combo 2 - 2nd step
            if (await SingleTarget.SavageClaw()) return true;
            if (await SingleTarget.WickedTalon()) return true;

            //Combo 3
            if (await SingleTarget.BurstStrike()) return true;
            if (await SingleTarget.LightningShot()) return true;

            //Combo 1 Filler
            if (await SingleTarget.SolidBarrel()) return true;
            if (await SingleTarget.BrutalShell()) return true;

            return await SingleTarget.KeenEdge();
        }
        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }
    }
}
