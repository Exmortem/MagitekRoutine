using ff14bot;
using ff14bot.Objects;
using System;
using System.Linq;

namespace Magitek.Gambits.Conditions
{
    public class PlayerHasAuraCondition : GambitCondition
    {
        public PlayerHasAuraCondition() : base(GambitConditionTypes.PlayerHasAura)
        {
        }

        public string AuraName { get; set; }
        public bool IsMyAura { get; set; }
        public int AuraMillisecondsMinimum { get; set; }
        public int AuraMillisecondsMaximum { get; set; }
        public int AuraStacks { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            var aura = IsMyAura
                ? Core.Me.CharacterAuras.FirstOrDefault(r => r.Caster == Core.Me && r.Name == AuraName)
                : Core.Me.CharacterAuras.FirstOrDefault(r => r.Name == AuraName);

            if (aura == null)
            {
                return false;
            }

            var timeLeft = Math.Abs((int)aura.TimespanLeft.TotalMilliseconds);

            if (timeLeft != 0)
            {
                if (timeLeft < AuraMillisecondsMinimum || timeLeft > AuraMillisecondsMaximum)
                {
                    return false;
                }
            }

            if (aura.Value < AuraStacks)
            {
                return false;
            }

            return true;
        }
    }
}
