using System;
using System.Linq;
using System.Threading.Tasks;
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

            if (BardSettings.Instance.UseRageingStrikesOnlyDuringWanderersMinuet)
            {
                if (ActionResourceManager.Bard.ActiveSong == ActionResourceManager.Bard.BardSong.WanderersMinuet)
                {
                    if (BardSettings.Instance.DelayRageingStrikes)
                    {
                        if (ActionResourceManager.Bard.Timer.Seconds <= BardSettings.Instance.DelayRageingStrikesDuringWanderersMinuetUntilXSecondsRemaining)
                            return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);

                        return false;
                    }

                    return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);
                }

                return false;
            }

            return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);
        }

        /*
        public static async Task<bool> RagingStrikes()
        {
            if (!BardSettings.Instance.RagingStrikes)
                return false;

            if (Core.Me.CurrentTarget.CombatTimeLeft() < 20 && !Core.Me.CurrentTarget.IsBoss())
                return false;

            if (!Utilities.Routines.Bard.HealthCheck(Core.Me.CurrentTarget))
                return false;

            switch (BardSettings.Instance.CombatBuffStrategy)
            {
                case BuffStrategy.OnlyBosses when Core.Me.CurrentTarget.IsBoss():
                    return await DoRagingStrikes();
                case BuffStrategy.Always:
                    return await DoRagingStrikes();
                case BuffStrategy.Never:
                    return false;
            }

            return false;

            async Task<bool> DoRagingStrikes()
            {
                if (BardSettings.Instance.RagingStrikeAfterWanderersMinuet)
                {
                    if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.WanderersMinuet)
                        return false;
                }

                return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);
            }
        }
        */

            public static async Task<bool> NaturesMinne()
        {
            if (!BardSettings.Instance.NaturesMinne)
                return false;

            if (!Globals.InParty)
            {
                if (Core.Me.CurrentHealthPercent > BardSettings.Instance.NaturesMinneHealthPercent)
                    return false;

                return await Spells.NaturesMinne.Cast(Core.Me);
            }

            var naturesMinneTarget = Group.CastableAlliesWithin30.FirstOrDefault(r =>
                r.CurrentHealthPercent <= BardSettings.Instance.NaturesMinneHealthPercent && (
                    BardSettings.Instance.NaturesMinneTanks && r.IsTank() ||
                    BardSettings.Instance.NaturesMinneDps && r.IsDps() ||
                    BardSettings.Instance.NaturesMinneHealers && r.IsHealer()));

            if (naturesMinneTarget == null)
                return false;

            return await Spells.NaturesMinne.Cast(naturesMinneTarget);
        }

        public static async Task<bool> BattleVoice()
        {
            if (!BardSettings.Instance.UseBattleVoice)
                return false;

            if (!PartyManager.IsInParty)
                return false;

            if (BardSettings.Instance.UseBattleVoiceOnBossOnly && !Core.Me.CurrentTarget.IsBoss())
                return false;

            return await Spells.BattleVoice.Cast(Core.Me);
        }
    }
}