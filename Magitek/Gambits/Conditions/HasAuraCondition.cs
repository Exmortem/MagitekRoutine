using System;
using System.Linq;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;

namespace Magitek.Gambits.Conditions
{
    public class HasAuraCondition : GambitCondition
    {
        public HasAuraCondition() : base(GambitConditionTypes.HasAura)
        {
        }

        public string AuraName { get; set; }
        public bool IsMyAura { get; set; }
        public int AuraMillisecondsMinimum { get; set; }
        public int AuraMillisecondsMaximum { get; set; }
        public int AuraStacks { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            var unitAsCharacter = Core.Me.CurrentTarget as Character;

            if (unitAsCharacter == null || !unitAsCharacter.IsValid)
                return false;

            var aura = IsMyAura
                ? unitAsCharacter.CharacterAuras.FirstOrDefault(r => r.Caster == Core.Me && r.Name == AuraName)
                : unitAsCharacter.CharacterAuras.FirstOrDefault(r => r.Name == AuraName);

            if (aura == null)
                return false;

            var timeLeft = Math.Abs((int)aura.TimespanLeft.TotalMilliseconds);

            if (timeLeft != 0)
            {
                if (timeLeft < AuraMillisecondsMinimum || timeLeft > AuraMillisecondsMaximum)
                    return false;
            }

            return aura.Value >= AuraStacks;
        }
    }
}