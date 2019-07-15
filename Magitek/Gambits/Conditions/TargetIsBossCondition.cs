using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;

namespace Magitek.Gambits.Conditions
{
    public class TargetIsBossCondition : GambitCondition
    {
        public TargetIsBossCondition() : base(GambitConditionTypes.TargetIsBoss)
        {
        }

        public override bool Check(GameObject gameObject = null)
        {
            return Core.Me.CurrentTarget.IsBoss();
        }
    }
}
