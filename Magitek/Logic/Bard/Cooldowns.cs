using ff14bot;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class Cooldowns
    {
        //&pins=2%24Main%24%23244F4B%24auras-gained%24-1%240.0.0.Any%240.0.0.Any%24true%240.0.0.Any%24true%241000125%24true%24true%24and%24any%24-1%240.0.0.Any%240.0.0.Any%24true%240.0.0.Any%24true%2416496|16495|3560|7409|7407|7406&type=casts 
        //FFLogs Pin To See The GCDs under RS
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

            if ((45 - ActionResourceManager.Bard.Timer.TotalSeconds) >= BardSettings.Instance.DelayRageingStrikesDuringWanderersMinuetUntilXSecondsInWM)
                return await Spells.RagingStrikes.CastAura(Core.Me, Auras.RagingStrikes);

            return false;

        }

        public static async Task<bool> BattleVoice()
        {
            if (!BardSettings.Instance.UseBattleVoice)
                return false;

            if (!Core.Me.HasAura(Auras.RagingStrikes))
                return false;

            return await Spells.BattleVoice.CastAura(Core.Me, Auras.BattleVoice, false, 0, false);
        }

        public static async Task<bool> Barrage()
        {

            if (Spells.Barrage.Cooldown != TimeSpan.Zero)
                return false;

            if (!BardSettings.Instance.UseBarrage)
                return false;

            //Wait for potential SSR procs from Burstshot/IJ/Windbite/VenomousBite
            if (Core.Me.ClassLevel < 76)
            {
                if (Spells.HeavyShot.Cooldown.TotalMilliseconds > 1850
                    && (Casting.LastSpell == Spells.HeavyShot || Casting.LastSpell == Spells.Windbite || Casting.LastSpell == Spells.VenomousBite || Casting.LastSpell == Spells.IronJaws))
                    return false;
            }
            else
            {
                if (Spells.HeavyShot.Cooldown.TotalMilliseconds > 1850
                    && (Casting.LastSpell == Spells.BurstShot || Casting.LastSpell == Spells.Stormbite || Casting.LastSpell == Spells.CausticBite || Casting.LastSpell == Spells.IronJaws))
                    return false;
            }

            //Dont Barrage when whe have a proc up
            if (Core.Me.HasAura(Auras.StraighterShot))
                return false;

            if (BardSettings.Instance.UseBarrageOnlyWithRageingStrikes && !Core.Me.HasAura(Auras.RagingStrikes))
                return false;

            return await Spells.Barrage.CastAura(Core.Me, Auras.Barrage);

        }

        public static async Task<bool> RadiantFinale()
        {
            if (!BardSettings.Instance.UseRadiantFinale)
                return false;

            //Basic Strategy for Raid (Mono Target)
            //TODO: Manage Multi Target (MB-PM-WM Strategy)
            if (ActionResourceManager.Bard.ActiveSong != ActionResourceManager.Bard.BardSong.WanderersMinuet)
                return false;

            if (!Core.Me.HasAura(Auras.RagingStrikes))
                return false;

            return await Spells.RadiantFinale.CastAura(Core.Me, Auras.RadiantFinale);
        }
    }
}