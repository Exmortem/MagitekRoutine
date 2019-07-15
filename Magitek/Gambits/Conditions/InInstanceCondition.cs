using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class InInstanceCondition : GambitCondition
    {
        public InInstanceCondition() : base(GambitConditionTypes.InInstance)
        {
        }

        public override bool Check(GameObject gameObject = null)
        {
            return DutyManager.InInstance;
        }
    }
}