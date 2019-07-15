using System.Windows;
using System.Windows.Controls;

namespace Magitek.Controls
{
    public partial class SharedSettings : UserControl
    {
        public static readonly DependencyProperty RoutineProperty = DependencyProperty.Register("Routine", typeof(string), typeof(SharedSettings), new UIPropertyMetadata("SharedSettings"));

        public SharedSettings()
        {
            InitializeComponent();
        }

        public string Routine
        {
            get => (string)GetValue(RoutineProperty);
            set => SetValue(RoutineProperty, value);
        }
    }
}
