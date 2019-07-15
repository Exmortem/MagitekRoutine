using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Magitek.Gambits.Conditions;

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