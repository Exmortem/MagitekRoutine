using Magitek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Magitek.Toggles
{
    internal static class SettingsHandler
    {
        // Method to set a property on an IRoutineSetting instance
        public static void SetPropertyOnSettingsInstance(List<SettingsToggleSetting> settings, IRoutineSettings settingsInstance, bool checkedOn)
        {
            // does the toggle have multiple settings? If so, we should try to iterate through the IRoutineSettings once
            // and change them at the same time

            if (settings.Count > 1)
            {
                foreach (var property in settingsInstance.GetType().GetProperties())
                {
                    var setting = settings.FirstOrDefault(r => r.Name == property.Name);

                    if (setting == null)
                        continue;

                    SetValueOnProperty(property, setting, settingsInstance, checkedOn);
                }
            }
            else
            {
                var setting = settings.FirstOrDefault();

                if (setting == null)
                    return;

                SetValueOnProperty(settingsInstance.GetType().GetProperties().FirstOrDefault(r => r.Name == setting.Name), setting, settingsInstance, checkedOn);
            }
        }

        private static void SetValueOnProperty(PropertyInfo property, SettingsToggleSetting setting, IRoutineSettings settingsInstance, bool checkedOn)
        {
            switch (setting.Type)
            {
                case SettingType.Boolean:
                    property.SetValue(settingsInstance,
                        checkedOn
                            ? Convert.ChangeType(setting.BoolCheckedValue, property.PropertyType)
                            : Convert.ChangeType(setting.BoolUncheckedValue, property.PropertyType));
                    break;
                case SettingType.Integer:
                    property.SetValue(settingsInstance,
                        checkedOn
                            ? Convert.ChangeType(setting.IntCheckedValue, property.PropertyType)
                            : Convert.ChangeType(setting.IntUncheckedValue, property.PropertyType));
                    break;
                case SettingType.Float:
                    property.SetValue(settingsInstance,
                        checkedOn
                            ? Convert.ChangeType(setting.FloatCheckedValue, property.PropertyType)
                            : Convert.ChangeType(setting.FloatUncheckedValue, property.PropertyType));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IEnumerable<ToggleProperty> ExtractPropertyNamesAndTypesFromSettingsInstance(IRoutineSettings settings)
        {
            return settings.GetType().GetProperties().Where(TypeIsValidType).Select(r => new ToggleProperty { Name = r.Name, Type = r.TypeToSettingType() }).ToList().OrderBy(r => r.Name);
        }

        private static bool TypeIsValidType(this PropertyInfo property)
        {
            return property.PropertyType == typeof(bool) || property.PropertyType == typeof(int) || property.PropertyType == typeof(float);
        }

        private static SettingType TypeToSettingType(this PropertyInfo property)
        {
            if (property.PropertyType == typeof(bool))
                return SettingType.Boolean;

            if (property.PropertyType == typeof(int))
                return SettingType.Integer;

            if (property.PropertyType == typeof(float))
                return SettingType.Float;

            return SettingType.None;
        }

        public static bool SettingToggleSettingMatchesProperty(SettingsToggleSetting setting, PropertyInfo property, IRoutineSettings iroutineSettingsInstance)
        {
            switch (setting.Type)
            {
                case SettingType.Boolean:
                    var boolValue = (bool)property.GetValue(iroutineSettingsInstance, null);
                    return setting.BoolCheckedValue == boolValue;

                case SettingType.Integer:
                    var intValue = (int)property.GetValue(iroutineSettingsInstance, null);
                    return setting.IntCheckedValue == intValue;

                case SettingType.Float:
                    var floatValue = (float)property.GetValue(iroutineSettingsInstance, null);
                    return setting.FloatCheckedValue == floatValue;

                case SettingType.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
