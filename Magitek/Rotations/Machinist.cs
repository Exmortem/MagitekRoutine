using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Machinist;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using MachinistRoutine = Magitek.Utilities.Routines.Machinist;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Magitek.Rotations
{
    public static class Machinist
    {
        public static Task<bool> Rest()
        {
            var needRest = Core.Me.CurrentHealthPercent < MachinistSettings.Instance.RestHealthPercent;
            return Task.FromResult(needRest);
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();
            if (WorldManager.InSanctuary)
                return false;

            return await PhysicalDps.Peloton(MachinistSettings.Instance);
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }
        public static async Task<bool> Heal()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await GambitLogic.Gambit();
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
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (!SpellQueueLogic.SpellQueue.Any())
                SpellQueueLogic.InSpellQueue = false;

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) 
                return true;

            if (MachinistSettings.Instance.UseFlamethrower && Core.Me.HasAura(Auras.Flamethrower))
            {
                // First check movement otherwise Flamethrower can be executed whereas you are moving
                if (MovementManager.IsMoving)
                    return false;

                if (!MachinistSettings.Instance.UseFlamethrower)
                    return true;

                if (Core.Me.EnemiesInCone(8) >= MachinistSettings.Instance.FlamethrowerEnemyCount)
                    return true;
            }

            //LimitBreak
            if (MultiTarget.ForceLimitBreak()) return true;

            if (ActionResourceManager.Machinist.OverheatRemaining != TimeSpan.Zero)
            {
                if (MachinistRoutine.GlobalCooldown.CanWeave(1)) {
                    //Utility
                    if (await PhysicalDps.ArmsLength(MachinistSettings.Instance)) return true;
                    if (await PhysicalDps.Interrupt(MachinistSettings.Instance)) return true;

                    //Pets
                    if (await Pet.RookQueen()) return true;

                    //Cooldowns
                    if (await Cooldowns.BarrelStabilizer()) return true;
                    //if (await Cooldowns.Reassemble()) return true;
                    
                    //oGCDs
                    if (await SingleTarget.GaussRound()) return true;
                    if (await MultiTarget.Ricochet()) return true;
                }
            } 
            else
            {
                if (MachinistRoutine.GlobalCooldown.CanWeave()) {
                    //Utility
                    if (await PhysicalDps.ArmsLength(MachinistSettings.Instance)) return true;
                    if (await Utility.Tactician()) return true;
                    if (await Utility.Dismantle()) return true;
                    if (await PhysicalDps.SecondWind(MachinistSettings.Instance)) return true;
                    if (await PhysicalDps.Interrupt(MachinistSettings.Instance)) return true;
                    if (await Cooldowns.UsePotion()) return true;

                    //Pets
                    if (await Pet.RookQueen()) return true;
                    if (await Pet.RookQueenOverdrive()) return true;

                    //Cooldowns
                    if (await Cooldowns.Reassemble()) return true;
                    if (await Cooldowns.Hypercharge()) return true;
                    if (await Cooldowns.Wildfire()) return true;
                    if (await Cooldowns.BarrelStabilizer()) return true;

                    //oGCDs
                    if (await SingleTarget.GaussRound()) return true;
                    if (await MultiTarget.Ricochet()) return true;

                }
            }

            //GCDs - Top Hypercharge Priority
            if (await MultiTarget.AutoCrossbow()) return true;
            if (await SingleTarget.HeatBlast()) return true;

            //Use On CD
            if (await MultiTarget.BioBlaster()) return true;
            if (await SingleTarget.HotAirAnchor()) return true;
            if (await SingleTarget.Drill()) return true;
            if (await MultiTarget.ChainSaw()) return true;
            
            //AOE
            if (await MultiTarget.Flamethrower()) return true;
            if (await MultiTarget.Scattergun()) return true;

            //Default Combo
            if (await SingleTarget.HeatedCleanShot()) return true;
            if (await SingleTarget.HeatedSlugShot()) return true;

            return await SingleTarget.HeatedSplitShot();
        }
        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            // Utilities
            if (await PhysicalDps.Guard(MachinistSettings.Instance)) return true;
            if (await PhysicalDps.Purify(MachinistSettings.Instance)) return true;
            if (await PhysicalDps.Recuperate(MachinistSettings.Instance)) return true;

            if (!PhysicalDps.GuardCheck())
            {
                //LB
                if (await Pvp.MarksmansSpite()) return true;
                if (await Pvp.HeatBlast()) return true;
                if (await Pvp.Scattergun()) return true;

                // Buff
                if (await Pvp.BishopAutoturret()) return true;
                if (await Pvp.Analysis()) return true;
                if (await Pvp.WildFire()) return true;

                // Tools
                if (await Pvp.ChainSaw()) return true;
                if (await Pvp.AirAnchor()) return true;
                if (await Pvp.BioBlaster()) return true;
                if (await Pvp.Drill()) return true;
            }

            // Main
            return await Pvp.BlastedCharge();
        }
    }
}
