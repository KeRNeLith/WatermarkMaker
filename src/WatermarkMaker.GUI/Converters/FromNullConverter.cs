using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WatermarkMaker.Converters
{
    internal sealed class FromNullConverter : MarkupExtension, IValueConverter
    {
        public object? IfNull { get; set; } = Binding.DoNothing;

        public object? IfNotNull { get; set; } = Binding.DoNothing;

        /// <inheritdoc />
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is null ? IfNull : IfNotNull;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Cannot {nameof(ConvertBack)} with {nameof(FromNullConverter)}.");
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}