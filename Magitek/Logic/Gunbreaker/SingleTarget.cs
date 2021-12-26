using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Gunbreaker;
using GunbreakerRoutine = Magitek.Utilities.Routines.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class SingleTarget
    {

        /********************************************************************************
         *                               Pull - GCD
         *******************************************************************************/
        public static async Task<bool> LightningShot()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.LightningShotToPullAggro, Spells.LightningShot))
                return false;

            if (!Core.Me.HasAura(Auras.RoyalGuard))
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var lightningShotTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach + GunbreakerSettings.Instance.LightningShotMinDistance
                                                                         && r.Distance(Core.Me) <= 20 + r.CombatReach
                                                                         && r.TargetGameObject != Core.Me);

            if (lightningShotTarget == null)
                return false;

            if (lightningShotTarget.TargetGameObject == null)
                return false;

            if (!await Spells.LightningShot.Cast(lightningShotTarget))
                return false;

            Logger.WriteInfo($@"Lightning Shot On {lightningShotTarget.Name} To Pull Aggro");
            return true;
        }

        /********************************************************************************
         *                            Primary combo
         *******************************************************************************/
        public static async Task<bool> KeenEdge()
        {
            if (!ActionManager.HasSpell(Spells.KeenEdge.Id))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.KeenEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BrutalShell()
        {
            if (!ActionManager.HasSpell(Spells.BrutalShell.Id))
                return false;

            if (!GunbreakerRoutine.CanContinueComboAfter(Spells.KeenEdge))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.BrutalShell.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SolidBarrel()
        {
            if (!ActionManager.HasSpell(Spells.SolidBarrel.Id))
                return false;

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
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseAmmoCombo, Spells.GnashingFang))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (Cartridge < GunbreakerRoutine.RequiredCartridgeForGnashingFang)
                return false;

            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.NoMercy, 10000))
                return false;

            return await Spells.GnashingFang.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SavageClaw()
        {
            if (!ActionManager.HasSpell(Spells.SavageClaw.Id))
                return false;

            if (SecondaryComboStage != 1)
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.SavageClaw.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WickedTalon()
        {
            if (!ActionManager.HasSpell(Spells.WickedTalon.Id))
                return false;

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
            if (!ActionManager.HasSpell(Spells.JugularRip.Id))
                return false;

            if (!Core.Me.HasAura(Auras.ReadytoRip))
                return false;

            return await Spells.JugularRip.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AbdomenTear()
        {
            if (!ActionManager.HasSpell(Spells.AbdomenTear.Id))
                return false;

            if (!Core.Me.HasAura(Auras.ReadytoTear))
                return false;

            return await Spells.AbdomenTear.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EyeGouge()
        {
            if (!ActionManager.HasSpell(Spells.EyeGouge.Id))
                return false;

            if (!Core.Me.HasAura(Auras.ReadytoGouge))
                return false;

            return await Spells.EyeGouge.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                              Third combo oGCD  
         *******************************************************************************/

        public static async Task<bool> BurstStrike()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseBurstStrike, Spells.BurstStrike))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            if (Cartridge < GunbreakerRoutine.RequiredCartridgeForBurstStrike)
                return false;

            if (Core.Me.HasAura(Auras.NoMercy) && Cartridge > 0)
            {
                if (Spells.IsAvailableAndReady(Spells.DoubleDown) || Spells.IsAvailableAndReady(Spells.GnashingFang))
                    return false;
            }

            //Delay if nomercy ready soon
            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.NoMercy, 14000) && Cartridge < GunbreakerRoutine.MaxCartridge - 1)
                return false;
            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.NoMercy, 7000) && Cartridge < GunbreakerRoutine.MaxCartridge)
                return false;

            //Delay if GnashingFang ready soon
            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.GnashingFang, 4000) && Cartridge <= GunbreakerRoutine.RequiredCartridgeForGnashingFang)
                return false;

            //Delay if DoubleDown ready soon
            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.DoubleDown, 4000) && Cartridge <= GunbreakerRoutine.RequiredCartridgeForDoubleDown)
                return false;

            if (Spells.IsAvailableAndReadyInLessThanXMs(Spells.Bloodfest, 2000) && Cartridge > 0)
                return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);

            return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);
        }

        /********************************************************************************
         *                              Third combo oGCD  
         *******************************************************************************/
        public static async Task<bool> Hypervelocity()
        {
            if (!ActionManager.HasSpell(Spells.Hypervelocity.Id))
                return false;

            if (!Core.Me.HasAura(Auras.ReadytoBlast))
                return false;

            return await Spells.Hypervelocity.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                                    oGCD 
         *******************************************************************************/
        public static async Task<bool> BlastingZone()
        {
            if (!ActionManager.HasSpell(GunbreakerRoutine.BlastingZone.Id))
                return false;

            if (GunbreakerSettings.Instance.SaveBlastingZone)
                if (Spells.NoMercy.Cooldown.TotalMilliseconds <= GunbreakerSettings.Instance.SaveBlastingZoneMseconds)
                    return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await GunbreakerRoutine.BlastingZone.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> RoughDivide() //Dash
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseRoughDivide, Spells.RoughDivide))
                return false;

            if (!Core.Me.HasAura(Auras.NoMercy))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.RoughDivide.Cast(Core.Me.CurrentTarget);
        }


        /********************************************************************************
         *                                    GCD 
         *******************************************************************************/
        public static async Task<bool> SonicBreak()
        {
            if (!ActionManager.HasSpell(Spells.SonicBreak.Id))
                return false;

            if (!Core.Me.HasAura(Auras.NoMercy))
                return false;

            if (GunbreakerRoutine.IsAurasForComboActive())
                return false;

            return await Spells.SonicBreak.Cast(Core.Me.CurrentTarget);
        }
    }
}