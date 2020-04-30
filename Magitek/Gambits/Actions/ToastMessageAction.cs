using ff14bot;
using Magitek.Gambits.Conditions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Gambits.Actions
{
    public class ToastMessageAction : GambitAction
    {
        public ToastMessageAction() : base(GambitActionTypes.ToastMessage)
        {
        }

        public string message { get; set; }
        public int displaySeconds { get; set; }

        public override async Task<bool> Execute(ObservableCollection<IGambitCondition> conditions)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
                return false;

            if (conditions.Any(condition => !condition.Check(Core.Me)))
                return false;

            Core.OverlayManager.AddToast(() => message,
                TimeSpan.FromSeconds(displaySeconds),
                System.Windows.Media.Colors.Red,
                System.Windows.Media.Colors.Black,
                new System.Windows.Media.FontFamily("Consolas"));

            return true;
        }
    }
}