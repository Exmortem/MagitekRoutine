using System;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using Magitek.Utilities.Routines;

namespace Magitek.Logic.Summoner
{
    internal static class Buff
    {
        public static async Task<bool> DreadwyrmTrance()
        {
            if (Core.Me.ClassLevel < 58) return false;

            if (ActionResourceManager.Arcanist.AetherAttunement == 2) return false;

            if ((int)PetManager.ActivePetType == 10) return false;

            if (Spells.TriDisaster.Cooldown == TimeSpan.Zero) return false;

            return await Spells.Trance.Cast(Core.Me);
        }

        public static async Task<bool> LucidDreaming()
        {
            if (Core.Me.ClassLevel < 24) return false;

            if (Core.Me.CurrentManaPercent > SummonerSettings.Instance.LucidDreamingManaPercent) return false;

            return await Spells.LucidDreaming.Cast(Core.Me);
        }

        public static async Task<bool> Aetherpact()
        {
            if (Core.Me.ClassLevel < 64) return false;
            
            return await Spells.SmnAetherpact.Cast(Core.Me);
        }
    }
}