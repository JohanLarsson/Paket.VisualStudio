namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;

    public class PackageViewModel : DependencyViewModel
    {
        private static readonly List<PackageViewModel> Cache = new List<PackageViewModel>();
        private readonly PackageInstallSettings packageInstallSettings;
        private Requirements.PackageRequirement packageRequirement;

        private PackageViewModel(Requirements.PackageRequirement packageRequirement)
            : base(packageRequirement.Name.ToString())
        {
            this.packageRequirement = packageRequirement;
        }

        private PackageViewModel(PackageInstallSettings packageInstallSettings)
            :base(packageInstallSettings.Name.ToString())
        {
            this.packageInstallSettings = packageInstallSettings;
        }

        internal static PackageViewModel GetOrCreate(PackageInstallSettings installSettings)
        {
            if (installSettings == null)
            {
                return null;
            }

            var key = installSettings.Name.ToString();
            var match = Cache.Find(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                match = new PackageViewModel(installSettings);
                Cache.Add(match);
            }

            return match;
        }


        internal static PackageViewModel GetOrCreate(Requirements.PackageRequirement packageRequirement)
        {
            if (packageRequirement == null)
            {
                return null;
            }

            var key = packageRequirement.Name.ToString();
            var match = Cache.Find(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                match = new PackageViewModel(packageRequirement);
                Cache.Add(match);
            }

            return match;
        }
    }
}