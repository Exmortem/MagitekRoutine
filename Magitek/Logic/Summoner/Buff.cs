using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using System;
using System.Threading.Tasks;

namespace Magitek.Logic.Summoner
{
    internal static class Buff
    {
        public static async Task<bool> DreadwyrmTrance()
        {
            if (Core.Me.ClassLevel < 58)
                return false;

            if (!SummonerSettings.Instance.DreadwyrmTrance
                || !SummonerSettings.Instance.FirebirdTrance)
                return false;

            //if (ActionResourceManager.Arcanist.AetherAttunement == 2)
            //    return false;

            if ((int)PetManager.ActivePetType == 10
                || (int)PetManager.ActivePetType == 14)
                return false;

            if (Spells.Ruin.Cooldown.TotalMilliseconds < 850)
                return false;

            if (Spells.TriDisaster.Cooldown.TotalMilliseconds < 2000)
                return false;

            /*if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;*/

            return await Spells.Trance.Cast(Core.Me);
        }

        public static async Task<bool> LucidDreaming()
        {
            if (Core.Me.ClassLevel < 24)
                return false;

            if (Core.Me.CurrentManaPercent > SummonerSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (ActionResourceManager.Summoner.TranceTimer == 0)
                return false;

            return await Spells.LucidDreaming.Cast(Core.Me);
        }


        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }
    }
}