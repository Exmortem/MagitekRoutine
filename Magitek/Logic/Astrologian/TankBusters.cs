using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

namespace Magitek.Logic.Astrologian
{
    internal static class TankBusters
    {
        public static async Task<bool> Execute()
        {
            if (!AstrologianSettings.Instance.UseTankBusters)
                return false;

            if (Core.Me.CurrentManaPercent < AstrologianSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (await Benefic2()) return true;
            if (await Helios()) return true;
            if (await AspectedHelios()) return true;
            return await AspectedBenefic();
        }

        private static async Task<bool> Benefic2()
        {           
            var benefic2Target = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.Benefic2List.Contains(r.CastingSpellId))?.TargetCharacter;

            if (benefic2Target == null)
                return false;

            if (!await Spells.Benefic2.Heal(benefic2Target))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }

        private static async Task<bool> Helios()
        {
            if (Casting.LastSpell == Spells.Helios && Casting.LastSpellTarget == Core.Me &&
                DateTime.Now < Casting.LastTankBusterTime.AddSeconds(5))
                return false;

            var helios = Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.HeliosList.Contains(r.CastingSpellId));

            if (!helios)
                return false;

            if (!await Spells.Helios.Heal(Core.Me))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }
        
        private static async Task<bool> AspectedHelios()
        {           
            var enemyCasting = Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.AspectedHeliosList.Contains(r.CastingSpellId));

            if (!enemyCasting)
                return false;

            if (Core.Me.Sect() == AstrologianSect.Diurnal && Group.CastableAlliesWithin15.Count(r => r.HasAura(Auras.AspectedHelios)) < AstrologianSettings.Instance.DiurnalHeliosAllies)
            {
                return false;
            }

            if (Core.Me.Sect() == AstrologianSect.Nocturnal && Group.CastableAlliesWithin15.Count(r => r.HasAura(Auras.NocturnalField)) < AstrologianSettings.Instance.NocturnalHeliosAllies)
            {
                return false;
            }

            if (!await Spells.AspectedHelios.Heal(Core.Me, false))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }

        private static async Task<bool> AspectedBenefic()
        {
            var aspectedbeneficTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.AspectedBeneficList.Contains(r.CastingSpellId))?.TargetCharacter;

            if (aspectedbeneficTarget == null)
                return false;

            if (!await Spells.AspectedBenefic.Cast(aspectedbeneficTarget))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }
    }
}