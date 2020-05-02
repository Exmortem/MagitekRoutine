using Magitek.Gambits;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class OpenerConditionAddConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return false;

            try
            {
                var openerGroup = (OpenerGroup)values[0];
                var conditionType = (string)values[1];
                return new Tuple<OpenerGroup, string>(openerGroup, conditionType);
            }
            catch (Exception e)
            {
                // catching and doing nothing with an exception here because for whatever reason WPF decides
                // to go through every converter on every ICommand action causing exceptions on some of the commands. 

                // removing this will cause RemoveGambit to throw an error on this converter that MSObject cannot be cast to Gambit
                // even though the delete gambit button does not using multibinding or a converter
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
