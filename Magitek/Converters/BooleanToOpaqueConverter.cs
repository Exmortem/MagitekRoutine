namespace Magitek.Converters
{
    public sealed class BooleanToOpaqueConverter : BooleanConverter<double>
    {
        public BooleanToOpaqueConverter() : base(1.0, 0.0) { }
    }
}
