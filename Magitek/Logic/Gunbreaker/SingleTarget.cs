using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class SingleTarget
    {
        public static async Task<bool> KeenEdge()
        {
            return await Spells.KeenEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BrutalShell()
        {
            if (ActionManager.LastSpell != Spells.KeenEdge)
                return false;

            
            return await Spells.BrutalShell.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SolidBarrel()
        {
            if (ActionManager.LastSpell != Spells.BrutalShell)
                return false;

            if (Cartridge == 2)
                return false;

            return await Spells.SolidBarrel.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> GnashingFang()
        {
            if (Cartridge == 0)
                return false;

            if (!GunbreakerSettings.Instance.UseAmmoCombo)
                return false;

            if (Spells.NoMercy.Cooldown.TotalMilliseconds < 10000)
                return false;

            return await Spells.GnashingFang.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SavageClaw()
        {
            if (SecondaryComboStage != 1)
                return false;
            
            return await Spells.SavageClaw.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WickedTalon()
        {
            if (SecondaryComboStage != 2)
                return false;
            
            return await Spells.WickedTalon.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> SonicBreak()
        {
            if (!Core.Player.HasAura(Auras.NoMercy))
                return false;
            
            return await Spells.SonicBreak.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DangerZone()
        {
            if (GunbreakerSettings.Instance.SaveDangerZone)
                if (Spells.NoMercy.Cooldown.TotalMilliseconds <= GunbreakerSettings.Instance.SaveDangerZoneMseconds)
                    return false;

            //Only use in the last 1/3rd of GCD window
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 850)
                return false;

            return await Spells.DangerZone.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlastingZone()
        {
            if (GunbreakerSettings.Instance.SaveBlastingZone)
                if (Spells.NoMercy.Cooldown.TotalMilliseconds <= GunbreakerSettings.Instance.SaveBlastingZoneMseconds)
                    return false;
            //Only use in the last 1/3rd of GCD window
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 850)
                return false;


            return await Spells.BlastingZone.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> BurstStrike()
        {
            if (Cartridge == 0)
                return false;
            //Save your bullet for Gnashin
            if (!Core.Player.HasAura(Auras.NoMercy) && Spells.GnashingFang.Cooldown.TotalMilliseconds < 10000 && Cartridge != 2)
                return false;
            if (Spells.NoMercy.Cooldown.TotalMilliseconds < 3000)
                return false;
            if (Core.Me.ClassLevel > 75 && Spells.Bloodfest.Cooldown.TotalMilliseconds < 5100)
                return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);
            if (Core.Player.HasAura(Auras.NoMercy) && Cartridge != 0 && Spells.GnashingFang.Cooldown.TotalMilliseconds > 0)
                return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);
            if (Cartridge == 2)
                return await Spells.BurstStrike.Cast(Core.Me.CurrentTarget);
            return false;

        }
        
        public static async Task<bool> JugularRip()
        {
            if (!Core.Player.HasAura(Auras.ReadytoRip))
                return false;

            //Only use in the last 1/3rd of GCD window
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 850)
                return false;

            return await Spells.JugularRip.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AbdomenTear()
        {
            if (!Core.Player.HasAura(Auras.ReadytoTear))
                return false;

            //Only use in the last 1/3rd of GCD window
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 750)
                return false;

            return await Spells.AbdomenTear.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EyeGouge()
        {
            if (!Core.Player.HasAura(Auras.ReadytoGouge))
                return false;

            //Only use in the last 1/3rd of GCD window
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 850)
                return false;

            return await Spells.EyeGouge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RoughDivide()
        {
            if (!GunbreakerSettings.Instance.UseRoughDivide)
                return false;

            //Only use in the last 1/3rd of GCD window
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 900)
                return false;

           
            if (Core.Player.HasAura(Auras.NoMercy) && Casting.LastSpell == Spells.BurstStrike)
                return await Spells.RoughDivide.Cast(Core.Me.CurrentTarget);
            if(Core.Player.HasAura(Auras.NoMercy) && Casting.LastSpell == Spells.KeenEdge)
                return await Spells.RoughDivide.Cast(Core.Me.CurrentTarget);
            if(Core.Player.HasAura(Auras.NoMercy) && Casting.LastSpell == Spells.BrutalShell)
                return await Spells.RoughDivide.Cast(Core.Me.CurrentTarget);
            if (Core.Player.HasAura(Auras.NoMercy) && Casting.LastSpell == Spells.SolidBarrel)
                return await Spells.RoughDivide.Cast(Core.Me.CurrentTarget);

            return false; 
        }

        public static async Task<bool> LightningShot()
        {
            if (!GunbreakerSettings.Instance.LightningShotToPullAggro)
                return false;

            if (!Core.Me.HasAura(Auras.RoyalGuard))
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var lightningShotTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach + GunbreakerSettings.Instance.LightningShotMinDistance
                                                                         && r.Distance(Core.Me) <= 15 + r.CombatReach 
                                                                         && r.TargetGameObject != Core.Me);

            if (lightningShotTarget == null)
                return false;

            if (lightningShotTarget.TargetGameObject == null)
                return false;

            if (!await Spells.LightningShot.Cast(lightningShotTarget))
                return false;

            Logger.Write($@"Lightning Shot On {lightningShotTarget.Name} To Pull Aggro");
            return true;
        }
    }
}