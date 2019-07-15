using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Gambits.Conditions;

namespace Magitek.Gambits.Actions
{
    public class PetCastAction : GambitAction
    {
        public PetCastAction() : base(GambitActionTypes.PetCast)
        {
        }

        public string SpellName { get; set; }
        public bool OnPet { get; set; }
        public bool OnTarget { get; set; } = true;
        public bool OnPlayer { get; set; }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (Core.Me.Pet == null)
                return false;

            GameObject target = null;

            if (OnPet)
            {
                target = Core.Me.Pet;
            }

            if (OnTarget)
            {
                target = Core.Me.CurrentTarget;
            }

            if (OnPlayer)
            {
                target = Core.Me;
            }

            if (target == null)
                return false;

            if (conditions.Any(condition => !condition.Check(target)))
                return false;

            if (!PetManager.CanCast(SpellName, target))
                return false;

            if (PetManager.DoAction(SpellName, target))
                return false;

            return await Task.FromResult(false);
        }
    }
}
