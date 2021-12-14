using ff14bot.Enums;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.BlueMage;
using Magitek.Models.DarkKnight;
using Magitek.Models.Dragoon;
using Magitek.Models.Machinist;
using Magitek.Models.Monk;
using Magitek.Models.Ninja;
using Magitek.Models.Paladin;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.Utilities.Managers;
using Magitek.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Magitek.Models.Reaper;
using Magitek.Rotations;
using BaseSettings = Magitek.ViewModels.BaseSettings;

namespace Magitek.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InheritanceBehavior = InheritanceBehavior.SkipToThemeNext;
            InitializeComponent();

            Loaded += SettingsWindow_OnLoaded;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                    Paladin.IsSelected = true;
                    break;
                case ClassJobType.Pugilist:
                    Monk.IsSelected = true;
                    break;
                case ClassJobType.Marauder:
                    Warrior.IsSelected = true;
                    break;
                case ClassJobType.Lancer:
                    Dragoon.IsSelected = true;
                    break;
                case ClassJobType.Archer:
                    Bard.IsSelected = true;
                    break;
                case ClassJobType.Conjurer:
                    WhiteMage.IsSelected = true;
                    break;
                case ClassJobType.Thaumaturge:
                    BlackMage.IsSelected = true;
                    break;
                case ClassJobType.Paladin:
                    Paladin.IsSelected = true;
                    break;
                case ClassJobType.Monk:
                    Monk.IsSelected = true;
                    break;
                case ClassJobType.Warrior:
                    Warrior.IsSelected = true;
                    break;
                case ClassJobType.Dragoon:
                    Dragoon.IsSelected = true;
                    break;
                case ClassJobType.Bard:
                    Bard.IsSelected = true;
                    break;
                case ClassJobType.WhiteMage:
                    WhiteMage.IsSelected = true;
                    break;
                case ClassJobType.BlackMage:
                    BlackMage.IsSelected = true;
                    break;
                case ClassJobType.Arcanist:
                    Scholar.IsSelected = true;
                    break;
                case ClassJobType.Summoner:
                    Summoner.IsSelected = true;
                    break;
                case ClassJobType.Scholar:
                    Scholar.IsSelected = true;
                    break;
                case ClassJobType.Rogue:
                    Ninja.IsSelected = true;
                    break;
                case ClassJobType.Ninja:
                    Ninja.IsSelected = true;
                    break;
                case ClassJobType.Machinist:
                    Machinist.IsSelected = true;
                    break;
                case ClassJobType.DarkKnight:
                    DarkKnight.IsSelected = true;
                    break;
                case ClassJobType.Astrologian:
                    Astrologian.IsSelected = true;
                    break;
                case ClassJobType.Samurai:
                    Samurai.IsSelected = true;
                    break;
                case ClassJobType.BlueMage:
                    BlueMage.IsSelected = true;
                    break;
                case ClassJobType.RedMage:
                    RedMage.IsSelected = true;
                    break;
                case ClassJobType.Dancer:
                    Dancer.IsSelected = true;
                    break;
                case ClassJobType.Gunbreaker:
                    Gunbreaker.IsSelected = true;
                    break;
                case ClassJobType.Reaper:
                    Reaper.IsSelected = true;
                    break;
                default:
                    return;
            }
        }

        private void SettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Dispelling.Instance.Save();
            InterruptsAndStuns.Instance.Save();
            //TankBusters.Instance.Save();
            DispelManager.Reset();
            InterruptsAndStunsManager.Reset();
            //TankBusterManager.ResetHealers();
            //TankBusterManager.ResetTanks();
            GambitsViewModel.Instance.SaveGambits();
            GambitsViewModel.Instance.ApplyGambits();
            OpenersViewModel.Instance.SaveOpeners();
            OpenersViewModel.Instance.ApplyOpeners();
            TogglesViewModel.Instance.SaveToggles();

            #region Save Settings For All Routines
            ScholarSettings.Instance.Save();
            WhiteMageSettings.Instance.Save();
            AstrologianSettings.Instance.Save();
            PaladinSettings.Instance.Save();
            DarkKnightSettings.Instance.Save();
            WarriorSettings.Instance.Save();
            BardSettings.Instance.Save();
            MachinistSettings.Instance.Save();
            DragoonSettings.Instance.Save();
            MonkSettings.Instance.Save();
            NinjaSettings.Instance.Save();
            SamuraiSettings.Instance.Save();
            ReaperSettings.Instance.Save();
            BlueMageSettings.Instance.Save();
            BlackMageSettings.Instance.Save();
            RedMageSettings.Instance.Save();
            SummonerSettings.Instance.Save();
            #endregion

            Hide();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            DragMove();
        }

        private void DebuggingClick(object sender, RoutedEventArgs e)
        {
            DebuggingTab.IsSelected = true;
        }

        private void GambitsClick(object sender, RoutedEventArgs e)
        {
            GambitsTab.IsSelected = true;
        }

        private void OpenersClick(object sender, RoutedEventArgs e)
        {
            OpenersTab.IsSelected = true;
        }

        public void ShowModal(Window modal)
        {
            modal.Width = Width;
            modal.Height = Height;
            modal.Top = Top;
            modal.Left = Left;
            modal.Owner = this;
            modal.ShowDialog();
        }

        private void JobTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is TabControl tabControl))
                return;

            var selectedTab = tabControl.SelectedItem as TabItem;

            if (selectedTab?.Name == null)
                return;

            BaseSettings.Instance.RoutineSelectedInUi = selectedTab.Name;
            TogglesViewModel.Instance.ResetJob(selectedTab.Name);
        }

        private void BossFightLogClick(object sender, RoutedEventArgs e)
        {
            BossFightLog.IsSelected = true;
        }
    }
}
