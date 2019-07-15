using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class NinjaNinkiCondition : GambitCondition
    {
        public NinjaNinkiCondition() : base(GambitConditionTypes.NinjaNinki)
        {
        }

        public int NinkiMinimum { get; set; }
        public int NinkiMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Ninja.NinkiGauge < NinkiMinimum)
                return false;

            if (ActionResourceManager.Ninja.NinkiGauge > NinkiMaximum)
                return false;

            return true;
        }
    }
}
