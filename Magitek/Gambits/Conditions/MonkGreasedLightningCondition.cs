using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class MonkGreasedLightningCondition : GambitCondition
    {
        public MonkGreasedLightningCondition() : base(GambitConditionTypes.MonkGreasedLightning)
        {
        }

        public int GreasedLightningMinimum { get; set; }
        public int GreasedLightningMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Monk.GreasedLightning < GreasedLightningMinimum)
                return false;

            if (ActionResourceManager.Monk.GreasedLightning > GreasedLightningMaximum)
                return false;

            return true;
        }
    }
}