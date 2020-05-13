/*using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

namespace Magitek.Logic.WhiteMage
{
    internal static class TankBusters
    {
        public static async Task<bool> Execute()
        {
            if (!WhiteMageSettings.Instance.UseTankBusters)
                return false;

            if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (await DivineBenison()) return true;
            if (await Cure2()) return true;
            if (await Medica()) return true;
            if (await Medica2()) return true;
            return await AfflatusRapture();
        }

        private static async Task<bool> Cure2()
        {           
            var cure2Target = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.Cure2List.Contains(r.CastingSpellId))?.TargetCharacter;

            if (cure2Target == null)
                return false;

            if (!await Spells.Cure2.Heal(cure2Target))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }
        
        private static async Task<bool> Medica()
        {
        
            if (Casting.LastSpell == Spells.Medica && Casting.LastSpellTarget == Core.Me && DateTime.Now < Casting.LastTankBusterTime.AddSeconds(5))
                return false;

            var medica = Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.MedicaList.Contains(r.CastingSpellId));

            if (!medica)
                return false;

            if (!await Spells.Medica.Heal(Core.Me))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }
        
        private static async Task<bool> Medica2()
        {           
            var enemyCasting = Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.Medica2List.Contains(r.CastingSpellId));

            if (!enemyCasting)
                return false;

            if (Group.CastableAlliesWithin15.Count(r => r.HasAura(Auras.Medica2)) < WhiteMageSettings.Instance.Medica2Allies)
            {
                return false;
            }

            if (!await Spells.Medica2.Heal(Core.Me, false))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }

        private static async Task<bool> DivineBenison()
        {
            var divineBenisonTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.DivineBenison.Contains(r.CastingSpellId) && r.HasTarget &&
                                                                         !r.TargetGameObject.HasAura(Auras.DivineBenison) && !r.TargetGameObject.HasAura(Auras.DivineBenison2))?.TargetCharacter;

            if (divineBenisonTarget == null)
                return false;

            if (!await Spells.DivineBenison.Cast(divineBenisonTarget))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }

        private static async Task<bool> AfflatusRapture()
        {
            if (Casting.LastSpell == Spells.AfflatusRapture&& Casting.LastSpellTarget == Core.Me && DateTime.Now < Casting.LastTankBusterTime.AddSeconds(5))
                return false;

            var AfflatusRapture= Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.AfflatusRaptureList.Contains(r.CastingSpellId));

            if (!AfflatusRapture)
                return false;

            if (!await Spells.AfflatusRapture.Heal(Core.Me))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }
    }
}*/
