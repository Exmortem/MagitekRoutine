using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Utilities;

namespace Magitek.Gambits.Actions
{
    public class CastSpellOnFriendlyNpcAction : BaseCastSpellAction
    {
        public CastSpellOnFriendlyNpcAction() : base(GambitActionTypes.CastSpellOnFriendlyNpc)
        {
        }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (SpellData == null)
                return false;

            var target = GameObjectManager.GameObjects.FirstOrDefault(x =>
                !x.CanAttack && x.Type != GameObjectType.Pc && conditions.All(z => z.Check(x)));

            if (target == null)
                return false;

            if (!await SpellData.Cast(target))
                return false;

            Casting.CastingGambit = true;
            return true;
        }
    }
}