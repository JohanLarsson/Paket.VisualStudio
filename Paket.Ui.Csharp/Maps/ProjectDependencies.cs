namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public sealed class ProjectDependencies : INotifyPropertyChanged
    {
        private readonly ProjectFile project;
        private IEnumerable<InstallGroup> groups;

        public ProjectDependencies(ProjectFile project)
        {
            this.project = project;
            this.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<InstallGroup> Groups
        {
            get
            {
                //this.VerifyProjectFile(); Could not get this to work
                return this.groups;
            }
            private set
            {
                if (Equals(value, this.groups)) return;
                this.groups = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<PackageInstallSettings> Packages => this.Groups?.SelectMany(x => x.NugetPackages);

        public IEnumerable<RemoteFileReference> RemoteFiles => this.Groups?.SelectMany(x => x.RemoteFiles);

        public IEnumerable<object> AllDependencies => this.Packages?.Concat<object>(this.RemoteFiles);

        private void Refresh()
        {
            if (ReferenceEquals(this.project, ProjectDependenciesMap.NotSetProjectFile))
            {
                return;
            }

            var referenceFile = this.project?.FindReferencesFile().ValueOrNull();
            this.Groups = referenceFile == null
                ? new InstallGroup[0]
                : ReferencesFile.FromFile(referenceFile).Groups.Select(x => x.Value);

            this.OnPropertyChanged(nameof(this.Packages));
            this.OnPropertyChanged(nameof(this.RemoteFiles));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void VerifyProjectFile()
        {
            if (ReferenceEquals(this.project, ProjectDependenciesMap.NotSetProjectFile))
            {
                // ~compiletime~ check, will throw in the designer.
                var message = "ProjectDependenciesMap.ForProject must be set before getting dependencies.\r\n" +
                              "example local:ProjectDependenciesMap.ForProject=\"{Binding Project}\"";
                throw new InvalidOperationException(message);
            }
        }
    }
}