using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class BardRepertoireCondition : GambitCondition
    {
        public BardRepertoireCondition() : base(GambitConditionTypes.BardRepertoire)
        {
        }

        public int RepertoireMinimum { get; set; }
        public int RepertoireMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Bard.Repertoire < RepertoireMinimum)
                return false;

            if (ActionResourceManager.Bard.Repertoire > RepertoireMaximum)
                return false;

            return true;
        }
    }
}
