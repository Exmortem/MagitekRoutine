using System.Windows.Controls;
using ff14bot.Enums;
using Magitek.Gambits;
using Magitek.Gambits.Actions;
using Magitek.Utilities.Managers;
using Magitek.ViewModels;

namespace Magitek.Controls
{
    public partial class OpenersControl : UserControl
    {
        public OpenersControl()
        {
            InitializeComponent();
            SelectStartingJob();
        }

        private void SelectStartingJob()
        {
            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.WhiteMage:
                    WhiteMage.IsChecked = true;
                    break;

                case ClassJobType.Scholar:
                    Scholar.IsChecked = true;
                    break;

                case ClassJobType.Astrologian:
                    Astrologian.IsChecked = true;
                    break;

                case ClassJobType.Paladin:
                    Paladin.IsChecked = true;
                    break;

                case ClassJobType.Warrior:
                    Warrior.IsChecked = true;
                    break;

                case ClassJobType.DarkKnight:
                    DarkKnight.IsChecked = true;
                    break;

                case ClassJobType.Bard:
                    Bard.IsChecked = true;
                    break;

                case ClassJobType.Machinist:
                    Machinist.IsChecked = true;
                    break;

                case ClassJobType.Summoner:
                    Summoner.IsChecked = true;
                    break;

                case ClassJobType.BlackMage:
                    BlackMage.IsChecked = true;
                    break;

                case ClassJobType.RedMage:
                    RedMage.IsChecked = true;
                    break;

                case ClassJobType.Ninja:
                    Ninja.IsChecked = true;
                    break;

                case ClassJobType.Monk:
                    Monk.IsChecked = true;
                    break;

                case ClassJobType.Dragoon:
                    Dragoon.IsChecked = true;
                    break;

                case ClassJobType.Samurai:
                    Samurai.IsChecked = true;
                    break;

                case ClassJobType.Dancer:
                    Dancer.IsChecked = true;
                    break;

                case ClassJobType.Gunbreaker:
                    Gunbreaker.IsChecked = true;
                    break;

                default:
                    return;
            }
        }

        private void GambitActionType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var cmbBox = (ComboBox)sender;

            if (sender == null)
                return;

            var selectedValue = (GambitActionTypes)e.AddedItems[0];
            var gambit = (Gambit)cmbBox.DataContext;

            if (selectedValue == gambit.ActionType)
                return;

            OpenersViewModel.Instance.ActionSelectionChange(gambit, selectedValue);
        }
    }
}
