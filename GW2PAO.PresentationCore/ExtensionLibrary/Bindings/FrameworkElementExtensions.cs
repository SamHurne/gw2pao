using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "ExtensionLibrary.Bindings")]
namespace ExtensionLibrary.Bindings
{
    public static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty ContainerDataContextProperty = DependencyProperty.RegisterAttached(
            "ContainerDataContext",
            typeof(object),
            typeof(FrameworkElementExtensions),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnContainerDataContextChanged));

        public static object GetContainerDataContext(DependencyObject target)
        {
            return target.GetValue(ContainerDataContextProperty);
        }

        public static void SetContainerDataContext(DependencyObject target, object value)
        {
            target.SetValue(ContainerDataContextProperty, value);
        }

        private static void OnContainerDataContextChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            OnContainerDataContextChanged(dObj, e.OldValue, e.NewValue);
        }

        private static void OnContainerDataContextChanged(DependencyObject dObj, object oldValue, object newValue)
        {
        }

        public static readonly DependencyProperty IsContextContainerProperty = DependencyProperty.RegisterAttached(
            "IsContextContainer",
            typeof(bool),
            typeof(FrameworkElementExtensions),
            new UIPropertyMetadata(false, OnIsContextContainerChanged));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [DisplayName(@"IsContextContainer")]
        [Category("Common Properties")]
        [Description("If set this item's Data Context is used as the new Container Context.")]
        public static bool GetIsContextContainer(DependencyObject target)
        {
            return (bool)target.GetValue(IsContextContainerProperty);
        }

        public static void SetIsContextContainer(DependencyObject target, bool value)
        {
            target.SetValue(IsContextContainerProperty, value);
        }

        private static void OnIsContextContainerChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            OnIsContextContainerChanged(dObj, (bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnIsContextContainerChanged(DependencyObject dObj, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                BindingOperations.SetBinding(dObj, ContainerDataContextProperty, new Binding("DataContext") { Source = dObj });
            }
        }
    }
}