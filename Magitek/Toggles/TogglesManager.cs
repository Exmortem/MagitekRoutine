using ff14bot;
using ff14bot.Helpers;
using Magitek.Models.Account;
using Magitek.Utilities;
using Magitek.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Magitek.Toggles
{
    internal static class TogglesManager
    {
        public static void LoadTogglesForCurrentJob()
        {
            var settingsToggles = GetTogglesForJob;
            Logger.WriteInfo($@"[Toggles] {Core.Me.CurrentJob} Loading...");
            SetToggleHotkeys(settingsToggles);
            SetTogglesOnOverlay(settingsToggles);
            ResetToggles();
        }

        public static void ResetToggles()
        {
            foreach (var toggle in MainOverlayViewModel.Instance.SettingsToggles)
                toggle.SetToggleState();
        }

        private static void SetTogglesOnOverlay(List<SettingsToggle> settingsToggles)
        {
            if (settingsToggles == null || !settingsToggles.Any())
            {
                MainOverlayViewModel.Instance.SettingsToggles = new ObservableCollection<SettingsToggle>();
                return;
            }

            MainOverlayViewModel.Instance.SettingsToggles =
                Models.Account.BaseSettings.Instance.ActivePvpCombatRoutine
                ? new ObservableCollection<SettingsToggle>(settingsToggles.Where(r => r.ToggleShowOnOverlay && r.IsPvpToggle))
                : new ObservableCollection<SettingsToggle>(settingsToggles.Where(r => r.ToggleShowOnOverlay && !r.IsPvpToggle));
        }

        private static void SetToggleHotkeys(IEnumerable<SettingsToggle> settingsToggles)
        {
            if (settingsToggles == null)
                return;

            foreach (var settingsToggle in settingsToggles)
                settingsToggle.RegisterHotkey();
        }

        private static List<SettingsToggle> GetTogglesForJob
        {
            get
            {
                var togglesFolder = JsonSettings.CharacterSettingsDirectory + "/Magitek/Toggles/";
                var togglesFile = togglesFolder + Core.Me.CurrentJob + "Toggles.json";

                return !File.Exists(togglesFile)
                    ? null
                    : new List<SettingsToggle>(JsonConvert.DeserializeObject<List<SettingsToggle>>(File.ReadAllText(togglesFile)));
            }
        }
    }
}
