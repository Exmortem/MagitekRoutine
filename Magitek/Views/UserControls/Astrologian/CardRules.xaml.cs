using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Clio.Utilities;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using Magitek.ViewModels;

namespace Magitek.Views.UserControls.Astrologian
{
    public partial class CardRules : UserControl
    {
        public CardRules()
        {
            InitializeComponent();

            var cardListItemStyle = new Style(typeof(ListBoxItem));

            // Set the events onto the ListItem style
            cardListItemStyle.Setters.Add(new Setter(AllowDropProperty, true));
            cardListItemStyle.Setters.Add(new EventSetter(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(ListsOnPreviewMouseLeftButtonDown)));
            cardListItemStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(CardRulesListDrop)));

            // Set ListItem style onto the List
            CardRulesList.ItemContainerStyle = cardListItemStyle;
        }

        private Point _dragStartPoint;

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parentObject = VisualTreeHelper.GetParent(child);
                if (parentObject == null)
                    return null;
                var parent = parentObject as T;

                if (parent != null)
                    return parent;
                child = parentObject;
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardRulesList.SelectedValue == null)
                return;

            AstrologianCardRules.Instance.SelectedCardRules = (CardRule)CardRulesList.SelectedValue;
            AstrologianCardRules.Instance.ReloadUiElements();
            CardRulesList.ScrollIntoView(CardRulesList.SelectedItem);
        }

        private void CombatSettingsToggleCheckChanged(object sender, RoutedEventArgs e)
        {
            AstrologianCardRules.Instance.ResetCombatSettingsUi();
        }

        private void HasHeldCardCheckChanged(object sender, RoutedEventArgs e)
        {
            AstrologianCardRules.Instance.ResetHasHeldCardSettingsUi();
        }

        private void DoesNotHaveHeldCardCheckChanged(object sender, RoutedEventArgs e)
        {
            AstrologianCardRules.Instance.ResetDoesNotHaveHeldCardSettingsUi();
        }

        private void IsJobCheckChanged(object sender, RoutedEventArgs e)
        {
            AstrologianCardRules.Instance.ResetIsJobSettingsUi();
        }

        private void IsRoleCheckChanged(object sender, RoutedEventArgs e)
        {
            AstrologianCardRules.Instance.ResetIsRoleSettingsUi();
        }

        private void TargetHasTargetCheckChanged(object sender, RoutedEventArgs e)
        {
            AstrologianCardRules.Instance.ResetTargetHasTargetSettingsUi();
        }

        private void MoveCard(int sourceIndex, int targetIndex)
        {
            // Get card priorities
            var newCardPriority = AstrologianCardRules.Instance.CardRules[targetIndex].CardPriority;
        
            // Park the card we're moving
            AstrologianCardRules.Instance.CardRules[sourceIndex].CardPriority = -1;

            // Shift the other cards down one to make room
            var count = 1;
            foreach (var ruletoreorder in AstrologianCardRules.Instance.CardRules)
            {
                if (ruletoreorder.CardPriority < newCardPriority) continue;
                else
                {
                    ruletoreorder.CardPriority = newCardPriority + count;
                    count++;
                }
            }

            // Move the card we're moving in place
            AstrologianCardRules.Instance.CardRules[sourceIndex].CardPriority = newCardPriority;

            // Fix CardPriority Values for the whole list
            count = AstrologianCardRules.Instance.CardRules.Count();

            foreach (var ruletoreorder in AstrologianCardRules.Instance.CardRules.OrderByDescending(r => r.CardPriority))
            {
                ruletoreorder.CardPriority = count;
                count--;
            }

            // Reset view source to re-sort
            AstrologianCardRules.Instance.ResetCollectionViewSource();
        }

        private void CardRulesListDrop(object sender, DragEventArgs e)
        {
            if (!(sender is ListBoxItem))
                return;

            // Cast to Card Rules
            var source = e.Data.GetData(typeof(CardRule)) as CardRule;
            var target = ((ListBoxItem)(sender)).DataContext as CardRule;

            // Get current indices
            var sourceIndex = AstrologianCardRules.Instance.CardRules.IndexOf(source);
            var targetIndex = AstrologianCardRules.Instance.CardRules.IndexOf(target);

            MoveCard(sourceIndex, targetIndex);
        }

        private void ListOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(null);
            var diff = _dragStartPoint - point;

            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                                                             Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));

                if (lbi != null)
                {
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void ListsOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
