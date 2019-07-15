using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class PlayerNotInCombatCondition : GambitCondition
    {
        public PlayerNotInCombatCondition() : base(GambitConditionTypes.PlayerNotInCombat)
        {
        }

        public override bool Check(GameObject gameObject = null)
        {
            return !Core.Me.InCombat;
        }
    }
}
