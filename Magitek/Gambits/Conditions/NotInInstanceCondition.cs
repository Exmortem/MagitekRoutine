using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class NotInInstanceCondition : GambitCondition
    {
        public NotInInstanceCondition() : base(GambitConditionTypes.NotInInstance)
        {
        }

        public override bool Check(GameObject gameObject = null)
        {
            return !DutyManager.InInstance;
        }
    }
}