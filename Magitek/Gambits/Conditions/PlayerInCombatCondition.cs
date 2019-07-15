using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class PlayerInCombatCondition : GambitCondition
    {
        public PlayerInCombatCondition() : base(GambitConditionTypes.PlayerInCombat)
        {
        }

        public override bool Check(GameObject gameObject = null)
        {
            return Core.Me.InCombat;
        }
    }
}
