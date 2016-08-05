namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public class InstalledDependencies : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public InstalledDependencies()
        {
            State.StaticPropertyChanged += async (_, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(State.DependenciesFile):
                        await this.UpdatePackageInfosAsync().ConfigureAwait(false);
                        break;
                }
            };

            this.UpdatePackageInfosAsync();
        }

        public IEnumerable<DependenciesGroup> Groups => State.DependenciesFile?.Groups.Select(g => g.Value);

        public IEnumerable<PackageViewModel> Packages => this.Groups?.SelectMany(g => g.Packages).Select(PackageViewModel.GetOrCreate);

        public IEnumerable<RemoteFileViewModel> RemoteFiles => this.Groups?.SelectMany(g => g.RemoteFiles).Select(RemoteFileViewModel.GetOrCreate);

        public IEnumerable<DependencyViewModel> AllDependencies => this.Packages?.Concat<DependencyViewModel>(this.RemoteFiles);

        internal void Refresh()
        {
            this.OnPropertyChanged(nameof(this.Groups));
            this.OnPropertyChanged(nameof(this.Packages));
            this.OnPropertyChanged(nameof(this.RemoteFiles));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task UpdatePackageInfosAsync()
        {
            if (this.Packages == null)
            {
                return;
            }

            // calling for side effect here, when downloaded package cache notifies about updates.
            await Task.WhenAll(this.Packages.Select(x => Nuget.GetPackageInfosAsync($"id:{x.Name}")))
                    .ConfigureAwait(false);
        }
    }
}
