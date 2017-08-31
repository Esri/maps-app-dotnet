#if __IOS__ || __ANDROID__ || NETFX_CORE
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using BindingFramework = Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Helpers;
using Esri.ArcGISRuntime.Xamarin.Forms;
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Helpers;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BindingFramework = System.Windows;
    using Esri.ArcGISRuntime.UI.Controls;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Utils
{
    /// <summary>
    /// Extends GeoView with ViewPoint Controller property
    /// </summary>
    public static class GeoViewExtensions
    {
        /// <summary>
        /// Creates a ViewpointControllerProperty property
        /// </summary>
        public static readonly DependencyProperty ViewpointControllerProperty =
            BindingFramework.DependencyProperty.Register("ViewpointController", typeof(ViewpointController), typeof(GeoView), new PropertyMetadata(null, OnViewpointControllerChanged));

        /// <summary>
        /// Invoked when the  ViewpointControllerProperty's value has changed
        /// </summary>
        private static void OnViewpointControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ViewpointController)
                ((ViewpointController)e.NewValue).SetGeoView(d as GeoView);
        }

        /// <summary>
        /// ViewpointControllerProperty getter method
        /// </summary>
        public static ViewpointController GetViewpointController(DependencyObject geoView)
        {
            return (geoView as GeoView)?.GetValue(ViewpointControllerProperty) as ViewpointController;
        }

        /// <summary>
        /// ViewpointControllerProperty setter method
        /// </summary>
        public static void SetViewpointController(DependencyObject geoView, ViewpointController ViewpointController)
        {
            (geoView as GeoView)?.SetValue(ViewpointControllerProperty, ViewpointController);
        }
    }
}
