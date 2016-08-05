namespace Paket.Ui.Csharp
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class DependencyInfoToImageConverter : MarkupExtension, IValueConverter
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

            var packageInfo = value as PackageInfo;
            if (packageInfo != null)
            {
                var url = packageInfo.IconUrl;
                if (string.IsNullOrWhiteSpace(url))
                {
                    return this.WhenPackage;
                }

                var extension = System.IO.Path.GetExtension(url);
                if (extension == ".svg")
                {
                    return this.WhenPackage;
                }

                return url;
            }
            if (value is RemoteFileInfo)
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