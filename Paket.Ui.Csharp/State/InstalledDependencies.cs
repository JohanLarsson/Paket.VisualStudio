namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class InstalledDependencies : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<DependenciesGroup> Groups => State.DependenciesFile?.Groups.Select(g => g.Value);

        public IEnumerable<Requirements.PackageRequirement> Packages => this.Groups?.SelectMany(g => g.Packages);

        public IEnumerable<ModuleResolver.UnresolvedSource> RemoteFiles => this.Groups?.SelectMany(g => g.RemoteFiles);

        public IEnumerable<object> AllDependencies => this.Packages?.Concat<object>(this.RemoteFiles);

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
    }
}
