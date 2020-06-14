using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class MultiBooleanAndConverter<T> : IMultiValueConverter
    {
        public MultiBooleanAndConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (!(value is bool) || (bool)value == false)
                {
                    return False;
                }
            }
            return True;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MultiBooleanAndConverter is a OneWay converter.");
        }
    }
}
