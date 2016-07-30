namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    public partial class PackageListView : UserControl
    {
        public static readonly DependencyProperty PackagesProperty = DependencyProperty.Register(
            nameof(Packages),
            typeof(IEnumerable<object>),
            typeof(PackageListView),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register(
            nameof(SelectedPackage),
            typeof(object),
            typeof(PackageListView),
            new PropertyMetadata(default(object)));

        public PackageListView()
        {
            this.InitializeComponent();
        }

        public IEnumerable<object> Packages
        {
            get { return (IEnumerable<object>)this.GetValue(PackagesProperty); }
            set { this.SetValue(PackagesProperty, value); }
        }

        public object SelectedPackage
        {
            get { return (object)this.GetValue(SelectedPackageProperty); }
            set { this.SetValue(SelectedPackageProperty, value); }
        }
    }
}
