using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    class MpPercentCondition : GambitCondition
    {
        public MpPercentCondition() : base(GambitConditionTypes.MpPercent)
        {
        }

        public int MPPercentageValue { get; set; }

        public bool Above { get; set; }

        public bool AboveOrEqual { get; set; }

        public bool Lower { get; set; }

        public bool LowerOrEqual { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (Above)
                return Core.Me.CurrentManaPercent > MPPercentageValue;

            if (AboveOrEqual)
                return Core.Me.CurrentManaPercent >= MPPercentageValue;

            if (Lower)
                return Core.Me.CurrentManaPercent < MPPercentageValue;

            if (LowerOrEqual)
                return Core.Me.CurrentManaPercent <= MPPercentageValue;

            return false;
        }
    }
}