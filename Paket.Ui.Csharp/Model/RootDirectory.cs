﻿namespace Paket.Ui.Csharp
{
    using System.IO;

    public static class RootDirectory
    {

        // Using global mutable state here, why not?
        public static DirectoryInfo Current { get; set; } = DesigntimeDirectory();

        private static DirectoryInfo DesigntimeDirectory()
        {
            if (Is.InDesignMode)
            {
                // Hacking it quick and dirty for now.
                var dir = @"C:\Git\Third Party\Paket.VisualStudio";
                return Directory.Exists(dir) ? new DirectoryInfo(dir) : null;
            }

            return null;
        }
    }
}