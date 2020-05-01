using Magitek.Gambits.Conditions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Magitek.Gambits.Actions
{
    public class NullAction : GambitAction
    {
        public NullAction() : base(GambitActionTypes.NoAction)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            return await Task.FromResult(false);
        }
    }
}