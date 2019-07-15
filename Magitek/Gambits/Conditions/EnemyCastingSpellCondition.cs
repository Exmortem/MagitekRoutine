using System;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Utilities;

namespace Magitek.Gambits.Conditions
{
    public class EnemyCastingSpellCondition : GambitCondition
    {
        public EnemyCastingSpellCondition() : base (GambitConditionTypes.EnemyCastingSpell) 
        {
        }

        public string SpellName { get; set; }
        public int TimeIntoCast { get; set; }
        public bool TargetAnyone { get; set; }
        public bool TargetOfSpell { get; set; }
        public bool PlayerTargetOfSpell { get; set; }
        
        public override bool Check(GameObject gameObject = null)
        {
            var enemyCasting = GameObjectManager.GetObjectsOfType<BattleCharacter>().FirstOrDefault(r => r.IsCasting && string.Equals(r.SpellCastInfo.Name, SpellName, StringComparison.InvariantCultureIgnoreCase));

            if (enemyCasting == null)
                return false;

            if (enemyCasting.SpellCastInfo.CurrentCastTime.TotalMilliseconds < TimeIntoCast)
            {
                return false;
            }

            if (TargetOfSpell)
            {
                if (enemyCasting.TargetGameObject != gameObject)
                    return false;
            }

            if (PlayerTargetOfSpell)
            {
                if (enemyCasting.TargetGameObject != Core.Me)
                {
                    return false;
                }
            }

            return true;
        }
    }
}