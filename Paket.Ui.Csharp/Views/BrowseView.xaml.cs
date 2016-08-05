namespace Paket.Ui.Csharp
{
    using System.Windows;
    using System.Windows.Controls;

    public partial class BrowseView : UserControl
    {
        public BrowseView()
        {
            this.InitializeComponent();
        }

        public BrowseViewModel SearchViewModel => this.DataContext as BrowseViewModel;

        private async void OnCloseToBottom(object sender, RoutedEventArgs e)
        {
            this.SearchViewModel?.FetchMorePackagesAsync();
        }
    }
}
