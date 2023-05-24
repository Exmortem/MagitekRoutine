using ff14bot;
using Magitek.Gambits.Conditions;
using Magitek.Logic.Dancer;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Gambits.Actions
{
    public class DanceFinish : GambitAction
    {
        public DanceFinish() : base(GambitActionTypes.DanceFinish)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {

            if (conditions.Any(condition => !condition.Check(Core.Me)))
                return false;

            if (!await Dances.DanceFinish())
                return false;

            return true;
        }
    }
}