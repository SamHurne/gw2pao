using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ExtensionLibrary.Bindings
{
    /// <summary>
    /// Markup Extension used for setting bindings to a container DataContext.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public class ContainerBindingExtension : CustomBindingBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBindingExtension"/> class.
        /// </summary>
        public ContainerBindingExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBindingExtension"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ContainerBindingExtension(string path)
            : base(path)
        {
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding.Source != null || Binding.ElementName != null || Binding.RelativeSource != null)
            {
                Debug.WriteLine("Binding source has already been set. ContainerDataContext will be ignored.");
            }
            else
            {
                Binding.RelativeSource = RelativeSource.Self;
                if (String.IsNullOrEmpty(Binding.Path.Path))
                {
                    Binding.Path = new PropertyPath("(FrameworkElementExtensions.ContainerDataContext)");
                }
                else
                {
                    Binding.Path = new PropertyPath(String.Format("(FrameworkElementExtensions.ContainerDataContext).{0}", Path.Path));
                }
            }

            return base.ProvideValue(serviceProvider);
        }
    }
}
