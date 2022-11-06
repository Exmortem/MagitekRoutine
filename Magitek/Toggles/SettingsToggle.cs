using ff14bot.Helpers;
using ff14bot.Managers;
using Magitek.Commands;
using Magitek.Extensions;
using Magitek.Utilities;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace Magitek.Toggles
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsToggle
    {
        // Text that's displayed on the toggle
        public string ToggleText { get; set; }

        // Job the toggle belongs to
        public string ToggleJob { get; set; }

        // Show toggle on overlay?
        public bool ToggleShowOnOverlay { get; set; } = true;

        // Is PVP Toggle ?
        public bool IsPvpToggle { get; set; } = false;

        // Property that's bound to the IsChecked on the toggle
        public bool ToggleChecked { get; set; }

        // Collection of setting values
        public ObservableCollection<SettingsToggleSetting> Settings { get; set; }

        // Command that is bound to the Command (XAML) on the toggle
        public ICommand ExecuteToggleCommand => new DelegateCommand(ExecuteToggle);

        public void SetToggleState()
        {
            // Sets the state of this current toggle to checked or unchecked
            if (Settings == null || !Settings.Any())
            {
                Logging.Write($@"No Settings In Toggle: {ToggleText}");
                return;
            }

            var settingsInstance = ToggleJob.GetIRoutineSettingsFromJobString();
            var settingsProperties = settingsInstance.GetType().GetProperties();

            foreach (var settingsToggleSetting in Settings)
            {
                // Find the property that matches the toggle setting
                var settingsProperty = settingsProperties.FirstOrDefault(r => r.Name == settingsToggleSetting.Name);

                // If there's no property, continue the loop
                if (settingsProperty == null)
                    continue;

                // Check to see if the value on the property matches what our Checked value should be 
                if (SettingsHandler.SettingToggleSettingMatchesProperty(settingsToggleSetting, settingsProperty, settingsInstance))
                    continue;

                // Toggle is unchecked because one of its properties does not match its checked value
                ToggleChecked = false;
                return;
            }

            // Toggle is checked because all properties match their checked values
            ToggleChecked = true;
        }

        // Method that acts as the delegate for the command; executed on checked and unchecked
        private void ExecuteToggle()
        {
            var settingsInstance = ToggleJob.GetIRoutineSettingsFromJobString();

            if (settingsInstance == null)
            {
                Logger.Error("[Toggles] Settings Instance is null");
                return;
            }

            if (Settings == null || Settings.Count == 0)
                return;

            // Change the settings property
            SettingsHandler.SetPropertyOnSettingsInstance(Settings.ToList(), settingsInstance, ToggleChecked);

            // Reset the state of toggles, we have to check every toggle here because some toggles may
            // effect the state of other toggles
            TogglesManager.ResetToggles();
        }

        public ICommand AddToggleSettingCommand => new DelegateCommand<ToggleProperty>(toggleProperty =>
        {
            if (Settings.Any(r => r.Name == toggleProperty.Name))
                return;

            var newToggleSetting = new SettingsToggleSetting
            {
                Name = toggleProperty.Name,
                BoolCheckedValue = true,
                BoolUncheckedValue = false,
                IntCheckedValue = 0,
                IntUncheckedValue = 0,
                FloatCheckedValue = 0,
                FloatUncheckedValue = 0,
                Type = toggleProperty.Type
            };

            Settings.Add(newToggleSetting);
            SetToggleState();
        });

        public ICommand RemoveToggleSettingCommand => new DelegateCommand<SettingsToggleSetting>(settingsToggleSetting =>
        {
            if (settingsToggleSetting == null)
                return;

            Settings.Remove(settingsToggleSetting);
            SetToggleState();
        });

        // Hotkey for the toggle
        public Keys ToggleKey { get; set; }

        // Modifier key for the toggle
        public ModifierKeys ToggleModifierKey { get; set; }

        // We can either use the built-in RB hotkey manager for this or we can write our own
        public void RegisterHotkey()
        {
            //Unregister first, not sure why I did this in my previous implementations, but i'm assuming there's a reason
            HotkeyManager.Unregister($@"Magitek{ToggleText.Replace(" ", "")}");

            // Does the toggle even have keys set?
            if (ToggleKey == Keys.None && ToggleModifierKey == ModifierKeys.None)
                return;

            //Register the hotkey
            HotkeyManager.Register($@"Magitek{ToggleText.Replace(" ", "")}", ToggleKey, ToggleModifierKey, r =>
            {
                if (ToggleChecked)
                {
                    // Set the checked to false
                    ToggleChecked = false;
                    // Run ExecuteToggle
                    ExecuteToggle();
                }
                else
                {
                    // Set the checked to true
                    ToggleChecked = true;
                    // Run ExecuteToggle
                    ExecuteToggle();
                }
            });
        }
    }
}
