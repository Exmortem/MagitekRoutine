using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Utilities;
using Newtonsoft.Json;

namespace Magitek.Gambits.Actions
{
    public class CastFillerOnCurrentTargetAction : BaseCastSpellAction
    {

        public string ProcName { get; set; }

        public CastFillerOnCurrentTargetAction() : base(GambitActionTypes.CastFillerOnCurrentTarget)
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

            if (conditions.All(condition => condition.Check(target)))
            {
                if (!await ProcData.Cast(target))
                    return false;

                Casting.CastingGambit = true;
                return true;
            }

            if (!await SpellData.Cast(target))
                return false;

            Casting.CastingGambit = true;
            return true;
        }

        [JsonIgnore]
        public SpellData ProcData
        {
            get
            {
                if (ProcName == null || string.IsNullOrEmpty(ProcName) || string.IsNullOrWhiteSpace(ProcName))
                    return null;

                return ActionManager.CurrentActions.Values.FirstOrDefault(ProcDataCheck);

            }
        }
        private bool ProcDataCheck(SpellData spell)
        {
            if (IsPvpSpell && !spell.IsPvP)
                return false;

            return string.Equals(spell.Name, ProcName, StringComparison.CurrentCultureIgnoreCase);
        }

    }
}
