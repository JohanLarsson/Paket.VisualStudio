namespace Paket.Ui.Csharp
{
    using System.IO;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            RootDirectory.Current = new DirectoryInfo(GetType().Assembly.Location);
        }
    }
}
