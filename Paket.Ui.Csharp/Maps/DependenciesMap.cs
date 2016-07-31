namespace Paket.Ui.Csharp
{
    using System.Windows;

    public static class DependenciesMap
    {
        public static readonly DependencyProperty ForKeyProperty = DependencyProperty.RegisterAttached(
            "ForKey", 
            typeof(object), 
            typeof(DependenciesMap),
            new PropertyMetadata(default(object)));

        public static void SetForKey(DependencyObject element, object value)
        {
            element.SetValue(ForKeyProperty, value);
        }

        public static object GetForKey(DependencyObject element)
        {
            return (object) element.GetValue(ForKeyProperty);
        }
    }
}
