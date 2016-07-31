namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using JetBrains.Annotations;

    public static class ProjectDependenciesMap
    {
        private static readonly ProjectFile UnsetProjectFile = new ProjectFile(null, null, null, null, null);
        private static readonly Dictionary<ProjectFile, Dependencies> Map = new Dictionary<ProjectFile, Dependencies>(ProjectFileNameComparer.Default);

        public static readonly DependencyProperty ForProjectProperty = DependencyProperty.RegisterAttached(
            "ForProject",
            typeof(ProjectFile),
            typeof(ProjectDependenciesMap),
            new PropertyMetadata(UnsetProjectFile, OnForProjectChanged));

        private static readonly DependencyPropertyKey DependenciesPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "Dependencies",
            typeof(Dependencies),
            typeof(ProjectDependenciesMap),
            new PropertyMetadata(new Dependencies(UnsetProjectFile)));

        public static readonly DependencyProperty DependenciesProperty = DependenciesPropertyKey.DependencyProperty;

        public static void SetForProject(DependencyObject element, ProjectFile value)
        {
            element.SetValue(ForProjectProperty, value);
        }

        public static ProjectFile GetForProject(DependencyObject element)
        {
            return (ProjectFile)element.GetValue(ForProjectProperty);
        }

        private static void SetDependencies(this DependencyObject element, Dependencies value)
        {
            element.SetValue(DependenciesPropertyKey, value);
        }

        public static Dependencies GetDependencies(DependencyObject element)
        {
            return (Dependencies)element.GetValue(DependenciesProperty);
        }

        private static void OnForProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                d.SetDependencies(null);
                return;
            }

            var projectFile = (ProjectFile)e.NewValue;
            Dependencies dependencies;
            if (!Map.TryGetValue(projectFile, out dependencies))
            {
                dependencies = new Dependencies(projectFile);
                Map[projectFile] = dependencies;
            }

            d.SetDependencies(dependencies);
        }

        public sealed class Dependencies : INotifyPropertyChanged
        {
            private readonly ProjectFile project;
            private IEnumerable<InstallGroup> groups;

            public Dependencies(ProjectFile project)
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
                if (ReferenceEquals(this.project, UnsetProjectFile))
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
                if (ReferenceEquals(this.project, UnsetProjectFile))
                {
                    // ~compiletime~ check, will throw in the designer.
                    var message = "ProjectDependenciesMap.ForProject must be set before getting dependencies.\r\n" +
                                  "example local:ProjectDependenciesMap.ForProject=\"{Binding Project}\"";
                    throw new InvalidOperationException(message);
                }
            }
        }

        private class ProjectFileNameComparer : IEqualityComparer<ProjectFile>
        {
            private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

            public static readonly ProjectFileNameComparer Default = new ProjectFileNameComparer();

            bool IEqualityComparer<ProjectFile>.Equals(ProjectFile x, ProjectFile y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return StringComparer.Equals(x.FileName, y.FileName);
            }

            int IEqualityComparer<ProjectFile>.GetHashCode(ProjectFile obj)
            {
                return StringComparer.GetHashCode(obj.FileName);
            }
        }
    }
}
