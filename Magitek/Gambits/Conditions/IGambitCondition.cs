using ff14bot.Objects;
using Magitek.Converters;
using Newtonsoft.Json;

namespace Magitek.Gambits.Conditions
{
    [JsonConverter(typeof(GambitConditionConverter))]
    public interface IGambitCondition
    {
        int Id { get; set; }
        bool Check(GameObject gameObject = null);
    }
}