using Magitek.Commands;
using Magitek.Models.Summoner;
using Magitek.Toggles;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainOverlayViewModel
    {
        private static MainOverlayViewModel _instance;
        public static MainOverlayViewModel Instance => _instance ?? (_instance = new MainOverlayViewModel());

        public BaseSettings GeneralSettings => BaseSettings.Instance;
        public SummonerSettings SummonerSettings => SummonerSettings.Instance;

        public ObservableCollection<SettingsToggle> SettingsToggles { get; set; }

        public ICommand OpenMainSettingsForm => new DelegateCommand(() =>
        {
            Application.Current.Dispatcher.Invoke(delegate { Magitek.Form.Show(); });
        });
    }
}
