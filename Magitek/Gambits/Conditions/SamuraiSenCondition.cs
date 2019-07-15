using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Gambits.Conditions
{
    public class SamuraiSenCondition : GambitCondition
    {
        public SamuraiSenCondition() : base(GambitConditionTypes.SamuraiSen)
        {
        }

        public bool HasSetsu { get; set; }
        public bool HasGetsu { get; set; }
        public bool HasKa { get; set; }

        public override bool Check(GameObject gameObject = null)
        {
            if (HasGetsu && !ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Getsu))
                return false;

            if (HasSetsu && !ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Setsu))
                return false;

            if (HasKa && !ActionResourceManager.Samurai.Sen.HasFlag(ActionResourceManager.Samurai.Iaijutsu.Ka))
                return false;

            return true;
        }
    }
}
