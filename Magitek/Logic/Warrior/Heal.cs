using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;

namespace Magitek.Logic.Warrior
{
    internal static class Heal
    {
        private static bool UseHeal()
        {
            if (!WarriorSettings.Instance.UseHeal)
                return false;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => WarriorRoutine.Heal.Contains(r)).ToList();

            if (currentAuras.Count >= WarriorSettings.Instance.MaxHealAtOnce)
            {
                if (currentAuras.Count >= WarriorSettings.Instance.MaxHealUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= WarriorSettings.Instance.MoreHealHp)
                    return false;
            }

            return true;
        }

        public static async Task<bool> Equilibrium()
        {
            if (!WarriorSettings.Instance.UseEquilibrium)
                return false;

            if (!UseHeal())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.EquilibriumHealthPercent)
                return false;

            return await Spells.Equilibrium.Cast(Core.Me);
        }

        public static async Task<bool> ThrillOfBattle()
        {
            if (!WarriorSettings.Instance.UseThrillOfBattle)
                return false;

            if (!UseHeal())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.ThrillOfBattleHpPercentage)
                return false;

            return await Spells.ThrillofBattle.CastAura(Core.Me, Auras.ThrillOfBattle);
        }
    }
}
