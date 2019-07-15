using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class MonkChakraCondition : GambitCondition
    {
        public MonkChakraCondition() : base(GambitConditionTypes.MonkChakra)
        {
        }

        public int ChakraMinimum { get; set; }
        public int ChakraMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Monk.FithChakra < ChakraMinimum)
                return false;

            if (ActionResourceManager.Monk.FithChakra > ChakraMaximum)
                return false;

            return true;
        }
    }
}