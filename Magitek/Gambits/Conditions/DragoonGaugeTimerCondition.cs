using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class DragoonGaugeTimerCondition : GambitCondition
    {
        public DragoonGaugeTimerCondition() : base(GambitConditionTypes.DragoonGaugeTimer)
        {
        }

        public int GaugeMinimum { get; set; }
        public int GaugeMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Dragoon.Timer.Seconds < GaugeMinimum)
                return false;

            if (ActionResourceManager.Dragoon.Timer.Seconds > GaugeMaximum)
                return false;

            return true;
        }
    }
}
