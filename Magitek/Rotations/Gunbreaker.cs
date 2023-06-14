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

            if (WorldManager.InSanctuary)
                return false;

            return await Buff.RoyalGuard();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            /*if (GunbreakerSettings.Instance.LightningShotToPullAggro)
                await Spells.LightningShot.Cast(Core.Me.CurrentTarget);*/

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();
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
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener()) 
                return true;

            //LimitBreak
            if (Defensive.ForceLimitBreak()) return true;

            //Utility
            if (await Buff.RoyalGuard()) return true;
            if (await Tank.Interrupt(GunbreakerSettings.Instance)) return true;

            if (GunbreakerRoutine.GlobalCooldown.CanWeave())
            {
                //Potion
                if (await Buff.UsePotion()) return true;

                //Defensive Buff
                if (await Defensive.Superbolide()) return true;
                if (await Healing.Aurora()) return true;
                if (await Defensive.Nebula()) return true;
                if (await Defensive.Rampart()) return true;
                if (await Defensive.Camouflage()) return true;
                if (await Defensive.Reprisal()) return true;
                if (await Defensive.HeartofLight()) return true;
                if (await Defensive.HeartofCorundum()) return true;
            }

            //oGCD to use with BurstStrike
            if (await SingleTarget.Hypervelocity()) return true;

            //oGCD to use inside Combo 2
            if (await SingleTarget.EyeGouge()) return true;
            if (await SingleTarget.AbdomenTear()) return true;
            if (await SingleTarget.JugularRip()) return true;

            if (GunbreakerRoutine.GlobalCooldown.CanWeave())
            {
                //oGCD - Buffs
                if (await Buff.NoMercy()) return true;
                if (await Buff.Bloodfest()) return true;

                //oGCD - Damage
                if (await SingleTarget.RoughDivide()) return true;
                if (await SingleTarget.BlastingZone()) return true;
                if (await Aoe.BowShock()) return true;
            }

            //Pull or get back aggro with LightningShot
            if (await SingleTarget.LightningShotToPullOrAggro()) return true;
            if (await SingleTarget.LightningShotToDps()) return true;

            //Apply DOT / Burst
            if (await Aoe.DoubleDown()) return true;
            if (await SingleTarget.SonicBreak()) return true;

            //Combo 2
            if (await SingleTarget.SavageClaw()) return true;
            if (await SingleTarget.WickedTalon()) return true;
            if (await SingleTarget.GnashingFang()) return true;

            //Combo 3
            if (await SingleTarget.BurstStrike()) return true;

            //AOE
            if (await Aoe.FatedCircle()) return true;
            if (await Aoe.DemonSlaughter()) return true;
            if (await Aoe.DemonSlice()) return true;

            //Combo 1 Filler
            if (await SingleTarget.SolidBarrel()) return true;
            if (await SingleTarget.BrutalShell()) return true;

            return await SingleTarget.KeenEdge();
        }

        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            if (await Tank.Guard(GunbreakerSettings.Instance)) return true;
            if (await Tank.Purify(GunbreakerSettings.Instance)) return true;
            if (await Tank.Recuperate(GunbreakerSettings.Instance)) return true;

            if (await Pvp.RelentlessRushPvp()) return true;
            if (await Pvp.NebulaPvp()) return true;
            if (await Pvp.AuroraPvp()) return true;

            if (!Tank.GuardCheck())
            {
                if (await Pvp.RoughDividePvp()) return true;
                if (await Pvp.BlastingZonePvp()) return true;
                if (await Pvp.DoubleDownPvp()) return true;

                if (await Pvp.DrawandJunctionPvp()) return true;
                if (await Pvp.ContinuationPvp()) return true;

                if (await Pvp.WickedTalonPvp()) return true;
                if (await Pvp.SavageClawPvp()) return true;
                if (await Pvp.GnashingFangPvp()) return true;
                if (await Pvp.BurstStrikePvp()) return true;
            }

            if (await Pvp.SolidBarrelPvp()) return true;
            if (await Pvp.BrutalShelPvp()) return true;

            return (await Pvp.KeenEdgePvp());
        }
    }
}
