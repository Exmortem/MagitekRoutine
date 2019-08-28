using System.Linq;
using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class EnemiesNearbyCondition : GambitCondition
    {
        public EnemiesNearbyCondition() : base(GambitConditionTypes.EnemiesNearby)
        {
        }

        public int Count { get; set; }
        public int Range { get; set; }
        public bool ByPlayer { get; set; }
        public bool ByTargetObject { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (ByTargetObject)
            {
                if (gameObject == null)
                    return false;

                if (Utilities.Combat.Enemies.Count(r => r.Distance(gameObject.Location) <= Range) >= Count)
                    return true;
            }

            if (!ByPlayer)
                return false;

            return Utilities.Combat.Enemies.Count(r => r.Distance(Core.Me) <= Range) >= Count;
        }
    }
}
