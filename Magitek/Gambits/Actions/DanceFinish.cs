using ff14bot;
using Magitek.Logic.Dancer;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Gambits.Conditions;
using System.Collections.ObjectModel;

namespace Magitek.Gambits.Actions
{
    public class DanceStep : GambitAction
    {
        public DanceStep() : base(GambitActionTypes.DanceStep)
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