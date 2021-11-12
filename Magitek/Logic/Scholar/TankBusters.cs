/*using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Magitek.Utilities.Managers;

namespace Magitek.Logic.Scholar
{
    internal static class TankBusters
    {
        public static async Task<bool> Execute()
        {
            if (!ScholarSettings.Instance.UseTankBusters)
                return false;

            if (Core.Me.CurrentManaPercent < ScholarSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (await Excogitation()) return true;
            if (await Adloquium()) return true;
            return await Succor();
        }

        private static async Task<bool> Adloquium()
        {
            var castingTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting);

            if (castingTarget == null)
                return false;

            var castingInfo = Combat.Enemies.FirstOrDefault(r => r.IsCasting).SpellCastInfo;
            /*
            if (!TankBusterManager.AdloquiumList.ContainsKey(castingInfo.ActionId))
                return false;

            var adloquiumTankBuster = TankBusterManager.AdloquiumList.FirstOrDefault(r => r.Key == castingInfo.ActionId);
            
            if (castingInfo.CurrentCastTime.TotalMilliseconds < adloquiumTankBuster.Value)
                return false;

            if (castingTarget.HasAura(Auras.Galvanize))
                return false;

            if (!await Spells.Adloquium.HealAura(castingTarget, Auras.Galvanize, false, true, true, 20000))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }

        private static async Task<bool> Excogitation()
        {
            var execogitationTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.ExcogitationList.ContainsKey(r.CastingSpellId))?.TargetCharacter;

            if (execogitationTarget == null)
                return false;

            if (execogitationTarget.HasAura(Auras.Exogitation))
                return false;

            if (!await Spells.Excogitation.HealAura(execogitationTarget, Auras.Exogitation, false, true, true, 20000))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }

        private static async Task<bool> Succor()
        {
            var castingTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting);

            if (castingTarget == null)
                return false;

            var castingInfo = castingTarget.SpellCastInfo;

            if (!TankBusterManager.SuccorList.ContainsKey(castingInfo.ActionId))
                return false;

            var succorTankBuster = TankBusterManager.SuccorList.FirstOrDefault(r => r.Key == castingInfo.ActionId);

            if (castingInfo.CurrentCastTime.TotalMilliseconds < succorTankBuster.Value)
                return false;

            if (Group.CastableAlliesWithin15.All(r => r.HasAura(Auras.Galvanize)))
                return false;

            if (!await Spells.Succor.Heal(Core.Me, false))
                return false;

            Casting.CastingTankBuster = true;
            return true;
        }
    }
}*/
