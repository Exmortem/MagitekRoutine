namespace Magitek.Converters
{
    public sealed class BooleanToHalfOpaqueConverter : BooleanConverter<double>
    {
        public BooleanToHalfOpaqueConverter() : base(1.0, 0.5) { }
    }
}
