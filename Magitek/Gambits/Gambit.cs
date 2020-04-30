using ff14bot.Enums;
using Magitek.Gambits.Actions;
using Magitek.Gambits.Conditions;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Magitek.Gambits
{
    [AddINotifyPropertyChangedInterface]
    public class Gambit
    {
        public ObservableCollection<IGambitCondition> Conditions { get; set; } = new ObservableCollection<IGambitCondition>();

        public IGambitAction Action { get; set; }

        public GambitActionTypes ActionType { get; set; }

        public ClassJobType Job { get; set; }

        public string Title { get; set; }

        public int Id { get; set; } = 1;

        public int Order { get; set; }

        public bool IsEnabled { get; set; }

        public int PreventSameActionForTheNextMilliseconds { get; set; }

        public bool HasChain { get; set; }

        public bool OnlyUseInChain { get; set; }

        public bool ForceChainActions { get; set; }

        public int ForceChainSleepMilliseconds { get; set; } = 250;

        public string ChainTitle { get; set; }

        public bool InterruptCast { get; set; }

        public int MaxTimeToWaitForAction { get; set; }

        public bool AbandonOpenerIfActionFail { get; set; }

        public int MaxTimeToWaitForCondition { get; set; } = 100;

        public async Task<bool> Execute()
        {
            return await Action.Execute(Conditions);
        }
    }
}