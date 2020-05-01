using ff14bot.Managers;
using ff14bot.Objects;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class SpellIdToSpellData : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint spell)
            {
                return DataManager.GetSpellData(spell);
            }

            return DataManager.GetSpellData(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SpellData spell)
            {
                return spell.Id;
            }

            return 0;
        }
    }
}
