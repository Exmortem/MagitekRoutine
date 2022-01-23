using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Reaper;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Logic.Reaper
{
    internal static class Buff
    {
        public static async Task<bool> FightLogic_ArcaneCrest()
        {
            if (!Spells.ArcaneCrest.IsKnownAndReady())
                return false;

            if (!FightLogic.ZoneHasFightLogic() || !FightLogic.EnemyHasAnyAoeLogic())
                return false;

            if (FightLogic.EnemyIsCastingAoe() || FightLogic.EnemyIsCastingBigAoe())
                return await FightLogic.DoAndBuffer(Spells.ArcaneCrest.CastAura(Core.Me, Auras.CrestOfTimeBorrowed));

            return false;
        }

        public static async Task<bool> FightLogic_Feint()
        {
            if (!Spells.Feint.IsKnownAndReady())
                return false;

            if (!FightLogic.ZoneHasFightLogic())
                return false;

            if (FightLogic.EnemyIsCastingAoe()
                || FightLogic.EnemyIsCastingBigAoe()
                || FightLogic.EnemyIsCastingTankBuster() != null
                || FightLogic.EnemyIsCastingSharedTankBuster() != null)
                return await FightLogic.DoAndBuffer(Spells.Feint.CastAura(Core.Me.CurrentTarget, Auras.Feint));

            return false;
        }
    }
}