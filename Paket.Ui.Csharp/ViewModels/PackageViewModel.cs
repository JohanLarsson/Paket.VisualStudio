namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;

    public class PackageViewModel : DependencyViewModel
    {
        private static readonly List<PackageViewModel> Cache = new List<PackageViewModel>();

        private PackageViewModel(string name)
            : base(name)
        {
        }

        internal static PackageViewModel GetOrCreate(PackageInstallSettings installSettings)
        {
            return GetOrCreate(installSettings?.Name.ToString());
        }

        internal static PackageViewModel GetOrCreate(Requirements.PackageRequirement packageRequirement)
        {
            return GetOrCreate(packageRequirement?.Name.ToString());
        }

        private static PackageViewModel GetOrCreate(string name)
        {
            if (name == null)
            {
                return null;
            }

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