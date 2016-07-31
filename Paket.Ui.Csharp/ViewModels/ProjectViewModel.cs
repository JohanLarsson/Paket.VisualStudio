namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public sealed class ProjectViewModel : INotifyPropertyChanged
    {
        private static readonly List<ProjectViewModel> Cache = new List<ProjectViewModel>();
        private static readonly InstallGroup[] EmptyInstallGroups = new InstallGroup[0];

        private readonly string projectFileName;
        private ProjectFile project1;
        private IReadOnlyList<InstallGroup> groups;
        private IReadOnlyList<PackageViewModel> packages;
        private IReadOnlyList<RemoteFileViewModel> remoteFiles;
        private IEnumerable<object> allDependencies;
        private ReferencesFile referenceFile;

        static ProjectViewModel()
        {
            State.StaticPropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(State.SelectedDependency):
                        {
                            foreach (var vm in Cache)
                            {
                                //vm.OnPropertyChanged(nameof(HasSelectedDependency));
                            }

                            break;
                        }
                }
            };
        }

        private ProjectViewModel(string projectFileName)
        {
            this.projectFileName = projectFileName;
            this.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectFile Project
        {
            get { return this.project1; }
            private set
            {
                if (Equals(value, this.project1)) return;
                this.project1 = value;
                this.OnPropertyChanged();
            }
        }

        public IReadOnlyList<InstallGroup> Groups
        {
            get { return this.groups; }
            private set
            {
                if (Equals(value, this.groups)) return;
                this.groups = value;
                this.OnPropertyChanged();
            }
        }

        public IReadOnlyList<PackageViewModel> Packages
        {
            get { return this.packages; }
            private set
            {
                if (Equals(value, this.packages)) return;
                this.packages = value;
                this.OnPropertyChanged();
            }
        }

        public IReadOnlyList<RemoteFileViewModel> RemoteFiles
        {
            get { return this.remoteFiles; }
            private set
            {
                if (Equals(value, this.remoteFiles)) return;
                this.remoteFiles = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<object> AllDependencies
        {
            get { return this.allDependencies; }
            private set
            {
                if (Equals(value, this.allDependencies)) return;
                this.allDependencies = value;
                this.OnPropertyChanged();
            }
        }

        internal static ProjectViewModel GetOrCreate(ProjectFile projectFile)
        {
            if (projectFile == null)
            {
                return null;
            }

            var match = Cache.SingleOrDefault(x => string.Equals(x.projectFileName, projectFile.FileName, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                match = new ProjectViewModel(projectFile.FileName);
                Cache.Add(match);
            }

            return match;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Refresh()
        {
            if (!File.Exists(this.projectFileName))
            {
                Cache.Remove(this);
                return;
            }

            this.Project = State.ProjectFiles.Single(p => p.FileName == this.projectFileName);
            var referencesFileName = this.Project.FindReferencesFile().ValueOrNull();
            this.referenceFile = referencesFileName == null
                ? null
                : ReferencesFile.FromFile(referencesFileName);
            this.Groups = this.referenceFile?.Groups.Select(x => x.Value).ToArray() ?? EmptyInstallGroups;

            this.Packages = this.Groups.SelectMany(x => x.NugetPackages)
                                       .Select(PackageViewModel.GetOrCreate)
                                       .ToArray();

            this.RemoteFiles = this.Groups.SelectMany(x => x.RemoteFiles)
                                   .Select(RemoteFileViewModel.GetOrCreate)
                                   .ToArray();

            this.AllDependencies = this.Packages?.Concat<object>(this.RemoteFiles);
        }
    }
}