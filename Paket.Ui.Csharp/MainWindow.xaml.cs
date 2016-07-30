namespace Paket.Ui.Csharp
{
    using System;
    using System.IO;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var uri = new Uri(this.GetType().Assembly.CodeBase);
            var paketVisualstudio = "Paket.VisualStudio";
            var path = uri.LocalPath.Split(new [] { paketVisualstudio}, StringSplitOptions.None)[0] + paketVisualstudio;
            State.RootDirectory = new DirectoryInfo(path);
        }
    }
}
