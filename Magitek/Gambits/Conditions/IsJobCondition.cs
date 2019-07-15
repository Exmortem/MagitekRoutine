using ff14bot.Enums;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class IsJobCondition : GambitCondition
    {
        public IsJobCondition() : base(GambitConditionTypes.IsJob)
        {
        }

        public bool IsWhm { get; set; } = false;
        public bool IsSch { get; set; } = false;
        public bool IsAst { get; set; } = false;
        public bool IsWar { get; set; } = false;
        public bool IsPld { get; set; } = false;
        public bool IsDrk { get; set; } = false;
        public bool IsBrd { get; set; } = false;
        public bool IsMch { get; set; } = false;
        public bool IsSmn { get; set; } = false;
        public bool IsBlm { get; set; } = false;
        public bool IsRdm { get; set; } = false;
        public bool IsNin { get; set; } = false;
        public bool IsMnk { get; set; } = false;
        public bool IsDrg { get; set; } = false;
        public bool IsSam { get; set; } = false;

        public override bool Check(GameObject gameObject = null)
        {
            if (gameObject == null)
                return false;

            var currentJob = ((BattleCharacter) gameObject).CurrentJob;

            if (IsWhm && currentJob == ClassJobType.WhiteMage)
                return true;

            if (IsSch && currentJob == ClassJobType.Scholar)
                return true;

            if (IsAst && currentJob == ClassJobType.Astrologian)
                return true;

            if (IsWar && currentJob == ClassJobType.Warrior)
                return true;

            if (IsPld && currentJob == ClassJobType.Paladin)
                return true;

            if (IsDrk && currentJob == ClassJobType.DarkKnight)
                return true;

            if (IsBrd && currentJob == ClassJobType.Bard)
                return true;

            if (IsMch && currentJob == ClassJobType.Machinist)
                return true;

            if (IsSmn && currentJob == ClassJobType.Summoner)
                return true;

            if (IsBlm && currentJob == ClassJobType.BlackMage)
                return true;

            if (IsRdm && currentJob == ClassJobType.RedMage)
                return true;

            if (IsNin && currentJob == ClassJobType.Ninja)
                return true;

            if (IsDrg && currentJob == ClassJobType.Dragoon)
                return true;

            if (IsSam && currentJob == ClassJobType.Samurai)
                return true;

            return IsMnk && currentJob == ClassJobType.Monk;
        }
    }
}