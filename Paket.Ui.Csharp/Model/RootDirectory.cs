namespace Paket.Ui.Csharp
{
    using System.IO;

    public static class RootDirectory
    {
        public static DirectoryInfo Current { get; } = GetSelfDirectory();


        private static DirectoryInfo GetSelfDirectory()
        {
            // Hacking it quick and dirty for now.
            return new DirectoryInfo(@"C:\Git\Third Party\Paket.VisualStudio");
        }
    }
}