namespace Paket.Ui.Csharp
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    public class GetTypeConverter : IValueConverter
    {
        public static readonly GetTypeConverter Default = new GetTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{this.GetType().Name} only supports oneway bindings.");
        }
    }
}
