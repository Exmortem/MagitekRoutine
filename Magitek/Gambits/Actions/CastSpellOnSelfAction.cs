using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Utilities;

namespace Magitek.Gambits.Actions
{
    public class CastSpellOnSelfAction : BaseCastSpellAction
    {
        public CastSpellOnSelfAction() : base(GambitActionTypes.CastSpellOnSelf)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (SpellData == null)
                return false;

            if (conditions.Any(condition => !condition.Check(Core.Me)))
              return false;

            if (!await SpellData.Cast(Core.Me))
                return false;

            Casting.CastingGambit = true;
            return true;
        }
    }
}