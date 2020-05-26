using PropertyChanged;
using Magitek.Models;
using BaseSettingsModel = Magitek.Models.Account.BaseSettings;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class CombatMessageOverlayViewModel
    {
        private static CombatMessageOverlayViewModel _instance;
        public static CombatMessageOverlayViewModel Instance => _instance ?? (_instance = new CombatMessageOverlayViewModel());

        public CombatMessageModel CombatOverlayMessage => CombatMessageModel.Instance;
        public BaseSettingsModel BaseSettingsInstance => BaseSettingsModel.Instance;
    }
}
