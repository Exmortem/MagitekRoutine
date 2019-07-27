using PropertyChanged;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class BossFightViewModel
    {
        private static BossFightViewModel _instance;
        public static BossFightViewModel Instance => _instance ?? (_instance = new BossFightViewModel());
    }
}
