using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Gunbreaker;
using Auras = Magitek.Utilities.Auras;
using GunbreakerRoutine = Magitek.Utilities.Routines.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class SingleTarget
    {

        /********************************************************************************
         *                               Pull - GCD
         *******************************************************************************/
        public static async Task<bool> LightningShotToDps()
        {
            if (!GunbreakerSettings.Instance.LightningShotToDps)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit()
                        || !Core.Me.CurrentTarget.NotInvulnerable()
                        || Core.Me.CurrentTarget.Distance(Core.Me) < Core.Me.CombatReach + Core.Me.CurrentTarget.CombatReach + GunbreakerSettings.Instance.LightningShotMinDistance
                        || Core.Me.CurrentTarget.Distance(Core.Me) > 20 + Core.Me.CurrentTarget.CombatReach
                        || (Core.Me.CurrentTarget as BattleCharacter).TargetGameObject == null)
                return false;

            if (!await Spells.LightningShot.Cast(Core.Me.CurrentTarget))
                return false;

            Logger.WriteInfo($@"Lightning Shot On {Core.Me.CurrentTarget.Name} To DPS");
            return true;
        }

        public static async Task<bool> LightningShotToPullOrAggro()
        {
            if (!GunbreakerSettings.Instance.LightningShotToPullAggro)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (!Core.Me.HasAura(Auras.RoyalGuard))
                return false;

            //need this in autonomous for dungeon profiles
            //if (BotManager.Current.IsAutonomous)
            //    return false;

            //find target already pulled on which I lose aggro
            var lightningShotTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit()
                                                                    && r.NotInvulnerable()
                                                                    && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach + GunbreakerSettings.Instance.LightningShotMinDistance
                                                                    && r.Distance(Core.Me) <= 20 + r.CombatReach
                                                                    && r.TargetGameObject != Core.Me);

            //if no target found, then check if current target is not pulled yet
            if (lightningShotTarget == null)
            {
                lightningShotTarget = (BattleCharacter) Core.Me.CurrentTarget;

                if (!lightningShotTarget.ValidAttackUnit()
                    || !lightningShotTarget.NotInvulnerable()
                    || lightningShotTarget.Distance(Core.Me) < Core.Me.CombatReach + lightningShotTarget.CombatReach + GunbreakerSettings.Instance.LightningShotMinDistance
                    || lightningShotTarget.Distance(Core.Me) > 20 + lightningShotTarget.CombatReach
                    || lightningShotTarget.TargetGameObject != null)
                    return false;
            }

            if (!await Spells.LightningShot.Cast(lightningShotTarget))
                return false;

            Logger.WriteInfo($@"Lightning Shot On {lightningShotTarget.Name} to pull");
            return true;
        }

        /********************************************************************************
         *                            Primary combo
         *******************************************************************************/
        public static async Task<bool> KeenEdge()
        {
            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.KeenEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BrutalShell()
        {
            if (!GunbreakerRoutine.CanContinueComboAfter(Spells.KeenEdge))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.BrutalShell.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SolidBarrel()
        {
            if (!GunbreakerRoutine.CanContinueComboAfter(Spells.BrutalShell))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (Cartridge == GunbreakerRoutine.MaxCartridge)
                return false;

            return await Spells.SolidBarrel.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                            Secondary combo
         *******************************************************************************/
        public static async Task<bool> GnashingFang()
        {
            if (!GunbreakerSettings.Instance.UseAmmoCombo)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (Cartridge < GunbreakerRoutine.RequiredCartridgeForGnashingFang)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= GunbreakerSettings.Instance.PrioritizeFatedCircleOverGnashingFangEnemies)
                return false;

            if (Spells.NoMercy.IsKnownAndReady(10000))
                return false;

            return await Spells.GnashingFang.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SavageClaw()
        {
            if (SecondaryComboStage != 1)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.SavageClaw.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WickedTalon()
        {
            if (SecondaryComboStage != 2)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.WickedTalon.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                           Secondary combo oGCD 
         *******************************************************************************/
        public static async Task<bool> JugularRip()
        {
            if (!Core.Me.HasAura(Auras.ReadytoRip))
                return false;

            return await Spells.JugularRip.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AbdomenTear()
        {
            if (!Core.Me.HasAura(Auras.ReadytoTear))
                return false;

            return await Spells.AbdomenTear.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EyeGouge()
        {
            if (!Core.Me.HasAura(Auras.ReadytoGouge))
                return false;

            return await Spells.EyeGouge.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                              Third combo oGCD  
         *******************************************************************************/

        public static async Task<bool> BurstStrike()
        {
            if (!GunbreakerSettings.Instance.UseBurstStrike)
                return false;

            if (Cartridge < GunbreakerRoutine.RequiredCartridgeForBurstStrike)
                return false; 
            
            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= GunbreakerSettings.Instance.PrioritizeFatedCircleOverBurstStrikeEnemies)
                return false;

            if (Core.Me.HasAura(Auras.NoMercy) && Cartridge > 0)
            {
                if (Cartridge < GunbreakerRoutine.MaxCartridge && (Spells.DoubleDown.IsKnownAndReady(10000) || Spells.GnashingFang.IsKnownAndReady(10000)))
                    return false;

                return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);
            }

            if (Cartridge == GunbreakerRoutine.MaxCartridge && ActionManager.LastSpell.Id != Spells.BrutalShell.Id)
                return false;

            if (Cartridge < GunbreakerRoutine.MaxCartridge)
                return false;

            return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);
        }

        /********************************************************************************
         *                              Third combo oGCD  
         *******************************************************************************/
        public static async Task<bool> Hypervelocity()
        {
            if (!Core.Me.HasAura(Auras.ReadytoBlast))
                return false;

            return await Spells.Hypervelocity.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                                    oGCD 
         *******************************************************************************/
        public static async Task<bool> BlastingZone()
        {
            if (GunbreakerSettings.Instance.SaveBlastingZone)
                if (Spells.NoMercy.Cooldown.TotalMilliseconds <= GunbreakerSettings.Instance.SaveBlastingZoneMseconds)
                    return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await GunbreakerRoutine.BlastingZone.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> RoughDivide() //Dash
        {
            if (!GunbreakerSettings.Instance.UseRoughDivide)
                return false;

            if (!Core.Me.HasAura(Auras.NoMercy))
                return false;

            if (GunbreakerSettings.Instance.RoughDivideOnlyInMelee
                && !Core.Me.CurrentTarget.WithinSpellRange(3))
            {
                return false;
            }

            if (Spells.RoughDivide.Charges < GunbreakerSettings.Instance.SaveRoughDivideCharges + 1)
            {
                return false;
            }

            //if (GunbreakerRoutine.IsAurasForComboActive())
            //    return false;

            return await Spells.RoughDivide.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                                    GCD 
         *******************************************************************************/
        public static async Task<bool> SonicBreak()
        {
            if (!Core.Me.HasAura(Auras.NoMercy))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.SonicBreak.Cast(Core.Me.CurrentTarget);
        }
    }
}