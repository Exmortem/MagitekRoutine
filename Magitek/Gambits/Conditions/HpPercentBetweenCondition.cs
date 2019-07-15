using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class HpPercentBetweenCondition : GambitCondition
    {
        public HpPercentBetweenCondition() : base(GambitConditionTypes.HpPercentBetween)
        {
        }

        public int HpPercentMinimum { get; set; }
        public int HpPercentMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            return gameObject?.CurrentHealthPercent >= HpPercentMinimum &&
                   gameObject?.CurrentHealthPercent <= HpPercentMaximum;
        }
    }
}