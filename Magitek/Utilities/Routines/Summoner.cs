using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Logic.Summoner;
using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    internal static class Summoner
    {
        public static bool OnGcd => Spells.Ruin.Cooldown.TotalMilliseconds > 100;

        public static uint[] BioAuras = { Auras.Bio, Auras.Bio2, Auras.Bio3 };
        public static uint[] MiasmaAuras = { Auras.Miasma, Auras.Miasma3 };
        public static HashSet<int> DemiSummonIds = new HashSet<int> { 10, 14 };
        
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Summoner, Spells.Ruin);

        public static bool NeedToInterruptCast()
        {

            if (Casting.CastingSpell == Spells.Resurrection && Casting.SpellTarget?.CurrentHealth > 1)
            {
                Logger.Error("Stopped Resurrection: Unit is now alive");
                return true;
            }
            
            return false;
        }

        public enum SmnPets
        {
            None,
            Carbuncle,
            Ruby,
            Topaz,
            Emerald,
            Ifrit,
            Titan,
            Garuda,
            Bahamut,
            Pheonix
        }


        public static SmnPets SummonedPet(this LocalPlayer me)
        {
            if ((int) PetManager.ActivePetType == 10)
                return SmnPets.Bahamut;
            
            if ((int) PetManager.ActivePetType == 14)
                return SmnPets.Pheonix;
            
            if ((int) PetManager.ActivePetType == 23)
                return SmnPets.Carbuncle;
            
            if ((int) PetManager.ActivePetType == 24)
                return SmnPets.Ruby;
            
            if ((int) PetManager.ActivePetType == 25)
                return SmnPets.Topaz;
            
            if ((int) PetManager.ActivePetType == 26)
                return SmnPets.Emerald;
            
            if ((int) PetManager.ActivePetType == 30)
                return SmnPets.Ifrit;
            
            if ((int) PetManager.ActivePetType == 31)
                return SmnPets.Titan;
            
            if ((int) PetManager.ActivePetType == 32)
                return SmnPets.Garuda;

            return GameObjectManager.PetObjectId != GameObjectManager.EmptyGameObject
                ? SmnPets.Carbuncle 
                : SmnPets.None;
        }

    }
}
