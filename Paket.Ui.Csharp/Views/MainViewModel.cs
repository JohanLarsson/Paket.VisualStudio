namespace Paket.Ui.Csharp
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Browse = new BrowseViewModel(RootDirectory.Current);
            Installed = new InstalledViewModel(RootDirectory.Current);
        }

        public BrowseViewModel Browse { get; }

        public InstalledViewModel Installed { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
