namespace Paket.Ui.Csharp
{
    using System.IO;

    public static class RootDirectory
    {
        // Using global mutable state here, why not?
        public static DirectoryInfo Current { get; set; } = GetSelfDirectory();

        private static DirectoryInfo GetSelfDirectory()
        {
            // Hacking it quick and dirty for now.
            return new DirectoryInfo(@"C:\Git\Third Party\Paket.VisualStudio");
        }
    }
}