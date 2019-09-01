using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class MpPercentAboveCondition : GambitCondition
    {
        public MpPercentAboveCondition() : base(GambitConditionTypes.MpAboveCondition)
        {
        }

        public int MPValue { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            return Core.Me.CurrentManaPercent <= MPValue;
        }
    }
}