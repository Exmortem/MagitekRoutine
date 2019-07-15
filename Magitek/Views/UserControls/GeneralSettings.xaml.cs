using System.Windows;
using System.Windows.Controls;

namespace Magitek.Views.UserControls
{
    public partial class GeneralSettings : UserControl
    {
        public GeneralSettings()
        {
            InitializeComponent();
        }

        private void OverlayCheckChanged(object sender, RoutedEventArgs e)
        {
            //Application.Current.Dispatcher.Invoke(Utilities.Overlays.TogglePositional);
        }
    }
}
