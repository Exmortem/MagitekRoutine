using ff14bot;
using ff14bot.Objects;
using System.Linq;

namespace Magitek.Gambits.Conditions
{
    public class PlayerHasAuraBelowCondition : GambitCondition
    {
        public PlayerHasAuraBelowCondition() : base(GambitConditionTypes.HasAura)
        {
        }

        public string AuraName { get; set; }
        public bool IsMyAura { get; set; }
        public int AuraMillisecondsLeft { get; set; }
        public int AuraStacks { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            var aura = IsMyAura
                ? Core.Me.CharacterAuras.FirstOrDefault(r => r.CasterId == Core.Player.ObjectId && r.LocalizedName == AuraName)
                : Core.Me.CharacterAuras.FirstOrDefault(r => r.LocalizedName == AuraName);

            if (aura == null)
                return false;

            if (aura.TimespanLeft.TotalMilliseconds > AuraMillisecondsLeft)
                return false;

            return aura.Value >= AuraStacks;
        }
    }
}
