
using System.Windows;

namespace Magitek.Controls
{

    public partial class OpenersBrowser : Window
    {
        public OpenersBrowser()
        {
            InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
