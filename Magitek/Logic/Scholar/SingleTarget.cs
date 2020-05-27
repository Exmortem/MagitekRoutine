using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Scholar
{
    internal static class SingleTarget
    {
        public static async Task<bool> Broil()
        {
            if (!ScholarSettings.Instance.RuinBroil)
                return false;

            return await Spells.Ruin.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Ruin2()
        {
            if (!ScholarSettings.Instance.Ruin2)
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast))
                return false;

            if (!MovementManager.IsMoving)
                return false;

            return await Spells.Ruin2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BioMultipleTargets()
        {
            if (!ScholarSettings.Instance.Bio)
                return false;

            if (!ScholarSettings.Instance.BioMultipleTargets)
                return false;

            var bioTarget = Combat.Enemies.FirstOrDefault(NeedsBio);

            if (bioTarget == null)
                return false;

            return await Spells.Bio.Cast(bioTarget);

            bool NeedsBio(BattleCharacter unit)
            {
                if (!CanBio(unit))
                    return false;

                if (Core.Me.ClassLevel < 26)
                    return !unit.HasAura(Auras.Bio, true, ScholarSettings.Instance.BioRefreshSeconds * 1000);

                if (Core.Me.ClassLevel < 72)
                    return !unit.HasAura(Auras.Bio2, true, ScholarSettings.Instance.BioRefreshSeconds * 1000);

                return !unit.HasAura(Auras.Biolysis, true, ScholarSettings.Instance.BioRefreshSeconds * 1000);
            }

            bool CanBio(GameObject unit)
            {
                if (!ScholarSettings.Instance.BioUseTimeTillDeath)
                    return true;

                return unit.CombatTimeLeft() >= ScholarSettings.Instance.BioDontIfEnemyDyingWithinSeconds;
            }
        }

        public static async Task<bool> Bio()
        {
            if (!ScholarSettings.Instance.Bio)
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(BioAuras, true, 4000))
                return false;

            return await Spells.Bio.Cast(Core.Me.CurrentTarget);
        }

        private static readonly uint[] BioAuras =
        {
            Auras.Bio,
            Auras.Bio2,
            Auras.Biolysis
        };
        public static async Task<bool> EnergyDrain2()
        {
            if (!ScholarSettings.Instance.EnergyDrain)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (ActionResourceManager.Scholar.Aetherflow == 3 && Spells.Aetherflow.Cooldown.TotalMilliseconds > 9000)
                return false;
            if (ActionResourceManager.Scholar.Aetherflow == 2 && Spells.Aetherflow.Cooldown.TotalMilliseconds > 6000)
                return false;
            if (ActionResourceManager.Scholar.Aetherflow == 1 && Spells.Aetherflow.Cooldown.TotalMilliseconds > 3000)
                return false;
            if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    return true;
            return await Spells.EnergyDrain2.Cast(Core.Me.CurrentTarget);
        }
    }
}
