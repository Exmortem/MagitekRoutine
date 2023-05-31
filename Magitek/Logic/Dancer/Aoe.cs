using ff14bot;
using ff14bot.Objects;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using System.Linq;
using System.Threading.Tasks;


namespace Magitek.Logic.Dancer
{
    internal static class Aoe
    {

        /*************************************************************************************
         *                       AOE used in Single Target Rotation
         * ***********************************************************************************/
        public static async Task<bool> StarfallDance()
        {
            if (!DancerSettings.Instance.StarfallDance)
                return false;

            if (Core.Me.ClassLevel < Spells.StarfallDance.LevelAcquired) 
                return false;

            if (!Core.Me.HasAura(Auras.FlourishingStarfall)) 
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) 
                return false;

            if (ActionResourceManager.Dancer.Esprit >= 100)
                return false; 
            
            if (!Core.Me.CurrentTarget.InCustomDegreeCone(15)) 
                return false;

            return await Spells.StarfallDance.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FanDance4()
        {
            if (!DancerSettings.Instance.FanDance4)
                return false;

            if (Core.Me.ClassLevel < Spells.FanDanceIV.LevelAcquired) 
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) 
                return false;

            if (!Core.Me.HasAura(Auras.FourfoldFanDance)) 
                return false;

            if (!Core.Me.CurrentTarget.InCustomRadiantCone(Spells.StarfallDance.Radius)) 
                return false;

            return await Spells.FanDanceIV.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FanDance3()
        {
            if (!DancerSettings.Instance.FanDance3)
                return false;

            if (Core.Me.ClassLevel < Spells.FanDance3.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (!Core.Me.HasAura(Auras.ThreefoldFanDance))
                return false;

            return await Spells.FanDance3.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SaberDance()
        {
            if (!DancerSettings.Instance.SaberDance)
                return false;

            if (Core.Me.ClassLevel < Spells.SaberDance.LevelAcquired)
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (DancerSettings.Instance.UseRangeAndFacingChecks)
                if (Core.Me.CurrentTarget.Distance(Core.Me) - Core.Me.CurrentTarget.CombatReach > Spells.SaberDance.Range)
                    return false;

            if (ActionResourceManager.Dancer.Esprit < 50)
                return false;

            if (ActionResourceManager.Dancer.Esprit >= 100)
                return await Spells.SaberDance.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.HasAura(Auras.TechnicalFinish))
            {
                if (ActionResourceManager.Dancer.Esprit >= DancerSettings.Instance.SaberDanceEsprit)
                {
                    //Fountainfall - Bloodshower
                    Aura SilkenFlowAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.SilkenFlow);
                    if (Core.Me.HasAura(Auras.SilkenFlow) && SilkenFlowAura.TimespanLeft.TotalMilliseconds < 3000)
                        return false;

                    //Reverse Cascade - RisingWindmill
                    Aura SilkenSymmetryAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.SilkenSymmetry);
                    if (Core.Me.HasAura(Auras.SilkenSymmetry) && SilkenSymmetryAura.TimespanLeft.TotalMilliseconds < 3000)
                        return false;

                    return await Spells.SaberDance.Cast(Core.Me.CurrentTarget);
                }

                return false;
                //if (ActionResourceManager.Dancer.Esprit >= 70 && Spells.StandardStep.IsKnownAndReady(5000))
                //    return await Spells.SaberDance.Cast(Core.Me.CurrentTarget);
            }
            return await Spells.SaberDance.Cast(Core.Me.CurrentTarget);
        }


        /*************************************************************************************
         *                          AOE used in Multiple Target Rotation
         * ***********************************************************************************/
        public static async Task<bool> FanDance2()
        {
            if (!DancerSettings.Instance.UseAoe) return false;

            if (!DancerSettings.Instance.FanDanceTwo) return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.FanDance2.Radius + r.CombatReach) < DancerSettings.Instance.FanDanceTwoEnemies) return false;

            if (ActionResourceManager.Dancer.FourFoldFeathers < 4 && !Core.Me.HasAura(Auras.Devilment) && Core.Me.ClassLevel >= 62) return false;

            return await Spells.FanDance2.Cast(Core.Me);
        }

        public static async Task<bool> Bloodshower()
        {
            if (!DancerSettings.Instance.UseAoe)
                return false;

            if (!DancerSettings.Instance.Bloodshower)
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.Bloodshower.Radius + r.CombatReach) < DancerSettings.Instance.BloodshowerEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.SilkenFlow) && !Core.Me.HasAura(Auras.FlourishingFlow))
                return false;

            return await Spells.Bloodshower.Cast(Core.Me);
        }

        public static async Task<bool> RisingWindmill()
        {
            if (!DancerSettings.Instance.UseAoe) 
                return false;

            if (!DancerSettings.Instance.RisingWindmill) 
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) 
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.RisingWindmill.Radius + r.CombatReach) < DancerSettings.Instance.RisingWindmillEnemies) 
                return false;

            if (Core.Me.CurrentTarget == null) 
                return false;

            if (!Core.Me.HasAura(Auras.FlourishingSymmetry) && !Core.Me.HasAura(Auras.SilkenSymmetry)) 
                return false;

            return await Spells.RisingWindmill.Cast(Core.Me);
        }

        public static async Task<bool> Bladeshower()
        {
            if (!DancerSettings.Instance.UseAoe) 
                return false;

            if (!DancerSettings.Instance.Bladeshower) 
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) 
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.Bladeshower.Radius + r.CombatReach) < DancerSettings.Instance.BladeshowerEnemies) 
                return false;

            if (ActionManager.LastSpell != Spells.Windmill) 
                return false;

            return await Spells.Bladeshower.Cast(Core.Me);
        }

        public static async Task<bool> Windmill()
        {
            if (!DancerSettings.Instance.UseAoe) 
                return false;

            if (!DancerSettings.Instance.Windmill) 
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) 
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.Windmill.Radius + r.CombatReach) < DancerSettings.Instance.WindmillEnemies) 
                return false;

            return await Spells.Windmill.Cast(Core.Me);
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return PhysicalDps.ForceLimitBreak(Spells.BigShot, Spells.Desperado, Spells.CrimsonLotus, Spells.Cascade);
        }
    }
}