using System;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class MultiBooleanAndToBooleanConverter : MultiBooleanAndConverter<bool>
    {
        public MultiBooleanAndToBooleanConverter() : base(true, false) { }
    }
}
