namespace Paket.Ui.Csharp
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class NullToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public Visibility? WhenNull { get; set; }

        public Visibility? WhenNot { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.WhenNull == null || this.WhenNot == null)
            {
                throw new InvalidOperationException("Specify visibility for both when null and when not");
            }

            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? this.WhenNull.Value : this.WhenNot.Value;

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
