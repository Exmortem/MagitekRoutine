using ff14bot;
using ff14bot.Managers;
using BardSong = ff14bot.Managers.ActionResourceManager.Bard.BardSong;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Bard;
using Magitek.Utilities;
using BardRoutine = Magitek.Utilities.Routines.Bard;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    public class Aoe
    {
        public static async Task<bool> ApexArrow()
        {
            if (!BardSettings.Instance.UseApexArrow)
                return false;

            if (BardSettings.Instance.UseBuffedApexArrow)
            {
                //Delay to use it inside Burst windows
                if (Spells.RagingStrikes.IsKnownAndReady(7000))
                    return false;

                //Use Apex in Buff windows under WM and delay it as much as possible at the end of buff
                if (BardSong.WanderersMinuet.Equals(ActionResourceManager.Bard.ActiveSong) && BardRoutine.IsUnderBuffWindow)
                {
                    if (ActionResourceManager.Bard.SoulVoice == 100)
                        return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);

                    if (BardRoutine.CheckCurrentDamageIncrease(BardSettings.Instance.UseBuffedApexArrowWithAtLeastXBonusDamage)
                        && ActionResourceManager.Bard.SoulVoice >= BardSettings.Instance.UseBuffedApexArrowWithAtLeastXSoulVoice
                        && (Core.Me.Auras.Any(x => x.Id == Auras.RagingStrikes && x.TimespanLeft.TotalMilliseconds < 7500)
                            || Core.Me.Auras.Any(x => x.Id == Auras.RadiantFinale && x.TimespanLeft.TotalMilliseconds < 7500)
                            || Core.Me.Auras.Any(x => x.Id == Auras.BattleVoice && x.TimespanLeft.TotalMilliseconds < 7500)))
                            return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);
                }

                //Force Apex in MB at 22sec if soulgauge >= 80
                if (BardSong.MagesBallad.Equals(ActionResourceManager.Bard.ActiveSong) 
                    && (ActionResourceManager.Bard.SoulVoice == 100 || ActionResourceManager.Bard.SoulVoice >= 80 && ActionResourceManager.Bard.Timer.TotalMilliseconds - Spells.HeavyShot.Cooldown.TotalMilliseconds - 21000 <= 1) )
                {
                    if(await Spells.ApexArrow.Cast(Core.Me.CurrentTarget))
                    {
                        Logger.WriteInfo($@"[ApexArrow] Execution with SoulVoice = {ActionResourceManager.Bard.SoulVoice} at {ActionResourceManager.Bard.Timer.TotalMilliseconds}"); 
                        return true;
                    }
                }
                return false;                    
            }

            if (ActionResourceManager.Bard.SoulVoice < BardSettings.Instance.UseApexArrowWithAtLeastXSoulVoice)
                return false;

            return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RainOfDeathDuringMagesBallard()
        {
            if (!BardSettings.Instance.UseAoe)
                return false;

            if (!BardSettings.Instance.UseRainOfDeath)
                return false;

            if (ActionResourceManager.Bard.ActiveSong != BardSong.MagesBallad)
                return false;

            if (!BardSettings.Instance.PrioritizeBloodletterDuringMagesBallard)
                return false;

            if (BardRoutine.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RainOfDeath()
        {
            if (!BardSettings.Instance.UseRainOfDeath)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (BardRoutine.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShadowBite()
        {
            if (!BardSettings.Instance.UseShadowBite)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < BardSettings.Instance.ShadowBiteEnemies)
                return false;

            return await Spells.Shadowbite.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> LadonsBite()
        {
            if (!BardSettings.Instance.UseQuickNock)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (BardRoutine.EnemiesInCone < BardSettings.Instance.QuickNockEnemiesInCone)
                return false;

            return await BardRoutine.LadonsBite.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlastArrow()
        {
            if (!BardSettings.Instance.UseBlastArrow)
                return false;

            if (!Core.Me.HasAura(Auras.BlastArrowReady))
                return false;

            if (BardSong.MagesBallad.Equals(ActionResourceManager.Bard.ActiveSong)
                && Core.Me.HasAura(Auras.BlastArrowReady, true, 3000) && Spells.Bloodletter.IsReady())
                return false;

            return await Spells.BlastArrow.Cast(Core.Me.CurrentTarget);
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return PhysicalDps.ForceLimitBreak(BardSettings.Instance, Spells.BigShot, Spells.Desperado, Spells.SaggitariusArrow, Spells.HeavyShot);
        }
    }
}