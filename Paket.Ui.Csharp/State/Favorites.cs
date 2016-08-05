namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Favorites
    {
        internal static readonly DirectoryInfo Folder;

        static Favorites()
        {
            var path = System.IO.Path.Combine(Paket.Constants.NuGetCacheFolder, "Paket", "Favorites");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Folder = new DirectoryInfo(path);
        }

        public static async Task<IReadOnlyList<PackageInfo>> GetPackagesAsync()
        {
            var favorites = new List<PackageInfo>();
            foreach (var favorite in Folder.EnumerateFiles("*.favorite", SearchOption.TopDirectoryOnly))
            {
                var json = await File.ReadAllTextAsync(favorite.FullName).ConfigureAwait(false);
                var packageInfo = JsonConvert.DeserializeObject<PackageInfo>(json);
                favorites.Add(packageInfo);
            }

            return favorites;
        }

        public static bool IsFavorite(PackageInfo package)
        {
            if (package == null)
            {
                return false;
            }

            return System.IO.File.Exists(GetFileName(package));
        }

        private static string GetFileName(PackageInfo package)
        {
            return System.IO.Path.Combine(Folder.FullName, $"{package.Id}.favorite");
        }

        public static void SetIsFavorite(PackageInfo package, bool isFavorite)
        {
            if (package == null)
            {
                return;
            }

            if (isFavorite)
            {
                NugetCache.JsonAndPackageInfo result;
                if (NugetCache.TryGet(package.Id, out result))
                {
                    System.IO.File.WriteAllText(GetFileName(package), result.Json);
                }
            }
            else
            {
                System.IO.File.Delete(GetFileName(package));
            }
        }
    }
}
