using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using GunbreakerRoutine = Magitek.Utilities.Routines.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Defensive
    {
        private static bool UseDefensives()
        {
            if (!GunbreakerSettings.Instance.UseDefensives)
                return false;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => Utilities.Routines.Gunbreaker.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= GunbreakerSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= GunbreakerSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= GunbreakerSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            return true;
        }

        public static async Task<bool> Superbolide()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseSuperbolide, Spells.Superbolide))
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.SuperbolideHealthPercent)
                return false;

            return await Spells.Superbolide.CastAura(Core.Me, Auras.Superbolide);
        }

        public static async Task<bool> Rampart()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseRampart, Spells.Rampart))
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.RampartHpPercentage)
                return false;

            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }

        public static async Task<bool> Reprisal()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseReprisal, Spells.Reprisal))
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.ReprisalHealthPercent)
                return false;

            return await Spells.Reprisal.CastAura(Core.Me.CurrentTarget, Auras.Reprisal);
        }

        public static async Task<bool> Camouflage()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseCamouflage, Spells.Camouflage))
                return false;
            
            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.CamouflageHealthPercent)
                return false;

            return await Spells.Camouflage.CastAura(Core.Me, Auras.Camouflage);
        }

        public static async Task<bool> Nebula()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseNebula, Spells.Nebula))
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.NebulaHealthPercent)
                return false;

            return await Spells.Nebula.CastAura(Core.Me, Auras.Nebula);
        }

        public static async Task<bool> HeartofLight()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseHeartofLight, Spells.HeartofLight))
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.HeartofLightHealthPercent)
                return false;

            return await Spells.HeartofLight.CastAura(Core.Me, Auras.HeartofLight);
        }

        public static async Task<bool> HeartofCorundum()
        {
            if (!GunbreakerRoutine.ToggleAndSpellCheck(GunbreakerSettings.Instance.UseHeartofCorundum, Spells.HeartOfCorundum))
                return false; 

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.HeartofCorundumHealthPercent)
                return false;

            return await GunbreakerRoutine.HeartOfCorundum.CastAura(Core.Me, Auras.HeartOfCorundum);
        }
    }
}