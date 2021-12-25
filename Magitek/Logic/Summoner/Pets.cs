using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Logic.Summoner
{
    internal static class Pets
    {
        public static async Task<bool> Summon()
        {
            if (Core.Me.ClassLevel < 4)
                return false;

            // Don't try to summon if we're mounted or moving
            if (Core.Me.IsMounted || MovementManager.IsMoving)
                return false;

            if ((int)PetManager.ActivePetType == (int)SummonerSettings.Instance.SelectedPet) return false;

            if (Core.Me.ClassLevel < 30 || Core.Me.CurrentJob == ClassJobType.Arcanist)
            {
                switch (SummonerSettings.Instance.SelectedPet)
                {
                    case SummonerPets.None:
                        return false;
                    case SummonerPets.EmeraldCarbuncle:
                        return await Spells.Summon.Cast(Core.Me);
                    case SummonerPets.TopazCarbuncle:
                        return await Spells.Summon2.Cast(Core.Me);
                    default:
                        if (PetManager.ActivePetType != PetType.None)
                            return false;
                        else
                            return await Spells.Summon.Cast(Core.Me);
                }
            }

            switch (SummonerSettings.Instance.SelectedPet)
            {
                case SummonerPets.None:
                    return false;
                case SummonerPets.EmeraldCarbuncle:
                case SummonerPets.Garuda:
                    return await Spells.Summon.Cast(Core.Me);
                case SummonerPets.TopazCarbuncle:
                case SummonerPets.Titan:
                    return await Spells.Summon2.Cast(Core.Me);
                case SummonerPets.Ifrit:
                    if (Core.Me.ClassLevel < 30 || Core.Me.CurrentJob == ClassJobType.Arcanist)
                        return await Spells.Summon.Cast(Core.Me);
                    return await Spells.Summon3.Cast(Core.Me);


                default:
                    return false;
            }
        }

        public static async Task<bool> SummonBahamut()
        {
            if (Core.Me.ClassLevel < 70) return false;

            if (!SummonerSettings.Instance.SummonBahamut)
                return false;

            if (Core.Me.Pet == null) return false;

            if (Casting.LastSpell == Spells.Deathflare)
                return await Spells.SummonBahamut.Cast(Core.Me);

            if (Spells.Deathflare.Cooldown.TotalSeconds > 8 && Spells.SummonBahamut.Cooldown.TotalSeconds == 0)
                return await Spells.SummonBahamut.Cast(Core.Me);

            return false;
        }
    }
}