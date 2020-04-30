using Magitek.Converters;
using Magitek.Gambits.Conditions;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Magitek.Gambits.Actions
{
    [JsonConverter(typeof(GambitActionConverter))]
    public interface IGambitAction
    {
        Task<bool> Execute(ObservableCollection<IGambitCondition> conditions);
    }
}