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
        private static DirectoryInfo rootDirectory = DesigntimeDirectory();
        private static PackageInfo selectedPackage;

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        // Using global mutable state here, why not?
        public static DirectoryInfo RootDirectory
        {
            get { return rootDirectory; }
            set
            {
                rootDirectory = value;
                NotifyRefresh();
            }
        }

        public static IReadOnlyList<ProjectFile> Projects => RootDirectory == null ? new ProjectFile[0] : ProjectFile.FindAllProjects(RootDirectory.FullName);

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

        internal static DependenciesFile DependenciesFile => RootDirectory == null ? null : Dependencies.Locate(RootDirectory.FullName).GetDependenciesFile();

        internal static LockFile LockFile
        {
            get
            {
                var lockfile = DependenciesFile?.FindLockfile();
                return lockfile == null ? null : LockFile.LoadFrom(lockfile.FullName);
            }
        }

        internal static void NotifyRefresh()
        {
            OnStaticPropertyChanged(nameof(RootDirectory));
            OnStaticPropertyChanged(nameof(Projects));
            OnStaticPropertyChanged(nameof(DependenciesFile));
        }

        private static DirectoryInfo DesigntimeDirectory()
        {
            if (Is.InDesignMode)
            {
                // Hacking it quick and dirty for now.
                var dir = @"C:\Git\Third Party\Paket.VisualStudio";
                return Directory.Exists(dir) ? new DirectoryInfo(dir) : null;
            }

            return null;
        }

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}