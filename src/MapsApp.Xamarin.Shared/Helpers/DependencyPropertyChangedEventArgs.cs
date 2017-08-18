using Xamarin.Forms;

namespace Esri.ArcGISRuntime.Internal
{
    /// <summary>
    /// Arguments received by a PropertyChangedCallback delegate when the value of a BindableProperty has changed
    /// </summary>
    internal class DependencyPropertyChangedEventArgs
    {
        /// <summary>
        /// Instantiates a DependencyPropertyChangedEventArgs instance
        /// </summary>
        internal DependencyPropertyChangedEventArgs(object oldValue, object newValue, BindableProperty property)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Property = property;
        }

        /// <summary>
        /// The value of the property before the property change
        /// </summary>
        internal object OldValue { get; private set; }
        /// <summary>
        /// The value of the property after the property change
        /// </summary>
        internal object NewValue { get; private set; }
        /// <summary>
        /// The property that has changed
        /// </summary>
        internal BindableProperty Property { get; private set; }
    }
}
