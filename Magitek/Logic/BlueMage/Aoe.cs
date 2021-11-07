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
            if (Casting.LastSpell != Spells.Surpanakha && Spells.Surpanakha.Charges < 4)
                return false;

            if (BlueMageSettings.Instance.UseMoonFlute && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.Surpanakha.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> JKick()
        {
            if (!BlueMageSettings.Instance.UseJKick)
                return false;

            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false;

            if (Utilities.Routines.BlueMage.IsMoonFluteWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.JKick.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FeatherRain()
        {
            if (!Core.Me.HasAura(Auras.WaxingNocturne) && Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false; 
            
            return await Spells.FeatherRain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Eruption()
        {
            if (!Core.Me.HasAura(Auras.WaxingNocturne) && Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            return await Spells.Eruption.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShockStrike()
        {
            if (!Core.Me.HasAura(Auras.WaxingNocturne) && Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            return await Spells.ShockStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Quasar()
        {
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 12 + r.CombatReach) < 1)
                return false;

            if (!Core.Me.HasAura(Auras.WaxingNocturne) && Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            return await Spells.Quasar.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GlassDance()
        {           
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 10 + r.CombatReach) < 1)
                return false;
            
            if (!Core.Me.HasAura(Auras.WaxingNocturne) && Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            return await Spells.GlassDance.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MountainBuster()
        {
            if (Core.Me.EnemiesInCone(6) < 2)
                return false;

            if (!Core.Me.HasAura(Auras.WaxingNocturne) && Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            return await Spells.MountainBuster.Cast(Core.Me.CurrentTarget);
        }

        /************** OTHER SPELLS **************/
        public static async Task<bool> NightBloom()
        {
            if (BlueMageSettings.Instance.UseMoonFlute && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.NightBloom.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhantomFlurry()
        {
            if (BlueMageSettings.Instance.UseMoonFlute && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            if (Spells.MatraMagic.Cooldown.TotalMilliseconds == 0)
                return false;

            return await Spells.PhantomFlurry.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhantomFlurryEnd()
        {
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