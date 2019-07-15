using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class TargetNameCondition : GambitCondition
    {
        public TargetNameCondition() : base(GambitConditionTypes.TargetName)
        {
        }

        public string TargetName { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (gameObject == null)
                return false;

            return gameObject.Name == TargetName;
        }
    }
}