namespace Paket.Ui.Csharp
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public abstract class DependencyViewModel : INotifyPropertyChanged
    {
        protected DependencyViewModel(string name)
        {
            this.Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; }

        public abstract string Version { get; }

        public abstract DependencyInfo Info { get; }

        public abstract bool IsFavorite { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}