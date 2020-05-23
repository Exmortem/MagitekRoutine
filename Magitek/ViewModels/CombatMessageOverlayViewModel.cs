using PropertyChanged;
using Magitek.Models;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class CombatMessageOverlayViewModel
    {
        private static CombatMessageOverlayViewModel _instance;
        public static CombatMessageOverlayViewModel Instance => _instance ?? (_instance = new CombatMessageOverlayViewModel());

        public CombatMessageModel CombatOverlayMessage => CombatMessageModel.Instance;
    }
}
