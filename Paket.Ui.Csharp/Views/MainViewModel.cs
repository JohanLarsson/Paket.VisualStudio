﻿namespace Paket.Ui.Csharp
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            this.Browse = new BrowseViewModel(State.RootDirectory);
            this.Installed = new InstalledViewModel();
        }

        public BrowseViewModel Browse { get; }

        public InstalledViewModel Installed { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
