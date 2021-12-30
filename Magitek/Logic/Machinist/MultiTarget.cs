using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using MachinistRoutine = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class MultiTarget
    {
        public static async Task<bool> Scattergun()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseScattergun, MachinistRoutine.Scattergun))
                return false;

            if (!MachinistSettings.Instance.UseAoe)
                return false;

            if (Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.SpreadShotEnemyCount)
                return false;

            return await MachinistRoutine.Scattergun.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BioBlaster()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseBioBlaster, Spells.Bioblaster))
                return false;

            if (!MachinistSettings.Instance.UseAoe)
                return false;

            if (Core.Me.HasAura(Auras.Reassembled))
                return false;

            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.BioBlasterEnemyCount)
                return false;

            return await Spells.Bioblaster.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AutoCrossbow()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseAutoCrossbow, Spells.AutoCrossbow))
                return false;

            if (!MachinistSettings.Instance.UseAoe)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            if (Core.Me.EnemiesInCone(12) < MachinistSettings.Instance.AutoCrossbowEnemyCount)
                return false;

            return await Spells.AutoCrossbow.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Flamethrower()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseFlamethrower, Spells.Flamethrower))
                return false;

            if (!MachinistSettings.Instance.UseAoe)
                return false;

            if (ActionResourceManager.Machinist.Heat >= 50)
                return false;

            if (Spells.Wildfire.IsKnownAndReady(11000))
                return false;

            if (Spells.Reassemble.IsKnownAndReady(11000))
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true) || Casting.SpellCastHistory.Any(x => x.Spell == Spells.Wildfire))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.EnemiesInCone(8) < MachinistSettings.Instance.FlamethrowerEnemyCount)
                return false;

            return await Spells.Flamethrower.CastAura(Core.Me, Auras.Flamethrower);
        }
        public static async Task<bool> Ricochet()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseRicochet, Spells.Ricochet))
                return false;

            if (!MachinistRoutine.IsInWeaveingWindow)
                return false;

            if (Casting.LastSpell == Spells.Wildfire || Casting.LastSpell == Spells.Hypercharge)
                return false;

            if (Spells.Wildfire.IsKnownAndReady() && Spells.Hypercharge.IsKnownAndReady() && Spells.Ricochet.Charges < 1.5f)
                return false;

            if (Core.Me.ClassLevel >= 45)
            {
                if (Spells.Ricochet.Charges < 1.5f && Spells.Wildfire.IsKnownAndReady(2000))
                    return false;

                // Do not run Rico if an hypercharge is almost ready and not enough charges available for Rico and Gauss
                if (ActionResourceManager.Machinist.Heat > 45 && Spells.Hypercharge.IsKnownAndReady())
                {
                    if (Spells.Ricochet.Charges < 1.5f && Spells.GaussRound.Charges < 0.5f)
                        return false;
                }
            }

            return await Spells.Ricochet.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ChainSaw()
        {
            if (!MachinistRoutine.ToggleAndSpellCheck(MachinistSettings.Instance.UseChainSaw, Spells.ChainSaw))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            if (MachinistSettings.Instance.UseRookQueen && ActionResourceManager.Machinist.Battery > 80)
                return false;

            return await Spells.ChainSaw.Cast(Core.Me.CurrentTarget);
        }
    }
}
