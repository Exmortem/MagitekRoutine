using System;
using System.Linq;
using ff14bot;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class PlayerDoesNotHaveAuraCondition : GambitCondition
    {
        public PlayerDoesNotHaveAuraCondition() : base(GambitConditionTypes.PlayerDoesNotHaveAura)
        {
        }

        public string AuraName { get; set; }
        public bool IsMyAura { get; set; }
        public int AuraMillisecondsMinimum { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            var aura = IsMyAura
                ? Core.Me.CharacterAuras.FirstOrDefault(r => r.CasterId == Core.Player.ObjectId && string.Equals(r.Name, AuraName, StringComparison.CurrentCultureIgnoreCase) && r.TimespanLeft.TotalMilliseconds > AuraMillisecondsMinimum)
                : Core.Me.CharacterAuras.FirstOrDefault(r => string.Equals(r.Name, AuraName, StringComparison.CurrentCultureIgnoreCase) && r.TimespanLeft.TotalMilliseconds > AuraMillisecondsMinimum);

            if (aura == null)
                return true;

            return false;
        }
    }
}
