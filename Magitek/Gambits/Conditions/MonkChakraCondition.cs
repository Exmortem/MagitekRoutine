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
            if (ActionResourceManager.Monk.ChakraCount < ChakraMinimum)
                return false;

            if (ActionResourceManager.Monk.ChakraCount > ChakraMaximum)
                return false;

            return true;
        }
    }
}