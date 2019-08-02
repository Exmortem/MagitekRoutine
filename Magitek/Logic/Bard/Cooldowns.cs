using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Logic.Bard
{
    internal static class Cooldowns
    {

        public static async Task<bool> RagingStrikes()
        {

            if (!BardSettings.Instance.UseRageingStrikes)
                return false;

            if (Spells.RagingStrikes.Cooldown != TimeSpan.Zero)
                return false;

            if (BardSettings.Instance.DelayRageingStrikesUntilBarrageIsReady)
                if (Spells.Barrage.Cooldown != TimeSpan.Zero)
                    return false;
            

            switch (BardSettings.Instance.UseCoolDowns)
            {
                case BuffStrategy.OnlyBosses when Core.Me.CurrentTarget.IsBoss():
                    break;
                case BuffStrategy.Always:
                    break;
                case BuffStrategy.Never:
                    return false;
            }

            if (!BardSettings.Instance.UseRageingStrikesOnlyDuringWanderersMinuet)
                return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);

            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.WanderersMinuet)
                return false;

            if (!BardSettings.Instance.DelayRageingStrikes)
                return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);

            if ((30 - ActionResourceManager.Bard.Timer.TotalSeconds) >= BardSettings.Instance.DelayRageingStrikesDuringWanderersMinuetUntilXSecondsInWM)
                return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);

            return false;

        }

        public static async Task<bool> BattleVoice()
        {
            if (!BardSettings.Instance.UseBattleVoice)
                return false;

            if (!PartyManager.IsInParty)
                return false;

            return await Spells.BattleVoice.Cast(Core.Me);
        }

        public static async Task<bool> RefulgentBarrage()
        {
            if (Spells.Barrage.Cooldown != TimeSpan.Zero)
                return false;

            if (!BardSettings.Instance.UseBarrage)
                return false;

            //Dont Barrage when whe have a proc up
            if (Core.Me.HasAura(Auras.StraighterShot))
                return false;

            if (BardSettings.Instance.UseBarrageOnlyWithRageingStrikes && !Core.Me.HasAura(Auras.RagingStrikes))
                return false;

            return await BarrageCombo();

            // Handle Barrage + Straighter Shot
            async Task<bool> BarrageCombo()
            {
                if (await Spells.Barrage.Cast(Core.Me))
                    await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Barrage));
                else
                    return false;

                while (Core.Me.HasAura(Auras.Barrage))
                {
                    if (await Spells.StraightShot.Cast(Core.Me.CurrentTarget))
                        return true;

                    await Coroutine.Yield();
                }

                return false;
            }
        }
    }
}