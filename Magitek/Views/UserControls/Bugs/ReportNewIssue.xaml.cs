using System.Windows.Controls;
using System.Windows.Input;

namespace Magitek.Views.UserControls.Bugs
{
    public partial class ReportNewIssue : UserControl
    {
        public ReportNewIssue()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bitbucket.org/Exmortem/magitek/");
        }
    }
}
