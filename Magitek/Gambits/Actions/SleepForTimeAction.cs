using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Magitek.Gambits.Conditions;

namespace Magitek.Gambits.Actions
{
    public class SleepForTimeAction : GambitAction
    {
        public SleepForTimeAction() : base(GambitActionTypes.SleepForMilliseconds)
        {
        }

        public int DurationInMilliseconds { get; set; }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (!conditions.All(x => x.Check())) return false;

            await Coroutine.Sleep(TimeSpan.FromMilliseconds(DurationInMilliseconds));
            return true;
        }
    }
}