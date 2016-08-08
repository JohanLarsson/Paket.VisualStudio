namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;

    public class PackageViewModel : DependencyViewModel
    {
        private static readonly List<PackageViewModel> Cache = new List<PackageViewModel>();
        private static readonly object Gate = new object();

        static PackageViewModel()
        {
            NugetCache.PackageUpdated += (_, info) =>
            {
                lock (Gate)
                {
                    var vm = Cache.SingleOrDefault(x => x.Name == info.Id);
                    if (vm != null)
                    {
                        vm.OnPropertyChanged(nameof(Info));
                        vm.OnPropertyChanged(nameof(IsFavorite));
                    }
                }
            };

            Icons.IconUpdated += (_, url) =>
            {
                lock (Gate)
                {
                    foreach (var viewModel in Cache)
                    {
                        if (viewModel?.Info?.IconUrl == url)
                        {
                            viewModel.OnPropertyChanged(nameof(Icon));
                        }
                    }
                }
            };
        }

        private PackageViewModel(string name)
            : base(name)
        {
        }

        public override string Version => this.GetLockFileVersion();

        public override BitmapSource Icon => Icons.GetIcon(this.Info?.IconUrl) ?? Icons.DefaultPackageIcon;

        public override DependencyInfo Info
        {
            get
            {
                NugetCache.JsonAndPackageInfo result;
                return NugetCache.TryGet(this.Name, out result) ? result.Package : null;
            }
        }

        public override bool IsFavorite
        {
            get { return Favorites.IsFavorite((PackageInfo)this.Info); }
            set
            {
                Favorites.SetIsFavorite((PackageInfo)this.Info, value);
                this.OnPropertyChanged();
            }
        }

        private string GetLockFileVersion()
        {
            var lockFile = State.LockFile;
            if (lockFile == null)
            {
                return string.Empty;
            }

            foreach (var fileGroup in lockFile.Groups)
            {
                foreach (var resolvedPackage in fileGroup.Value.Resolution)
                {
                    if (string.Equals(resolvedPackage.Value.Name.Item1, this.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resolvedPackage.Value.Version.AsString;
                    }
                }
            }

            return "not found";
        }

        internal static PackageViewModel GetOrCreate(PackageInstallSettings installSettings)
        {
            return GetOrCreate(installSettings?.Name.ToString());
        }

        internal static PackageViewModel GetOrCreate(Requirements.PackageRequirement packageRequirement)
        {
            return GetOrCreate(packageRequirement?.Name.ToString());
        }

        internal static PackageViewModel GetOrCreate(PackageInfo packageInfo)
        {
            return GetOrCreate(packageInfo.Id);
        }

        private static PackageViewModel GetOrCreate(string name)
        {
            if (name == null)
            {
                return null;
            }

            lock (Gate)
            {
                var match = Cache.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
                if (match == null)
                {
                    match = new PackageViewModel(name);
                    Cache.Add(match);
                }

                return match;
            }
        }
    }
}