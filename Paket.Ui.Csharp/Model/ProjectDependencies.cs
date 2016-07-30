namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    public static class ProjectDependencies
    {
        private static readonly List<ProjectDependenciesPair> Map = new List<ProjectDependenciesPair>();

        public static readonly DependencyProperty ForProjectProperty = DependencyProperty.RegisterAttached(
            "ForProject",
            typeof(ProjectFile),
            typeof(ProjectDependencies),
            new PropertyMetadata(default(ProjectFile), OnForProjectChanged));

        private static readonly DependencyPropertyKey DependenciesPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "Dependencies",
            typeof(ReadOnlyObservableCollection<string>),
            typeof(ProjectDependencies),
            new PropertyMetadata(default(ReadOnlyObservableCollection<string>)));

        public static readonly DependencyProperty DependenciesProperty = DependenciesPropertyKey.DependencyProperty;

        public static void SetForProject(DependencyObject element, ProjectFile value)
        {
            element.SetValue(ForProjectProperty, value);
        }

        public static ProjectFile GetForProject(DependencyObject element)
        {
            return (ProjectFile)element.GetValue(ForProjectProperty);
        }

        private static void SetDependencies(this DependencyObject element, ReadOnlyObservableCollection<string> value)
        {
            element.SetValue(DependenciesPropertyKey, value);
        }

        public static ReadOnlyObservableCollection<string> GetDependencies(DependencyObject element)
        {
            return (ReadOnlyObservableCollection<string>)element.GetValue(DependenciesProperty);
        }

        private static void OnForProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                d.SetDependencies(null);
            }

            var projectFile = (ProjectFile)e.NewValue;
            var match = Map.SingleOrDefault(x => x.Project.FileName == projectFile.FileName) ??
                        new ProjectDependenciesPair(projectFile);

            d.SetDependencies(match.Dependencies);
        }

        // ReSharper disable once InconsistentNaming
        private class ProjectDependenciesPair
        {
            internal readonly ProjectFile Project;
            internal readonly ReadOnlyObservableCollection<string> Dependencies;
            private readonly ObservableCollection<string> innerDependencies;

            public ProjectDependenciesPair(ProjectFile project)
            {
                this.Project = project;
                this.innerDependencies = new ObservableCollection<string>(FindDependencies(project));
                this.Dependencies = new ReadOnlyObservableCollection<string>(this.innerDependencies);
            }

            internal void UpdateDependencies()
            {
                var dependencies = FindDependencies(this.Project);
                if (dependencies == null)
                {
                    this.innerDependencies.Clear();
                    return;
                }

                if (Enumerable.SequenceEqual(this.innerDependencies, dependencies))
                {
                    return;
                }

                this.innerDependencies.Clear();
                foreach (var dependency in dependencies)
                {
                    this.innerDependencies.Add(dependency);
                }
            }

            private static IEnumerable<string> FindDependencies(ProjectFile project)
            {
                var file = project.FindReferencesFile().ValueOrNull();
                if (file == null)
                {
                    return Enumerable.Empty<string>();
                }

                return ReferencesFile.FromFile(file)
                                     .Groups.SelectMany(x => x.Value.NugetPackages)
                                     .Select(x => x.Name.Item1);
            }
        }
    }
}
