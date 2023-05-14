using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WatermarkMaker.Converters
{
    internal sealed class FromBooleanConverter : MarkupExtension, IValueConverter
    {
        public object IfTrue { get; set; } = Binding.DoNothing;

        public object IfFalse { get; set; } = Binding.DoNothing;

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? IfTrue : IfFalse;
            return Binding.DoNothing;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == IfTrue)
                return true;
            return false;
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}