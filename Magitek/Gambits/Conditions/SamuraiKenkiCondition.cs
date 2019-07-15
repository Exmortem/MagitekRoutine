using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class SamuraiKenkiCondition : GambitCondition
    {
        public SamuraiKenkiCondition() : base(GambitConditionTypes.SamuraiKenki)
        {
        }

        public int KenkiMinimum { get; set; }
        public int KenkiMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Samurai.Kenki < KenkiMinimum)
                return false;

            if (ActionResourceManager.Samurai.Kenki > KenkiMaximum)
                return false;

            return true;
        }
    }
}
