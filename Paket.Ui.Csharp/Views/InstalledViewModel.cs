namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class InstalledViewModel : INotifyPropertyChanged
    {
        private IReadOnlyCollection<GroupInfo> groups;
        private IReadOnlyCollection<PackageInfo> packages;

        public InstalledViewModel()
        {
            this.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IReadOnlyCollection<GroupInfo> Groups
        {
            get { return this.groups; }
            private set
            {
                if (Equals(value, this.groups)) return;
                this.groups = value;
                this.OnPropertyChanged();
            }
        }

        public IReadOnlyCollection<PackageInfo> Packages
        {
            get { return this.packages; }
            private set
            {
                if (Equals(value, this.packages)) return;
                this.packages = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Refresh()
        {
            this.Groups = State.DependenciesFile
                                       ?.Groups.Select(g => new GroupInfo(g))
                                       .ToArray() ??
                                       new GroupInfo[0];

            this.Packages = this.Groups.SelectMany(g => g.Packages).ToArray();
        }
    }
}