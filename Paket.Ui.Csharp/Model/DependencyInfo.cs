namespace Paket.Ui.Csharp
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class DependencyInfo : INotifyPropertyChanged
    {
        public DependencyInfo(string id)
        {
            this.Id = id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}