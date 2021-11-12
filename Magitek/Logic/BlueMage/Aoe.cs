using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Models.BlueMage;

namespace Magitek.Logic.BlueMage
{
    internal static class Aoe
    {
        /************** PRIMAL SPELLS **************/
        public static async Task<bool> Surpanakha()
        {
            //At least 1 ennemy in 16 yalms front
            if (Core.Me.EnemiesInCone(16) < 1)
                return false; 
            
            if (Casting.LastSpell != Spells.Surpanakha && Spells.Surpanakha.Charges < 4)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.Surpanakha.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> JKick()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            if (!BlueMageSettings.Instance.UseJKick)
                return false;

            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.JKick.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FeatherRain()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne) )
                return false; 
            
            return await Spells.FeatherRain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Eruption()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.Eruption.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShockStrike()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.ShockStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Quasar()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            //At least 1 ennemy in 15 yalm range
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) < 15 + r.CombatReach) < 1)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.Quasar.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GlassDance()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            //At least 1 ennemy in 12 yalm range
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) < 12 + r.CombatReach) < 1)
                return false;
            
            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.GlassDance.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MountainBuster()
        {
            if (!BlueMageSettings.Instance.UsePrimalSkills)
                return false;

            //At least 1 ennemy in 6 yalm front
            if (Core.Me.EnemiesInCone(6) < 1)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.MountainBuster.Cast(Core.Me.CurrentTarget);
        }

        /************** OTHER SPELLS **************/
        public static async Task<bool> NightBloom()
        {
            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            //At least 1 ennemy in 10 yalm range
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) < 10 + r.CombatReach) < 1)
                return false;

            return await Spells.NightBloom.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhantomFlurry()
        {
            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            //At least 1 ennemy in 8 yalm range
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) < 8 + r.CombatReach) < 1)
                return false;

            //force MatraMagic before
            if (Spells.MatraMagic.Cooldown.TotalMilliseconds == 0)
                return false;

            return await Spells.PhantomFlurry.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhantomFlurryEnd()
        {
            if (Spells.PhantomFlurryEnd.Cooldown.TotalMilliseconds >= 1)
                return false;

            return await Spells.PhantomFlurryEnd.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Tingle()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false; 
            
            if (Core.Me.HasAura(Auras.Tingle))
                return false;

            if (!Core.Me.HasAura(Auras.Harmonized))
                return false;

            if (Spells.TripleTrident.Cooldown.TotalMilliseconds >= 3000)
                return false;
            
            if (Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.Tingle.Cast(Core.Me.CurrentTarget);
        }

    }
}