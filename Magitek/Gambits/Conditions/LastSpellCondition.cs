using ff14bot.Objects;
using System;

namespace Magitek.Gambits.Conditions
{
    public class LastSpellCondition : GambitCondition
    {
        public LastSpellCondition() : base(GambitConditionTypes.LastSpell)
        {
        }

        public string SpellName { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (Utilities.Casting.LastSpell == null)
                return false;

            return string.Equals(Utilities.Casting.LastSpell.Name, SpellName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}