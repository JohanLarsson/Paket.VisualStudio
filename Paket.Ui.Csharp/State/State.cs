// ReSharper disable ExplicitCallerInfoArgument
namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class State
    {
        private static FileInfo solutionFile = DesigntimeSolutionFile();
        private static DependencyViewModel selectedDependency;
        private static ProjectViewModel selectedProject;
        private static ProjectFile[] projectFiles;

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        // Using global mutable state here, why not?
        public static FileInfo SolutionFile
        {
            get { return solutionFile; }
            set
            {
                solutionFile = value;
                Refresh();
            }
        }

        internal static ProjectFile[] ProjectFiles
        {
            get { return projectFiles; }
            private set
            {
                projectFiles = value;
                OnStaticPropertyChanged(nameof(Projects));
            }
        }

        public static IEnumerable<ProjectViewModel> Projects => ProjectFiles.Select(ProjectViewModel.GetOrCreate);

        public static DependenciesFile DependenciesFile => SolutionFile == null ? null : Dependencies.Locate(SolutionFile.DirectoryName).GetDependenciesFile();

        public static InstalledDependencies InstalledDependencies { get; } = new InstalledDependencies();

        public static LockFile LockFile
        {
            get
            {
                var lockfile = DependenciesFile?.FindLockfile();
                return lockfile == null ? null : LockFile.LoadFrom(lockfile.FullName);
            }
        }

        public static ProjectViewModel SelectedProject
        {
            get { return selectedProject; }
            set
            {
                if (selectedProject == value)
                {
                    return;
                }

                selectedProject = value;
                OnStaticPropertyChanged();
            }
        }

        public static DependencyViewModel SelectedDependency
        {
            get { return selectedDependency; }
            set
            {
                if (value == selectedDependency)
                {
                    return;
                }

                selectedDependency = value;
                OnStaticPropertyChanged();
            }
        }

        internal static void Refresh()
        {
            OnStaticPropertyChanged(nameof(SolutionFile));
            ProjectFiles = solutionFile == null ? new ProjectFile[0] : ProjectFile.FindAllProjects(SolutionFile.DirectoryName);
            OnStaticPropertyChanged(nameof(DependenciesFile));
            OnStaticPropertyChanged(nameof(LockFile));
        }

        private static FileInfo DesigntimeSolutionFile()
        {
            if (Is.InDesignMode)
            {
                // Hacking it quick and dirty for now.
                var sln = @"C:\Git\Third Party\Paket.VisualStudio\Paket.VisualStudio.sln";
                if (File.Exists(sln))
                {
                    var slnFile = new FileInfo(sln);
                    projectFiles = ProjectFile.FindAllProjects(slnFile.DirectoryName);
                    return slnFile;
                }

                else return null;
            }

            return null;
        }

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}