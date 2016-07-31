namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public sealed class ProjectViewModel : INotifyPropertyChanged
    {
        private readonly string project;
        private static readonly ConcurrentDictionary<string, ProjectViewModel> Map = new ConcurrentDictionary<string, ProjectViewModel>(StringComparer.OrdinalIgnoreCase);

        private ProjectViewModel(string project)
        {
            this.project = project;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectFile Project => State.ProjectFiles.Single(p => p.FileName == this.project);

        public IEnumerable<InstallGroup> Groups
        {
            get
            {
                var referenceFile = this.Project?.FindReferencesFile().ValueOrNull();
                return referenceFile == null
                    ? new InstallGroup[0]
                    : ReferencesFile.FromFile(referenceFile).Groups.Select(x => x.Value);
            }
        }

        public IEnumerable<PackageInstallSettings> Packages => this.Groups?.SelectMany(x => x.NugetPackages);

        public IEnumerable<RemoteFileReference> RemoteFiles => this.Groups?.SelectMany(x => x.RemoteFiles);

        public IEnumerable<object> AllDependencies => this.Packages?.Concat<object>(this.RemoteFiles);

        internal static ProjectViewModel GetOrCreate(ProjectFile projectFile)
        {
            if (projectFile == null)
            {
                return null;
            }

            return Map.GetOrAdd(projectFile.FileName, x => new ProjectViewModel(x));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Refresh()
        {
            this.OnPropertyChanged(nameof(this.Project));
            this.OnPropertyChanged(nameof(this.Groups));
            this.OnPropertyChanged(nameof(this.Packages));
            this.OnPropertyChanged(nameof(this.RemoteFiles));
        }
    }
}