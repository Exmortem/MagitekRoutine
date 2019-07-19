using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Magitek.Gambits.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Magitek.Gambits.Actions
{
    public abstract class GambitAction : IGambitAction
    {
        protected GambitAction(GambitActionTypes type)
        {
            GambitActionType = type;
        }

        public GambitActionTypes GambitActionType { get; }
        public abstract Task<bool> Execute(ObservableCollection<IGambitCondition> conditions);
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GambitActionTypes
    {
        [Description("NO ACTION")] NoAction,
        [Description("CAST ON TARGET")] CastSpellOnCurrentTarget,
        [Description("CAST FILLER ON TARGET")] CastFillerOnCurrentTarget,
        [Description("CAST ON SELF")] CastSpellOnSelf,
        [Description("CAST ON ALLY")] CastSpellOnAlly,
        [Description("CAST ON ENEMY")] CastSpellOnEnemy,
        [Description("CAST ON NPC")] CastSpellOnFriendlyNpc,
        [Description("USE ITEM ON SELF")] UseItemOnSelf,
        [Description("SLEEP")] SleepForMilliseconds,
        [Description("TOAST MESSAGE")] ToastMessage,
        [Description("PET CAST")] PetCast,
    }
}