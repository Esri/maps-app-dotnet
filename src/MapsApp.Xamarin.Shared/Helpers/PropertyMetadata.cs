using Xamarin.Forms;

namespace Esri.ArcGISRuntime.Internal
{
    /// <summary>
    /// Defines the default value and property changed callback for a BindableProperty
    /// </summary>
    internal class PropertyMetadata
    {
        private PropertyChangedCallback m_dependencyPropertyChanged;

        /// <summary>
        /// Instantiates a PropertyMetadata object with the default value specfied
        /// </summary>
        internal PropertyMetadata(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Instantiates a PropertyMetadata object with the default value and property changed handler specfied
        /// </summary>
        internal PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChanged) : this(defaultValue)
        {
            m_dependencyPropertyChanged = propertyChanged;
            BindablePropertyChanged = OnBindablePropertyChanged;
        }

        /// <summary>
        /// Gets the BindableProperty with which the PropertyMetadata object is associated
        /// </summary>
        internal BindableProperty Property { get; set; }

        /// <summary>
        /// Gets the default value for the BindableProperty
        /// </summary>
        internal object DefaultValue { get; private set; }

        /// <summary>
        /// Gets the delegate that is invoked when the bindable property's value has changed
        /// </summary>
        internal BindableProperty.BindingPropertyChangedDelegate BindablePropertyChanged { get; private set; }

        // Handles the change callback from the BindableProperty associated with the metadata instance
        private void OnBindablePropertyChanged(BindableObject b, object oldValue, object newValue)
        {
            // Fire the property metadata's PropertyChangedCallback
            m_dependencyPropertyChanged(b, new DependencyPropertyChangedEventArgs(oldValue, newValue, Property));
        }
    }

    /// <summary>
	/// Specifies the method to be invoked when the value of the associated BindableProperty has changed
	/// </summary>
	internal delegate void PropertyChangedCallback(BindableObject b, DependencyPropertyChangedEventArgs e);
}
