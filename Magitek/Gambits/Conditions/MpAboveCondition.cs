using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class MpAboveCondition : GambitCondition
    {
        public MpAboveCondition() : base(GambitConditionTypes.MpAbove)
        {
        }

        public int MpPercentage { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            return Core.Me.CurrentMana <= MpPercentage;
        }
    }
}