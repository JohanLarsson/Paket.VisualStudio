﻿namespace Paket.Ui.Csharp
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using JetBrains.Annotations;

    public class BrowseViewModel : INotifyPropertyChanged
    {
        private string searchText;

        private bool isIncludingPreRelease;

        private string selectedPackageSource;

        private PackageInfo selectedPackage;
        private DirectoryInfo rootDirectory;

        public BrowseViewModel(DirectoryInfo rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RefreshCommand { get; }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (value == searchText) return;
                searchText = value;
                OnPropertyChanged();
            }
        }

        public bool IsIncludingPreRelease
        {
            get
            {
                return isIncludingPreRelease;
            }
            set
            {
                if (value == isIncludingPreRelease) return;
                isIncludingPreRelease = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PackageSources { get; }

        public string SelectedPackageSource
        {
            get
            {
                return selectedPackageSource;
            }
            set
            {
                if (value == selectedPackageSource) return;
                selectedPackageSource = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PackageInfo> FeedPackages { get; }

        public PackageInfo SelectedPackage
        {
            get
            {
                return selectedPackage;
            }
            set
            {
                if (Equals(value, selectedPackage)) return;
                selectedPackage = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
