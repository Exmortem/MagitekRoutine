using System;
using Magitek.Gambits.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magitek.Converters
{
    public class GambitConditionConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            if (Enum.TryParse<GambitConditionTypes>(jsonObject["GambitConditionType"].Value<string>(), out var parsedEnum))
            {
                GambitCondition condition;
                switch (parsedEnum)
                {
                    case GambitConditionTypes.EnemyCastingSpell:
                        condition = new EnemyCastingSpellCondition();
                        break;
                    case GambitConditionTypes.HasAura:
                        condition = new HasAuraCondition();
                        break;
                    case GambitConditionTypes.HpPercentBelow:
                        condition = new HpPercentBelowCondition();
                        break;
                    case GambitConditionTypes.HpPercentBetween:
                        condition = new HpPercentBetweenCondition();
                        break;
                    case GambitConditionTypes.MpPercent:
                        condition = new MpPercentCondition();
                        break;
                    case GambitConditionTypes.InInstance:
                        condition = new InInstanceCondition();
                        break;
                    case GambitConditionTypes.NotInInstance:
                        condition = new NotInInstanceCondition();
                        break;
                    case GambitConditionTypes.IsJob:
                        condition = new IsJobCondition();
                        break;
                    case GambitConditionTypes.IsRole:
                        condition = new IsRoleCondition();
                        break;
                    case GambitConditionTypes.TargetName:
                        condition = new TargetNameCondition();
                        break;
                    case GambitConditionTypes.PlayerHasAura:
                        condition = new PlayerHasAuraCondition();
                        break;
                    case GambitConditionTypes.LastSpell:
                        condition = new LastSpellCondition();
                        break;
                    case GambitConditionTypes.EnemiesNearby:
                        condition = new EnemiesNearbyCondition();
                        break;
                    case GambitConditionTypes.PlayerDoesNotHaveAura:
                        condition = new PlayerDoesNotHaveAuraCondition();
                        break;
                    case GambitConditionTypes.DoesNotHaveAura:
                        condition = new DoesNotHaveAuraCondition();
                        break;
                    case GambitConditionTypes.PlayerInCombat:
                        condition = new PlayerInCombatCondition();
                        break;
                    case GambitConditionTypes.PlayerNotInCombat:
                        condition = new PlayerNotInCombatCondition();
                        break;
                    case GambitConditionTypes.MonkChakra:
                        condition = new MonkChakraCondition();
                        break;
                    case GambitConditionTypes.MonkGreasedLightning:
                        condition = new MonkGreasedLightningCondition();
                        break;
                    case GambitConditionTypes.SpellOffCooldown:
                        condition = new SpellOffCooldownCondition();
                        break;
                    case GambitConditionTypes.BardRepertoire:
                        condition = new BardRepertoireCondition();
                        break;
                    case GambitConditionTypes.MachinistHeat:
                        condition = new MachinistHeatCondition();
                        break;
                    case GambitConditionTypes.DragoonGaze:
                        condition = new DragoonGazeCondition();
                        break;
                    case GambitConditionTypes.DragoonGaugeTimer:
                        condition = new DragoonGazeCondition();
                        break;
                    case GambitConditionTypes.TargetIsBoss:
                        condition = new TargetIsBossCondition();
                        break;
                    case GambitConditionTypes.NinjaHuton:
                        condition = new NinjaHutonCondition();
                        break;
                    case GambitConditionTypes.NinjaNinki:
                        condition = new NinjaNinkiCondition();
                        break;
                    case GambitConditionTypes.SamuraiSen:
                        condition = new SamuraiSenCondition();
                        break;
                    case GambitConditionTypes.SamuraiKenki:
                        condition = new SamuraiKenkiCondition();
                        break;
                    case GambitConditionTypes.HasPet:
                        condition = new HasPetCondition();
                        break;
                    case GambitConditionTypes.CombatTime:
                        condition = new CombatTimeCondition();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                serializer.Populate(jsonObject.CreateReader(), condition);
                return condition;
            }
            throw new InvalidCastException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GambitCondition);
        }
    }
}
