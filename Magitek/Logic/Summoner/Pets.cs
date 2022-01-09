using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using Magitek.Utilities.Routines;
using System;
using System.Threading.Tasks;
using ArcResources = ff14bot.Managers.ActionResourceManager.Arcanist;
using SmnResources = ff14bot.Managers.ActionResourceManager.Summoner;
using static Magitek.Utilities.Routines.Summoner;


namespace Magitek.Logic.Summoner
{
    internal static class Pets
    {

        public static async Task<bool> SummonCarbuncle()
        {
            if (!SummonerSettings.Instance.SummonCarbuncle)
                return false;
            
            if (!Spells.SummonCarbuncle.IsKnown())
                return false;

            if (Core.Me.IsMounted || MovementManager.IsMoving || MovementManager.IsOccupied)
                return false;

            if (Core.Me.SummonedPet() != SmnPets.None)
                return false;

            return await Spells.SummonCarbuncle.Cast(Core.Me);
        }

        public static async Task<bool> SummonPhoenix()
        {
            if (!SummonerSettings.Instance.SummonPhoenix)
                return false;
            
            if (!Spells.SummonPhoenix.IsKnownAndReady())
                return false;
            
            if (!SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Phoenix))
                return false;
            
            if (!Core.Me.InCombat)
                return false;
            
            if (SmnResources.PetTimer + SmnResources.TranceTimer > 0)
                return false;

            if (SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Ifrit)
                || SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Titan)
                || SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Garuda)
                || ArcResources.AvailablePets.HasFlag(ArcResources.AvailablePetFlags.Ruby)
                || ArcResources.AvailablePets.HasFlag(ArcResources.AvailablePetFlags.Topaz)
                || ArcResources.AvailablePets.HasFlag(ArcResources.AvailablePetFlags.Emerald))
                return false;

            if ((SmnResources.PetTimer + SmnResources.TranceTimer) > 0)
                return false;

            return await Spells.SummonPhoenix.Cast(Core.Me);
        }

        public static async Task<bool> SummonBahamut()
        {
            if (!SummonerSettings.Instance.SummonBahamut)
                return false;
            
            if (!Spells.SummonBahamut.IsKnownAndReady())
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Phoenix))
                return false;
            
            if ((SmnResources.PetTimer + SmnResources.TranceTimer) > 0)
                return false;

            if (!SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.None))
                return false;
            
            if (Core.Me.SummonedPet() != SmnPets.Carbuncle)
                return false;

            if (Combat.CombatTotalTimeLeft < 15)
                return false;

            if (!SummonerSettings.Instance.SearingLight)
                return await Spells.SummonBahamut.Cast(Core.Me);

            if (!Spells.SearingLight.IsReady() && !Core.Me.HasAura(Auras.SearingLight))
                return await Spells.SummonBahamut.Cast(Core.Me);

            if (Spells.SearingLight.IsReady() && GlobalCooldown.CanWeave())
                return await Buff.SearingLight();

            if (!Core.Me.HasAura(Auras.SearingLight))
                return false;

            return await Spells.SummonBahamut.Cast(Core.Me);
        }

        public static async Task<bool> SummonCarbuncleOrEgi()
        {
            if (Core.Me.SummonedPet() == SmnPets.None)
                return await SummonCarbuncle();

            if (!Core.Me.InCombat)
                return false;

            if ((SmnResources.PetTimer + SmnResources.TranceTimer) > 0)
                return false;
            
            if (Combat.CombatTotalTimeLeft < 30)
                return false;
            
            if (SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.None))
                return false;
            
            if (SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Titan) &&
                Spells.SummonTitan.IsKnownAndReady())
                return Spells.SummonTitan2.IsKnown()
                    ? await Spells.SummonTitan2.Cast(Core.Me)
                    : await Spells.SummonTitan.Cast(Core.Me);

            if (ArcResources.AvailablePets.HasFlag(ArcResources.AvailablePetFlags.Topaz))
                return await Spells.SummonTopaz.Cast(Core.Me);

            if (SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Garuda) &&
                Spells.SummonGaruda.IsKnownAndReady())
                return Spells.SummonGaruda2.IsKnown()
                    ? await Spells.SummonGaruda2.Cast(Core.Me)
                    : await Spells.SummonGaruda.Cast(Core.Me);

            if (ArcResources.AvailablePets.HasFlag(ArcResources.AvailablePetFlags.Emerald))
                return await Spells.SummonEmerald.Cast(Core.Me);
            
            if (SmnResources.AvailablePets.HasFlag(SmnResources.AvailablePetFlags.Ifrit) &&
                Spells.SummonIfrit.IsKnownAndReady())
                return Spells.SummonIfrit2.IsKnown()
                    ? await Spells.SummonIfrit2.Cast(Core.Me)
                    : await Spells.SummonIfrit.Cast(Core.Me);

            if (ArcResources.AvailablePets.HasFlag(ArcResources.AvailablePetFlags.Ruby))
                return await Spells.SummonRuby.Cast(Core.Me);

            return false;
        } 
    }
}