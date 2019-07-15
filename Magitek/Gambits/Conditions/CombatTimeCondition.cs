using ff14bot.Objects;
using Magitek.Utilities;

namespace Magitek.Gambits.Conditions
{
    public class CombatTimeCondition : GambitCondition
    {
        public CombatTimeCondition() : base(GambitConditionTypes.CombatTime)
        {
        }

        public int MillisecondsMinimum { get; set; }
        public int MillisecondsMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (Combat.CombatTime.ElapsedMilliseconds < MillisecondsMinimum)
                return false;

            if (Combat.CombatTime.ElapsedMilliseconds > MillisecondsMaximum)
                return false;

            return true;
        }
    }
}
