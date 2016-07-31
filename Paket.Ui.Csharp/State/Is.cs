namespace Paket.Ui.Csharp
{
    using System.ComponentModel;
    using System.Windows;

    internal static class Is
    {
        private static readonly DependencyObject DependencyObject = new DependencyObject();

        public static bool InDesignMode = DesignerProperties.GetIsInDesignMode(DependencyObject);
    }
}