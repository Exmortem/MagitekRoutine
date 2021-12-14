using ff14bot;
using ff14bot.Helpers;
using Magitek.Commands;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.BlueMage;
using Magitek.Models.Dancer;
using Magitek.Models.DarkKnight;
using Magitek.Models.Dragoon;
using Magitek.Models.Gunbreaker;
using Magitek.Models.Machinist;
using Magitek.Models.Monk;
using Magitek.Models.Ninja;
using Magitek.Models.Paladin;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Reaper;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.Toggles;
using Magitek.Utilities;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;


namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TogglesViewModel
    {
        private static TogglesViewModel _instance;
        public static TogglesViewModel Instance => _instance ?? (_instance = new TogglesViewModel());

        private TogglesViewModel()
        {
            ResetJob(Core.Me.CurrentJob.ToString());
        }

        public string SelectedJob { get; set; }
        public SettingsToggle SelectedToggle { get; set; }
        public ObservableCollection<SettingsToggle> SettingsToggles { get; set; } = new ObservableCollection<SettingsToggle>();
        public IEnumerable<ToggleProperty> JobSettingsList { get; set; }

        public void ResetJob(string job)
        {
            // Add this here because TabChange is called on every tab selection, ugh
            if (job == SelectedJob)
                return;

            // Save toggles incase someone changes tabs without saving
            SaveToggles();

            SelectedJob = job;
            ResetPropertiesForJob(job);

            // Load new toggles
            LoadToggles();
        }

        #region Add and Register
        public ICommand AddToggleCommand => new DelegateCommand(() =>
        {
            Logger.WriteInfo($"[Toggles] Adding new toggle [{SelectedJob}]");

            var newToggle = new SettingsToggle
            {
                ToggleText = "New Toggle",
                Settings = new ObservableCollection<SettingsToggleSetting>(),
                ToggleJob = SelectedJob,
                ToggleKey = Keys.None,
                ToggleModifierKey = ModifierKeys.None
            };

            SettingsToggles.Add(newToggle);
        });

        public ICommand RemoveToggleCommand => new DelegateCommand<SettingsToggle>(settingsToggle =>
        {
            if (settingsToggle == null)
                return;

            if (SettingsToggles.Remove(settingsToggle))
                Logger.WriteInfo($@"[Toggles] Removed Toggle [{settingsToggle.ToggleText}]");
            else
                Logger.Error($@"[Toggles] Failed to Remove Toggle [{settingsToggle.ToggleText}]");
        });

        public ICommand RegisterTogglesCommand => new DelegateCommand(RegisterToggles);

        public void RegisterToggles()
        {
            SaveToggles();

            // We save here because LoadToggles reads from the json file
            TogglesManager.LoadTogglesForCurrentJob();
        }
        #endregion

        #region Saving and Loading
        public ICommand SaveTogglesCommand => new DelegateCommand(SaveToggles);

        public void SaveToggles()
        {
            // Called on main SettingsWindow closed or on command
            try
            {
                if (SettingsToggles == null || SettingsToggles.Count == 0)
                    return;

                var data = JsonConvert.SerializeObject(SettingsToggles, Formatting.Indented);
                File.WriteAllText(JsonSettings.CharacterSettingsDirectory + "/Magitek/Toggles/" + SelectedJob + "Toggles.json", data);
            }
            catch (Exception e)
            {
                Logger.WriteInfo(@"[Toggles] [Saving]");
                Logger.Error(e.Message);
            }
        }

        public void LoadToggles()
        {
            var togglesFolder = JsonSettings.CharacterSettingsDirectory + "/Magitek/Toggles/";
            var togglesFile = togglesFolder + SelectedJob + "Toggles.json";

            try
            {
                // Create the folder if it doesn't exist
                if (!Directory.Exists(togglesFolder))
                    Directory.CreateDirectory(togglesFolder);

                if (!File.Exists(togglesFile))
                {
                    // Create the new Toggle List here
                    SettingsToggles = new ObservableCollection<SettingsToggle>();
                    Logger.WriteInfo("Toggles do not exist for the current job");
                    return;
                }

                SettingsToggles = new ObservableCollection<SettingsToggle>(JsonConvert.DeserializeObject<ObservableCollection<SettingsToggle>>(File.ReadAllText(togglesFile)));
            }
            catch (Exception e)
            {
                Logger.WriteInfo(@"[Toggles] [Loading]");
                Logger.Error(e.Message);
            }
        }

        #endregion

        private void ResetPropertiesForJob(string job)
        {
            switch (SelectedJob)
            {
                case "Scholar":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(ScholarSettings.Instance));
                    break;

                case "WhiteMage":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(WhiteMageSettings.Instance));
                    break;

                case "Astrologian":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(AstrologianSettings.Instance));
                    break;

                case "Paladin":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(PaladinSettings.Instance));
                    break;

                case "Warrior":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(WarriorSettings.Instance));
                    break;

                case "DarkKnight":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(DarkKnightSettings.Instance));
                    break;

                case "Gunbreaker":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(GunbreakerSettings.Instance));
                    break;

                case "Bard":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(BardSettings.Instance));
                    break;

                case "Machinist":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(MachinistSettings.Instance));
                    break;

                case "Dancer":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(DancerSettings.Instance));
                    break;

                case "BlackMage":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(BlackMageSettings.Instance));
                    break;

                case "Summoner":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(SummonerSettings.Instance));
                    break;

                case "RedMage":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(RedMageSettings.Instance));
                    break;

                case "Monk":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(MonkSettings.Instance));
                    break;

                case "Dragoon":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(DragoonSettings.Instance));
                    break;

                case "Ninja":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(NinjaSettings.Instance));
                    break;

                case "Samurai":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(SamuraiSettings.Instance));
                    break;

                case "Reaper":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(ReaperSettings.Instance));
                    break;

                case "BlueMage":
                    JobSettingsList = new List<ToggleProperty>(SettingsHandler.ExtractPropertyNamesAndTypesFromSettingsInstance(BlueMageSettings.Instance));
                    break;
            }
        }
    }
}
