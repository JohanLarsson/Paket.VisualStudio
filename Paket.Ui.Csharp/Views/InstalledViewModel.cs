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
        private PackageInfo selectedPackage;
        private DirectoryInfo rootDirectory;

        public InstalledViewModel(DirectoryInfo rootDirectory)
        {
            this.rootDirectory = rootDirectory;
            var dependencies = Paket.Dependencies.Locate(rootDirectory.FullName);
            var dependenciesFile = dependencies.GetDependenciesFile();
            Packages = dependenciesFile.Lines.Select(x => new PackageInfo(x))
                                       .ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IReadOnlyCollection<PackageInfo> Packages { get; }

        public PackageInfo SelectedPackage
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