using System;
using Magitek.Gambits.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magitek.Converters
{
    public class GambitActionConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            if (Enum.TryParse<GambitActionTypes>(jsonObject["GambitActionType"].Value<string>(), out var parsedEnum))
            {
                GambitAction action;
                switch (parsedEnum)
                {
                    case GambitActionTypes.CastSpellOnSelf:
                        action = new CastSpellOnSelfAction();
                        break;

                    case GambitActionTypes.CastSpellOnCurrentTarget:
                        action = new CastSpellOnCurrentTargetAction();
                        break;

                    case GambitActionTypes.CastFillerOnCurrentTarget:
                        action = new CastFillerOnCurrentTargetAction();
                        break;

                    case GambitActionTypes.CastSpellOnAlly:
                        action = new CastSpellOnAllyAction();
                        break;

                    case GambitActionTypes.CastSpellOnEnemy:
                        action = new CastSpellOnEnemyAction();
                        break;

                    case GambitActionTypes.CastSpellOnFriendlyNpc:
                        action = new CastSpellOnFriendlyNpcAction();
                        break;

                    case GambitActionTypes.SleepForMilliseconds:
                        action = new SleepForTimeAction();
                        break;

                    case GambitActionTypes.NoAction:
                        action = new NullAction();
                        break;
                    case GambitActionTypes.ToastMessage:
                        action = new ToastMessageAction();
                        break;
                    case GambitActionTypes.UseItemOnSelf:
                        action = new UseItemOnSelfAction();
                        break;
                    case GambitActionTypes.PetCast:
                        action = new PetCastAction();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                serializer.Populate(jsonObject.CreateReader(), action);
                return action;
            }
            throw new InvalidCastException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GambitAction);
        }
    }
}
