using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class DragoonGazeCondition : GambitCondition
    {
        public DragoonGazeCondition() : base(GambitConditionTypes.DragoonGaze)
        {
        }

        public int GazeMinimum { get; set; }
        public int GazeMaximum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ActionResourceManager.Dragoon.DragonGaze < GazeMinimum)
                return false;

            if (ActionResourceManager.Dragoon.DragonGaze > GazeMaximum)
                return false;

            return true;
        }
    }
}
