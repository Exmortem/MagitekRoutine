using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Utilities;

namespace Magitek.Gambits.Actions
{
    public class CastSpellOnAllyAction : BaseCastSpellAction
    {
        public CastSpellOnAllyAction() : base(GambitActionTypes.CastSpellOnAlly)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (SpellData == null)
                return false;

            var target = Group.CastableAlliesWithin30.FirstOrDefault(x => conditions.All(z => z.Check(x)));

            if (target == null)
                return false;

            if (!await SpellData.Cast(target))
                return false;

            Casting.CastingGambit = true;
            return true;
        }
    }
}