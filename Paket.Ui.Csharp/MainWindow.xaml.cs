namespace Paket.Ui.Csharp
{
    using System.IO;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            State.RootDirectory = new DirectoryInfo(this.GetType().Assembly.Location);
        }
    }
}
