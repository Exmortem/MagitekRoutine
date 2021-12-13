using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using BardRoutine = Magitek.Utilities.Routines.Bard;

namespace Magitek.Logic.Bard
{
    public class Aoe
    {
        public static async Task<bool> ApexArrow()
        {
            if (!BardSettings.Instance.UseApexArrow)
                return false;

            if (BardSettings.Instance.UseBuffedApexArrow
                && ActionResourceManager.Bard.SoulVoice >= BardSettings.Instance.UseBuffedApexArrowWithAtLeastXSoulVoice)
            {
                if (Utilities.Routines.Bard.CheckCurrentDamageIncrease(BardSettings.Instance.UseBuffedApexArrowWithAtLeastXBonusDamage))
                    return await Spells.ApexArrow.Cast(Core.Me.CurrentTarget);
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

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.MagesBallad)
                return false;

            if (!BardSettings.Instance.PrioritizeBloodletterDuringMagesBallard)
                return false;

            if (Utilities.Routines.Bard.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RainOfDeath()
        {
            if (!BardSettings.Instance.UseRainOfDeath)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (Utilities.Routines.Bard.AoeEnemies8Yards < BardSettings.Instance.RainOfDeathEnemies)
                return false;

            return await Spells.RainofDeath.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShadowBite()
        {
            if (!BardSettings.Instance.UseShadowBite)
                return false;

            if (!ActionManager.HasSpell(Spells.Shadowbite.Id))
                return false;

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() <= 1)
                return false;

            return await Spells.Shadowbite.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> LadonsBite()
        {
            if (!BardSettings.Instance.UseQuickNock)
                return false;

            if (!BardSettings.Instance.UseAoe)
                return false;

            if (Utilities.Routines.Bard.EnemiesInCone < BardSettings.Instance.QuickNockEnemiesInCone)
                return false;

            return await BardRoutine.LadonsBite.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlastArrow()
        {
            if (!BardSettings.Instance.UseBlastArrow)
                return false;

            if (!ActionManager.HasSpell(Spells.BlastArrow.Id))
                return false;

            if (!Core.Me.HasAura(Auras.BlastArrowReady))
                return false;

            switch (ActionResourceManager.Bard.ActiveSong)
            {
                case ActionResourceManager.Bard.BardSong.None:
                    return false;

                case ActionResourceManager.Bard.BardSong.MagesBallad:
                    if (Core.Me.HasAura(Auras.BlastArrowReady, true, 3000) && Spells.Bloodletter.Cooldown == TimeSpan.Zero)
                        return false;
                    break;

                case ActionResourceManager.Bard.BardSong.WanderersMinuet:
                    if (Core.Me.HasAura(Auras.BlastArrowReady, true, 3000) && !Core.Me.HasAura(Auras.RagingStrikes) && Spells.RagingStrikes.Cooldown == TimeSpan.Zero)
                        return false;
                    break;

                case ActionResourceManager.Bard.BardSong.ArmysPaeon:
                    if (BardSettings.Instance.CurrentSongPlaylist == SongStrategy.WM_MB_AP)
                    {
                        if (BardSettings.Instance.EndArmysPaeonEarly)
                        {
                            if (Core.Me.HasAura(Auras.BlastArrowReady, true, 6000) && (ActionResourceManager.Bard.Timer.TotalSeconds - BardSettings.Instance.EndArmysPaeonEarlyWithXSecondsRemaining) < BardSettings.Instance.DontUseBlastArrowWhenAPEndsInXSeconds)
                                return false;
                        } else
                        {
                            if (Core.Me.HasAura(Auras.BlastArrowReady, true, 6000) && ActionResourceManager.Bard.Timer.TotalSeconds < BardSettings.Instance.DontUseBlastArrowWhenAPEndsInXSeconds)
                                return false;
                        }
                        
                    }
                    break;

                default:
                    break;
            }

            return await Spells.BlastArrow.Cast(Core.Me.CurrentTarget);
        }
    }
}