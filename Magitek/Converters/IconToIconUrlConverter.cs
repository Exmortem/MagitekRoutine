using ff14bot.Objects;
using Magitek.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class IconToIconUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SpellData spell)
            {
                return spell.IconUrl();
            }

            return "https://secure.xivdb.com/img/game/000000/000405.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
