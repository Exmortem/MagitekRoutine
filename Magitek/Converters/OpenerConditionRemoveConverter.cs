using Magitek.Gambits;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class OpenerConditionRemoveConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return false;

            var openerGroup = (OpenerGroup)values[0];
            var id = (int)values[1];
            return new Tuple<OpenerGroup, int>(openerGroup, id);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
