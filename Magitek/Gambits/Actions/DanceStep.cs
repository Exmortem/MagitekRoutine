using ff14bot;
using Magitek.Logic.Dancer;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Gambits.Conditions;
using System.Collections.ObjectModel;

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

            if (!await Dances.DanceStep())
                return false;

            return true;
        }
    }
}