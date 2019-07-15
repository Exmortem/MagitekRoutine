using System.Windows;
using System.Windows.Controls;

namespace Magitek.Controls
{
    public partial class SubmitSettings : UserControl
    {
        public static readonly DependencyProperty RoutineProperty = DependencyProperty.Register("Routine", typeof(string), typeof(SubmitSettings), new UIPropertyMetadata("SubmitSettings"));

        public SubmitSettings()
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
