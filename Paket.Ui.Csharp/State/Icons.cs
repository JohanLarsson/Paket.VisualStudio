namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;

    public static class Icons
    {
        public static readonly BitmapSource DefaultPackageIcon = CreateIcon(@"Images/NuGetIcon_32x32.png");
        public static readonly BitmapSource OctocatIcon = CreateIcon(@"Images/OctocatIcon_32x32.png");
        private static readonly ConcurrentDictionary<string, Task<BitmapSource>> Cache = new ConcurrentDictionary<string, Task<BitmapSource>>();

        static Icons()
        {
            NugetCache.PackageUpdated += (_, p) =>
            {
                var url = p?.IconUrl;
                if (url != null)
                {
                    Cache.GetOrAdd(url, Download);
                }
            };
        }


        internal static event EventHandler<string> IconUpdated;

        internal static BitmapSource GetIcon(string iconUrl)
        {
            if (string.IsNullOrWhiteSpace(iconUrl))
            {
                return null;
            }

            var task = Cache.GetValueOrDefault(iconUrl);
            if (task?.IsCompleted == true)
            {
                return task.Result;
            }

            return null;
        }

        private static BitmapSource CreateIcon(string resourceName)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Paket.Ui.Csharp;component/" + resourceName, UriKind.Absolute));
        }

        private static async Task<BitmapSource> Download(string iconUrl)
        {
            if (iconUrl.EndsWith(".svg"))
            {
                return null;
            }

            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = await client.OpenReadTaskAsync(iconUrl).ConfigureAwait(false))
                    {
                        using (var bitmap = new Bitmap(stream))
                        {
                            var source = bitmap.AsBitmapSource();
                            source.Freeze();
                            IconUpdated?.Invoke(null, iconUrl);
                            return source;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static BitmapSource AsBitmapSource(this Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
