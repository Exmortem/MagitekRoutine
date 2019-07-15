using System;
using System.Globalization;
using System.Windows.Data;
using Magitek.Gambits;

namespace Magitek.Converters
{
    public class ConditionRemoveConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return false;

            var gambit = (Gambit)values[0];
            var id = (int)values[1];
            return new Tuple<Gambit, int>(gambit, id);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
