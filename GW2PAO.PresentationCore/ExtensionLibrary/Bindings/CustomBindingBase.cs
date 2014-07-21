using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ExtensionLibrary.Bindings
{
    /// <summary>
    /// Base class for custom Binding MarkupExtensions.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class CustomBindingBase : MarkupExtension
    {
        protected CustomBindingBase()
        {
            Binding = new Binding();
        }

        protected CustomBindingBase(string path)
        {
            Binding = new Binding(path);
        }

        [Browsable(false)]
        protected Binding Binding { get; set; }

        #region Binding Property Wrappers

        #region Sources
        /// <summary>
        /// Gets or sets the object to use as the binding source.
        /// </summary>
        /// <value>The source.</value>
        [DefaultValue(null)]
        public object Source
        {
            get { return Binding.Source; }
            set { Binding.Source = value; }
        }

        /// <summary>
        /// Gets or sets the name of the element to use as the binding source object.
        /// </summary>
        /// <value>The name of the element.</value>
        [DefaultValue((string)null)]
        public string ElementName
        {
            get { return Binding.ElementName; }
            set { Binding.ElementName = value; }
        }

        /// <summary>
        /// Gets or sets the binding source by specifying its location relative to the position of the binding target.
        /// </summary>
        /// <value>The relative source.</value>
        [DefaultValue((string)null)]
        public RelativeSource RelativeSource
        {
            get { return Binding.RelativeSource; }
            set { Binding.RelativeSource = value; }
        }
        #endregion

        #region Paths
        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// </summary>
        /// <value>The path.</value>
        [DefaultValue(null)]
        public PropertyPath Path
        {
            get { return Binding.Path; }
            set { Binding.Path = value; }
        }

        /// <summary>
        /// Gets or sets an XPath query that returns the value on the XML binding source to use.
        /// </summary>
        /// <value>The XPath query.</value>
        [DefaultValue((string)null)]
        public string XPath
        {
            get { return Binding.XPath; }
            set { Binding.XPath = value; }
        }
        #endregion

        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// </summary>
        /// <value>The mode.</value>
        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode
        {
            get { return Binding.Mode; }
            set { Binding.Mode = value; }
        }

        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
        /// </summary>
        /// <value>The string format.</value>
        [DefaultValue((string)null)]
        public string StringFormat
        {
            get { return Binding.StringFormat; }
            set { Binding.StringFormat = value; }
        }

        /// <summary>
        /// Gets or sets the name of the System.Windows.Data.BindingGroup to which this binding belongs.
        /// </summary>
        /// <value>The name of the binding group.</value>
        [DefaultValue("")]
        public string BindingGroupName
        {
            get { return Binding.BindingGroupName; }
            set { Binding.BindingGroupName = value; }
        }

        #region Async
        /// <summary>
        /// Gets or sets a value that indicates whether the System.Windows.Data.Binding should get and set values asynchronously.
        /// </summary>
        /// <value><c>true</c> if this instance is async; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool IsAsync
        {
            get { return Binding.IsAsync; }
            set { Binding.IsAsync = value; }
        }

        /// <summary>
        /// Gets or sets the state data for an async operation.
        /// </summary>
        /// <value>The async state data.</value>
        [DefaultValue((string)null)]
        public object AsyncState
        {
            get { return Binding.AsyncState; }
            set { Binding.AsyncState = value; }
        }
        #endregion

        /// <summary>
        /// Gets or sets a value that indicates whether to evaluate the System.Windows.Data.Binding.Path relative to the data item or the System.Windows.Data.DataSourceProvider object.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if binds directly to source; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool BindsDirectlyToSource
        {
            get { return Binding.BindsDirectlyToSource; }
            set { Binding.BindsDirectlyToSource = value; }
        }

        #region Defaults
        /// <summary>
        /// Gets or sets the value that is used in the target when the value of the source is null.
        /// </summary>
        /// <value>The target null value.</value>
        [DefaultValue(null)]
        public object TargetNullValue
        {
            get { return Binding.TargetNullValue; }
            set { Binding.TargetNullValue = value; }
        }

        /// <summary>
        /// Gets or sets the value to use when the binding is unable to return a value.
        /// </summary>
        /// <value>The fallback value.</value>
        public object FallbackValue
        {
            get { return Binding.FallbackValue; }
            set { Binding.FallbackValue = value; }
        }
        #endregion

        #region Converter
        /// <summary>
        /// Gets or sets the converter to use.
        /// </summary>
        /// <value>The converter.</value>
        [DefaultValue((string)null)]
        public IValueConverter Converter
        {
            get { return Binding.Converter; }
            set { Binding.Converter = value; }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the System.Windows.Data.Binding.Converter.
        /// </summary>
        /// <value>The converter parameter.</value>
        [DefaultValue((string)null)]
        public object ConverterParameter
        {
            get { return Binding.ConverterParameter; }
            set { Binding.ConverterParameter = value; }
        }

        /// <summary>
        /// Gets or sets the culture in which to evaluate the converter.
        /// </summary>
        /// <value>The converter culture.</value>
        [DefaultValue((string)null), TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture
        {
            get { return Binding.ConverterCulture; }
            set { Binding.ConverterCulture = value; }
        }
        #endregion

        #region Notifications
        /// <summary>
        /// Gets or sets a value that indicates whether to raise the System.Windows.Data.Binding.SourceUpdated event when a value is transferred from the binding target to the binding source.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if notify on source updated; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated
        {
            get { return Binding.NotifyOnSourceUpdated; }
            set { Binding.NotifyOnSourceUpdated = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to raise the System.Windows.Data.Binding.TargetUpdated event when a value is transferred from the binding source to the binding target.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if notify on target updated; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated
        {
            get { return Binding.NotifyOnTargetUpdated; }
            set { Binding.NotifyOnTargetUpdated = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to raise the System.Windows.Controls.Validation.Error attached event on the bound object.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if notify on validation error; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool NotifyOnValidationError
        {
            get { return Binding.NotifyOnValidationError; }
            set { Binding.NotifyOnValidationError = value; }
        }
        #endregion

        #region UpdateSource
        /// <summary>
        /// Gets or sets a value that determines the timing of binding source updates.
        /// </summary>
        /// <value>The update source trigger.</value>
        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return Binding.UpdateSourceTrigger; }
            set { Binding.UpdateSourceTrigger = value; }
        }

        /// <summary>
        /// Gets or sets a handler you can use to provide custom logic for handling exceptions
        /// that the binding engine encounters during the update of the binding source value.
        /// This is only applicable if you have associated an System.Windows.Controls.ExceptionValidationRule with your binding.
        /// </summary>
        /// <value>The update source exception filter.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
        {
            get { return Binding.UpdateSourceExceptionFilter; }
            set { Binding.UpdateSourceExceptionFilter = value; }
        }
        #endregion

        #region Validation
        /// <summary>
        /// Gets or sets a value that indicates whether to include the System.Windows.Controls.DataErrorValidationRule.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if validates on data errors; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool ValidatesOnDataErrors
        {
            get { return Binding.ValidatesOnDataErrors; }
            set { Binding.ValidatesOnDataErrors = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the System.Windows.Controls.ExceptionValidationRule.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if validates on exceptions; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool ValidatesOnExceptions
        {
            get { return Binding.ValidatesOnExceptions; }
            set { Binding.ValidatesOnExceptions = value; }
        }

        /// <summary>
        /// Gets a collection of rules that check the validity of the user input.
        /// </summary>
        /// <value>The validation rules.</value>
        [DefaultValue(null)]
        public Collection<ValidationRule> ValidationRules
        {
            get { return Binding.ValidationRules; }
        }
        #endregion

        #endregion

        /// <summary>
        /// Provides the value for the markup extension.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider provider)
        {
            return Binding.ProvideValue(provider);
        }
    }
}
