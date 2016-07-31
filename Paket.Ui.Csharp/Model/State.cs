// ReSharper disable ExplicitCallerInfoArgument
namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class State
    {
        private static FileInfo solutionFile = DesigntimeSolutionFile();
        private static PackageInfo selectedPackage;
        private static ProjectFile selectedProject;

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        // Using global mutable state here, why not?
        public static FileInfo SolutionFile
        {
            get { return solutionFile; }
            set
            {
                solutionFile = value;
                NotifyRefresh();
            }
        }

        public static IReadOnlyList<ProjectFile> Projects => SolutionFile == null ? new ProjectFile[0] : ProjectFile.FindAllProjects(SolutionFile.DirectoryName);

        public static DependenciesFile DependenciesFile => SolutionFile == null ? null : Dependencies.Locate(SolutionFile.DirectoryName).GetDependenciesFile();

        public static LockFile LockFile
        {
            get
            {
                var lockfile = DependenciesFile?.FindLockfile();
                return lockfile == null ? null : LockFile.LoadFrom(lockfile.FullName);
            }
        }

        public static ProjectFile SelectedProject
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

        public static PackageInfo SelectedPackage
        {
            get { return selectedPackage; }
            set
            {
                if (value == selectedPackage)
                {
                    return;
                }

                selectedPackage = value;
                OnStaticPropertyChanged();
            }
        }


        internal static void NotifyRefresh()
        {
            OnStaticPropertyChanged(nameof(SolutionFile));
            OnStaticPropertyChanged(nameof(Projects));
            OnStaticPropertyChanged(nameof(DependenciesFile));
            OnStaticPropertyChanged(nameof(LockFile));
        }

        private static FileInfo DesigntimeSolutionFile()
        {
            if (Is.InDesignMode)
            {
                // Hacking it quick and dirty for now.
                var sln = @"C:\Git\Third Party\Paket.VisualStudio\Paket.sln";
                return File.Exists(sln) ? new FileInfo(sln) : null;
            }

            return null;
        }

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}