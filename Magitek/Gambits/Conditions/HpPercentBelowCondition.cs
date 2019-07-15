using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class HpPercentBelowCondition : GambitCondition
    {
        public HpPercentBelowCondition() : base(GambitConditionTypes.HpPercentBelow)
        {
        }

        public int HpPercentage { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            return gameObject?.CurrentHealthPercent <= HpPercentage;
        }
    }
}