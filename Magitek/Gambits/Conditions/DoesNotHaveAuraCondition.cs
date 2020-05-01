using ff14bot;
using ff14bot.Objects;
using System;
using System.Linq;

namespace Magitek.Gambits.Conditions
{
    public class DoesNotHaveAuraCondition : GambitCondition
    {
        public DoesNotHaveAuraCondition() : base(GambitConditionTypes.DoesNotHaveAura)
        {
        }

        public string AuraName { get; set; }
        public bool IsMyAura { get; set; }
        public int AuraMillisecondsMinimum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (gameObject == null)
                return false;

            var gameObjectAsBc = gameObject as BattleCharacter;

            if (gameObjectAsBc == null)
                return false;

            var aura = IsMyAura
                ? gameObjectAsBc.CharacterAuras.FirstOrDefault(r => r.CasterId == Core.Player.ObjectId && string.Equals(r.Name, AuraName, StringComparison.CurrentCultureIgnoreCase) && r.TimespanLeft.TotalMilliseconds > AuraMillisecondsMinimum)
                : gameObjectAsBc.CharacterAuras.FirstOrDefault(r => string.Equals(r.Name, AuraName, StringComparison.CurrentCultureIgnoreCase) && r.TimespanLeft.TotalMilliseconds > AuraMillisecondsMinimum);

            if (aura == null)
                return true;

            return false;
        }
    }
}