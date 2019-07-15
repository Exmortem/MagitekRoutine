using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class MachinistHeatCondition : GambitCondition
    {
        public MachinistHeatCondition() : base(GambitConditionTypes.MachinistHeat)
        {
        }

        public int HeatMinimum { get; set; }
        public int HeatMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Machinist.Heat < HeatMinimum)
                return false;

            if (ActionResourceManager.Machinist.Heat > HeatMaximum)
                return false;

            return true;
        }
    }
}
