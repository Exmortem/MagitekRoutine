using System.Windows;

namespace Magitek.Controls
{
    public partial class GambitBrowser : Window
    {
        public GambitBrowser()
        {
            InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
