using System.Windows;

namespace Magitek.Views
{
    public partial class SettingsModal : Window
    {
        public SettingsModal()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //private void OverlayCheckChanged(object sender, RoutedEventArgs e)
        //{
        //    Application.Current.Dispatcher.Invoke(Utilities.Overlays.TogglePositional);
        //}
    }
}
