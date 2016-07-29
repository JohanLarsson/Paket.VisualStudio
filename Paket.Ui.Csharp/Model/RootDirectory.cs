namespace Paket.Ui.Csharp
{
    using System.ComponentModel;
    using System.IO;
    using System.Windows;

    public static class RootDirectory
    {
        private static readonly DependencyObject DependencyObject = new DependencyObject();

        // Using global mutable state here, why not?
        public static DirectoryInfo Current { get; set; } = DesigntimeDirectory();

        private static DirectoryInfo DesigntimeDirectory()
        {
            if (DesignerProperties.GetIsInDesignMode(DependencyObject))
            {
                // Hacking it quick and dirty for now.
                return new DirectoryInfo(@"C:\Git\ThirdParty\Paket.VisualStudio");
            }

            return null;
        }
    }
}