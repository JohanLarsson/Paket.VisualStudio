namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public static class ProjectDependenciesMap
    {
        internal static readonly ProjectFile NotSetProjectFile = new ProjectFile(null, null, null, null, null);

        private static readonly Dictionary<ProjectFile, ProjectDependencies> Map = new Dictionary<ProjectFile, ProjectDependencies>(ProjectFileNameComparer.Default);

        public static readonly DependencyProperty ForProjectProperty = DependencyProperty.RegisterAttached(
            "ForProject",
            typeof(ProjectFile),
            typeof(ProjectDependenciesMap),
            new PropertyMetadata(NotSetProjectFile, OnForProjectChanged));

        private static readonly DependencyPropertyKey DependenciesPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "Dependencies",
            typeof(ProjectDependencies),
            typeof(ProjectDependenciesMap),
            new PropertyMetadata(new ProjectDependencies(NotSetProjectFile)));

        public static readonly DependencyProperty DependenciesProperty = DependenciesPropertyKey.DependencyProperty;

        public static void SetForProject(DependencyObject element, ProjectFile value)
        {
            element.SetValue(ForProjectProperty, value);
        }

        public static ProjectFile GetForProject(DependencyObject element)
        {
            return (ProjectFile)element.GetValue(ForProjectProperty);
        }

        private static void SetDependencies(this DependencyObject element, ProjectDependencies value)
        {
            element.SetValue(DependenciesPropertyKey, value);
        }

        public static ProjectDependencies GetDependencies(DependencyObject element)
        {
            return (ProjectDependencies)element.GetValue(DependenciesProperty);
        }

        private static void OnForProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                d.SetDependencies(null);
                return;
            }

            var projectFile = (ProjectFile)e.NewValue;
            ProjectDependencies projectDependencies;
            if (!Map.TryGetValue(projectFile, out projectDependencies))
            {
                projectDependencies = new ProjectDependencies(projectFile);
                Map[projectFile] = projectDependencies;
            }

            d.SetDependencies(projectDependencies);
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
