namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    public partial class PackageListView : UserControl
    {
        public static readonly DependencyProperty PackagesProperty = DependencyProperty.Register(
            nameof(Packages),
            typeof(IEnumerable<PackageInfo>),
            typeof(PackageListView),
            new PropertyMetadata(default(IEnumerable<PackageInfo>)));

        public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register(
            nameof(SelectedPackage),
            typeof(PackageInfo),
            typeof(PackageListView),
            new PropertyMetadata(default(PackageInfo)));

        public PackageListView()
        {
            InitializeComponent();
        }

        public IEnumerable<PackageInfo> Packages
        {
            get { return (IEnumerable<PackageInfo>)this.GetValue(PackagesProperty); }
            set { this.SetValue(PackagesProperty, value); }
        }

        public PackageInfo SelectedPackage
        {
            get { return (PackageInfo)this.GetValue(SelectedPackageProperty); }
            set { this.SetValue(SelectedPackageProperty, value); }
        }
    }
}
