using System;
using System.Globalization;
using System.Windows.Data;

namespace Magitek.Converters
{
    public class MultiBooleanAndToHalfOpaqueConverter : MultiBooleanAndConverter<double>
    {
        public MultiBooleanAndToHalfOpaqueConverter() : base(1.0, 0.5) { }
    }
}
