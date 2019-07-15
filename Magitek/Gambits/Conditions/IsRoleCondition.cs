using ff14bot.Objects;
using Magitek.Extensions;

namespace Magitek.Gambits.Conditions
{
    public class IsRoleCondition : GambitCondition
    {
        public IsRoleCondition() : base(GambitConditionTypes.IsRole)
        {
        }

        public bool IsTank { get; set; }
        public bool IsHealer { get; set; }
        public bool IsDps { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (gameObject == null)
                return false;

            if (IsTank && gameObject.IsTank())
                return true;

            if (IsHealer && gameObject.IsHealer())
                return true;

            return IsDps && gameObject.IsDps();
        }
    }
}