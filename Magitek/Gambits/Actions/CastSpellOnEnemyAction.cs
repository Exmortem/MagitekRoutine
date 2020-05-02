using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Utilities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Gambits.Actions
{
    public class CastSpellOnEnemyAction : BaseCastSpellAction
    {
        public CastSpellOnEnemyAction() : base(GambitActionTypes.CastSpellOnEnemy)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (SpellData == null)
                return false;

            var target = Combat.Enemies.FirstOrDefault(x => conditions.All(z => z.Check(x)));

            if (target == null)
                return false;

            if (!await SpellData.Cast(target))
                return false;

            // Dragon Specific for Mirage Dive tracking
            if (SpellData == Spells.MirageDive)
                Utilities.Routines.Dragoon.MirageDives++;

            Casting.CastingGambit = true;
            return true;
        }
    }
}