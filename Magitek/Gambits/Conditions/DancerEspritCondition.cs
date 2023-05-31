using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class DancerEspritCondition : GambitCondition
    {
        public DancerEspritCondition() : base(GambitConditionTypes.DancerEsprit)
        {
        }

        public int EspritMinimum { get; set; }
        public int EspritMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Dancer.Esprit < EspritMinimum)
                return false;

            if (ActionResourceManager.Dancer.Esprit > EspritMaximum)
                return false;

            return true;
        }
    }
}
