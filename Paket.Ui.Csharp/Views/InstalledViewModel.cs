namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class InstalledViewModel : INotifyPropertyChanged
    {
        public static readonly InstalledViewModel Default = new InstalledViewModel(RootDirectory.Current);

        private Requirements.PackageRequirement selectedPackage;

        public InstalledViewModel(DirectoryInfo rootDirectory)
        {
            var dependencies = Paket.Dependencies.Locate(rootDirectory.FullName);
            var dependenciesFile = dependencies.GetDependenciesFile();
            Packages = dependenciesFile.Groups.SelectMany(g => g.Value.Packages)
                                       .ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IReadOnlyCollection<Requirements.PackageRequirement> Packages { get; }

        public Requirements.PackageRequirement SelectedPackage
        {
            get
            {
                return selectedPackage;
            }
            set
            {
                if (Equals(value, selectedPackage)) return;
                selectedPackage = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}