using ff14bot;
using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Utilities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Gambits.Actions
{
    public class CastSpellOnCurrentTargetAction : BaseCastSpellAction
    {
        public CastSpellOnCurrentTargetAction() : base(GambitActionTypes.CastSpellOnCurrentTarget)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (SpellData == null)
                return false;

            if (!Core.Me.HasTarget)
                return false;

            var target = Core.Me.CurrentTarget;

            if (target == null)
                return false;

            if (conditions.Any(condition => !condition.Check(target)))
                return false;

            if (!await SpellData.Cast(target))
                return false;

            Casting.CastingGambit = true;
            return true;
        }
    }
}
