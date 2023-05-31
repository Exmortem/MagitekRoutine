using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class DancerFeatherCondition : GambitCondition
    {
        public DancerFeatherCondition() : base(GambitConditionTypes.DancerFeather)
        {
        }

        public int FeatherMinimum { get; set; }
        public int FeatherMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Dancer.FourFoldFeathers < FeatherMinimum)
                return false;

            if (ActionResourceManager.Dancer.FourFoldFeathers > FeatherMaximum)
                return false;

            return true;
        }
    }
}
