using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using System.Threading.Tasks;
using ArcResources = ff14bot.Managers.ActionResourceManager.Arcanist;
using SmnResources = ff14bot.Managers.ActionResourceManager.Summoner;
using static Magitek.Utilities.Routines.Summoner;

namespace Magitek.Logic.Summoner
{
    internal static class Buff
    {
        public static async Task<bool> DreadwyrmTrance()
        {
            if (!SummonerSettings.Instance.DreadwyrmTrance)
                return false;

            if (!Spells.DreadwyrmTrance.IsKnownAndReady())
                return false;

            if (Spells.SummonBahamut.IsKnown())
                return false;
            
            if (SmnResources.PetTimer + SmnResources.TranceTimer > 0)
                return false;
            
            if (!SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.None))
                return false;
            
            if (Core.Me.SummonedPet() != SmnPets.Carbuncle)
                return false;

            if (Combat.CombatTotalTimeLeft < 15)
                return false;

            return await Spells.DreadwyrmTrance.Cast(Core.Me);
        }

        public static async Task<bool> LucidDreaming()
        {
            if (!Spells.LucidDreaming.IsKnownAndReady())
                return false;

            if (Core.Me.CurrentManaPercent > SummonerSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (!GlobalCooldown.CanWeave())
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
        
        public static async Task<bool> Aethercharge()
        {
            if (await Pets.SummonPhoenix()) return true;
            if (await Pets.SummonBahamut()) return true;

            if (Spells.SummonBahamut.IsKnown())
                return false;
            
            if (await DreadwyrmTrance()) return true;
            
            if (Spells.DreadwyrmTrance.IsKnown())
                return false;

            if (!SummonerSettings.Instance.Aethercharge)
                return false;
            
            return await Spells.Aethercharge.Cast(Core.Me);
        }

        public static async Task<bool> SearingLight()
        {
            return await Spells.SearingLight.CastAura(Core.Me, Auras.SearingLight);
        }
    }
}