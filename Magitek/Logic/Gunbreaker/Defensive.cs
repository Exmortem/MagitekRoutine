using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Defensive
    {
        public static async Task<bool> ExecuteTankBusters()
        {
            if (!GunbreakerSettings.Instance.UseTankBusters)
                return false;

            var targetAsCharacter = (Core.Me.CurrentTarget as Character);

            if (targetAsCharacter == null)
                return false;

            var castingSpell = targetAsCharacter.CastingSpellId;
            var targetIsMe = targetAsCharacter.TargetGameObject == Core.Me;

            if (!TankBusterManager.GunbreakerTankBusters.ContainsKey(castingSpell))
                return false;

            var tankBuster = TankBusterManager.GunbreakerTankBusters.First(r => r.Key == castingSpell).Value;

            if (tankBuster == null)
                return false;

            if (tankBuster.HeartofStone && await Spells.Aurora.CastAura(Core.Me.CurrentTarget, Auras.Aurora)) return true;
            if (tankBuster.ReprisalGnb && targetIsMe && await Spells.Reprisal.CastAura(Core.Me, Auras.Reprisal)) return true;
            if (tankBuster.Camouflage && targetIsMe && await Spells.Camouflage.CastAura(Core.Me, Auras.Camouflage)) return true;
            if (tankBuster.HeartofLight && await Spells.Aurora.CastAura(Core.Me.CurrentTarget, Auras.Aurora)) return true;
            if (tankBuster.RampartGnb && targetIsMe && await Spells.Rampart.CastAura(Core.Me, Auras.Rampart)) return true;
            if (tankBuster.Nebula && targetIsMe && await Spells.Nebula.CastAura(Core.Me, Auras.Nebula)) return true;
            if (tankBuster.Aurora && await Spells.Aurora.CastAura(Core.Me.CurrentTarget, Auras.Aurora)) return true;
            return tankBuster.Superbolide && targetIsMe && await Spells.Superbolide.CastAura(Core.Me, Auras.Superbolide);
        }
      
        public static async Task<bool> Execute()
        {
            if (!GunbreakerSettings.Instance.UseDefensives)
                return false;
          
            if (GunbreakerSettings.Instance.UseDefensivesOnlyOnTankBusters)
                return false;
          
            if (Core.Me.HasAura(Auras.Superbolide))
                return false;

            if (await Superbolide()) return true;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => Utilities.Routines.Gunbreaker.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= GunbreakerSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= GunbreakerSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= GunbreakerSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            if (await HeartofStone()) return true;
            if (await Reprisal()) return true;
            if (await Camouflage()) return true;
            if (await HeartofLight()) return true;
            if (await Rampart()) return true;
            if (await Nebula()) return true;
            return await Aurora();
        }
      
        private static async Task<bool> Superbolide()
        {
            if (!GunbreakerSettings.Instance.UseSuperbolide)
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.SuperbolideHealthPercent)
                return false;
          
            return await Spells.Superbolide.CastAura(Core.Me, Auras.Superbolide);
        }

        private static async Task<bool> Rampart()
        {
            if (!GunbreakerSettings.Instance.UseRampart)
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.RampartHpPercentage)
                return false;

            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }

        private static async Task<bool> Reprisal()
        {
            if (!GunbreakerSettings.Instance.UseReprisal)
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.ReprisalHealthPercent)
                return false;

            return await Spells.Reprisal.CastAura(Core.Me.CurrentTarget, Auras.Reprisal);
        }

        private static async Task<bool> Camouflage()
        {
            if (!GunbreakerSettings.Instance.UseCamouflage)
                return false;

            if (!ActionManager.HasSpell(Spells.Camouflage.Id))
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.CamouflageHealthPercent)
                return false;

            return await Spells.Camouflage.CastAura(Core.Me, Auras.Camouflage);
        }

        private static async Task<bool> Nebula()
        {
            if (!GunbreakerSettings.Instance.UseNebula)
                return false;

            if (!ActionManager.HasSpell(Spells.Nebula.Id))
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.NebulaHealthPercent)
                return false;

            return await Spells.Nebula.CastAura(Core.Me, Auras.Nebula);
        }

        private static async Task<bool> Aurora()
        {
            if (!GunbreakerSettings.Instance.UseAurora)
                return false;

            if (!ActionManager.HasSpell(Spells.Aurora.Id))
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.AuroraAsDefensiveHealthPercent)
                return false;

            return await Spells.Aurora.CastAura(Core.Me, Auras.Aurora);
        }

        private static async Task<bool> HeartofLight()
        {
            if (!GunbreakerSettings.Instance.UseHeartofLight)
                return false;

            if (!ActionManager.HasSpell(Spells.HeartofLight.Id))
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.HeartofLightHealthPercent)
                return false;

            return await Spells.HeartofLight.CastAura(Core.Me, Auras.HeartofLight);
        }

        private static async Task<bool> HeartofStone()
        {
            if (!GunbreakerSettings.Instance.UseHeartofStone)
                return false;

            if (!ActionManager.HasSpell(Spells.HeartofStone.Id))
                return false;

            if (Core.Me.CurrentHealthPercent > GunbreakerSettings.Instance.HeartofStoneHealthPercent)
                return false;

            return await Spells.HeartofStone.CastAura(Core.Me, Auras.HeartofStone);
        }
    }
}