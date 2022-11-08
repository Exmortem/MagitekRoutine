using ff14bot;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using WarriorRoutine = Magitek.Utilities.Routines.Warrior;

namespace Magitek.Logic.Warrior
{
    public class Defensive
    {
        private static bool UseDefensives()
        {
            if (!WarriorSettings.Instance.UseDefensives)
                return false;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => WarriorRoutine.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= WarriorSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= WarriorSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= WarriorSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            return true;
        }

        public static async Task<bool> Holmgang()
        {
            if (!WarriorSettings.Instance.UseHolmgang)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.HolmgangHpPercentage)
                return false;

            return await Spells.Holmgang.CastAura(Core.Me.CurrentTarget, Utilities.Auras.Holmgang);
        }

        public static async Task<bool> Reprisal()
        {
            if (!WarriorSettings.Instance.UseReprisal)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.ReprisalHealthPercent)
                return false;

            return await Spells.Reprisal.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Rampart()
        {
            if (!WarriorSettings.Instance.UseRampart)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.RampartHpPercentage)
                return false;

            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }

        public static async Task<bool> Vengeance()
        {
            if (!WarriorSettings.Instance.UseVengeance)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.VengeanceHpPercentage)
                return false;

            return await Spells.Vengeance.CastAura(Core.Me, Auras.Vengeance);
        }

        public static async Task<bool> ShakeItOff()
        {
            if (!WarriorSettings.Instance.UseShakeItOff)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.ShakeItOffHpPercentage)
                return false;

            return await Spells.ShakeItOff.Cast(Core.Me);
        }

        public static async Task<bool> BloodWhetting()
        {
            if (!WarriorSettings.Instance.UseBloodWhetting)
                return false;

            if (Core.Me.CurrentHealthPercent > WarriorSettings.Instance.BloodWhettingHpPercentage)
                return false;

            // The CastAura below correctly handles casting either Raw Intuition or Bloodwhetting,
            // but we need to pass in the correct aura id in the second parameter otherwise the
            // routine will "hang" for 6 seconds waiting on the wrong aura.
            var aura = Spells.Bloodwhetting.IsKnown()
                ? Auras.Bloodwhetting
                : Auras.RawIntuition;

            return await WarriorRoutine.Bloodwhetting.CastAura(Core.Me, (uint)aura);
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return Tank.ForceLimitBreak(Spells.ShieldWall, Spells.Stronghold, Spells.LandWaker, Spells.HeavySwing);
        }

    }
}