using ff14bot.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Magitek.Gambits.Conditions
{
    public abstract class GambitCondition : IGambitCondition
    {
        protected GambitCondition(GambitConditionTypes type)
        {
            GambitConditionType = type;
        }

        public int Id { get; set; } = 1;
        public GambitConditionTypes GambitConditionType { get; }
        public abstract bool Check(GameObject gameObject = null);
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GambitConditionTypes
    {
        EnemyCastingSpell,
        HasAura,
        PlayerHasAura,
        HpPercentBelow,
        HpPercentBetween,
        MpPercent,
        InInstance,
        NotInInstance,
        IsJob,
        IsRole,
        TargetName,
        LastSpell,
        EnemiesNearby,
        PlayerDoesNotHaveAura,
        DoesNotHaveAura,
        PlayerInCombat,
        PlayerNotInCombat,
        TargetIsBoss,
        MonkChakra,
        MonkGreasedLightning,
        SpellOffCooldown,
        BardRepertoire,
        MachinistHeat,
        DragoonGaze,
        DragoonGaugeTimer,
        NinjaHuton,
        NinjaNinki,
        SamuraiSen,
        SamuraiKenki,
        HasPet,
        CombatTime,
    }
}