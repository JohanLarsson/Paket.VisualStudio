namespace Paket.Ui.Csharp
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class DependencyToImageConverter : MarkupExtension, IValueConverter
    {
        public Uri WhenPackage { get; set; }

        public Uri WhenRemoteFile { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.WhenPackage == null || this.WhenRemoteFile == null)
            {
                throw new InvalidOperationException("Specify visibility for both when null and when not");
            }

            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is PackageViewModel)
            {
                return this.WhenPackage;
            }
            if (value is RemoteFileViewModel)
            {
                return this.WhenRemoteFile;
            }

            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}