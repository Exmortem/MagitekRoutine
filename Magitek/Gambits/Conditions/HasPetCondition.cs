using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class HasPetCondition : GambitCondition
    {
        public HasPetCondition() : base(GambitConditionTypes.HasPet)
        {
        }

        public override bool Check(GameObject gameObject = null)
        {
            return Core.Me.Pet != null;
        }
    }
}
