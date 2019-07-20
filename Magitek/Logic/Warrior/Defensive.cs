using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

namespace Magitek.Logic.Warrior
{
    public class Defensive
    {
        public static async Task<bool> ExecuteTankBusters()
        {
            if (!WarriorSettings.Instance.UseTankBusters)
                return false;

            var targetAsCharacter = Core.Me.CurrentTarget as Character;

            if (targetAsCharacter == null)
                return false;

            var castingSpell = targetAsCharacter.CastingSpellId;
            var targetIsMe = targetAsCharacter.TargetGameObject == Core.Me;

            if (!TankBusterManager.WarriorTankBusters.ContainsKey(castingSpell))
                return false;

            var tankBuster = TankBusterManager.WarriorTankBusters.First(r => r.Key == castingSpell).Value;

            if (tankBuster == null)
                return false;

			if (tankBuster.RawIntuition && targetIsMe && await Spells.RawIntuition.CastAura(Core.Me, Utilities.Auras.RawIntuition)) return true;
			if (tankBuster.ReprisalWar && await Spells.Reprisal.CastAura(Core.Me.CurrentTarget, Utilities.Auras.Reprisal)) return true;
			if (tankBuster.RampartWar && targetIsMe && await Spells.Rampart.CastAura(Core.Me, Utilities.Auras.Rampart)) ;
			if (tankBuster.Vengeance && targetIsMe && await Spells.Vengeance.CastAura(Core.Me, Utilities.Auras.Vengeance)) return true;
			return (tankBuster.Holmgang && targetIsMe && await Spells.Holmgang.CastAura(Core.Me.CurrentTarget, Utilities.Auras.Holmgang));
		}

        public static async Task<bool> Defensives()
        {
            if (!WarriorSettings.Instance.UseDefensives)
                return false;

            if (WarriorSettings.Instance.UseDefensivesOnlyOnTankBusters)
                return false;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => Utilities.Routines.Warrior.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= WarriorSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= WarriorSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= WarriorSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            #region Holmgang
            if (WarriorSettings.Instance.UseHolmgang)
            {
                if (Core.Me.CurrentHealthPercent <= WarriorSettings.Instance.HolmgangHpPercentage)
                {
                    if (await Spells.Holmgang.CastAura(Core.Me.CurrentTarget, Utilities.Auras.Holmgang)) return true;
                }
            }
            #endregion

            #region Vengeance
            if (WarriorSettings.Instance.UseVengeance)
            {
                if (Core.Me.CurrentHealthPercent <= WarriorSettings.Instance.VengeanceHpPercentage)
                {
                    if (await Spells.Vengeance.CastAura(Core.Me, Utilities.Auras.Vengeance)) return true;
                }
            }
            #endregion

            if (await Tank.Rampart(WarriorSettings.Instance)) return true;

            #region ShakeItOff
            if (WarriorSettings.Instance.UseShakeItOff)
            {
                if (Core.Me.Auras.Any(x => x.IsDispellable))
                {
                    if (await Spells.ShakeItOff.Cast(Core.Me)) return true;
                }
            }
            #endregion
            
            #region RawIntuition
            if (WarriorSettings.Instance.UseRawIntuition)
            {
                if (Core.Me.CurrentHealthPercent <= WarriorSettings.Instance.RawIntuitionHpPercentage)
                {
                    if (await Spells.RawIntuition.CastAura(Core.Me, Utilities.Auras.RawIntuition)) return true;
                }
            }
            #endregion
            
            return false;
        }

        public static async Task<bool> Reprisal()
        {
            if (!WarriorSettings.Instance.UseReprisal) return false;
            if (Core.Me.CurrentTarget == null) return false;

            return await Spells.Reprisal.Cast(Core.Me.CurrentTarget);
        }
    }
}