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
        private DependencyInfo selectedDependency;
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
                return this.searchText;
            }
            set
            {
                if (value == this.searchText) return;
                this.searchText = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsIncludingPreRelease
        {
            get
            {
                return this.isIncludingPreRelease;
            }
            set
            {
                if (value == this.isIncludingPreRelease) return;
                this.isIncludingPreRelease = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PackageSources { get; }

        public string SelectedPackageSource
        {
            get
            {
                return this.selectedPackageSource;
            }
            set
            {
                if (value == this.selectedPackageSource) return;
                this.selectedPackageSource = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<DependencyInfo> FeedPackages { get; }

        public DependencyInfo SelectedDependency
        {
            get
            {
                return this.selectedDependency;
            }
            set
            {
                if (Equals(value, this.selectedDependency)) return;
                this.selectedDependency = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
