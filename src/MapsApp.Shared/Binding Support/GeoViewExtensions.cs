using System.Collections.ObjectModel;
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
        ///// <summary>
        ///// Creates a ViewpointControllerProperty property
        ///// </summary>
        //public static readonly DependencyProperty ViewpointControllerProperty =
        //    BindingFramework.DependencyProperty.Register("ViewpointController", typeof(ViewpointController), typeof(GeoView), new PropertyMetadata(null, OnViewpointControllerChanged));

        ///// <summary>
        ///// Invoked when the  ViewpointControllerProperty's value has changed
        ///// </summary>
        //private static void OnViewpointControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.NewValue is ViewpointController)
        //        ((ViewpointController)e.NewValue).SetGeoView(d as GeoView);
        //}

        ///// <summary>
        ///// ViewpointControllerProperty getter method
        ///// </summary>
        //public static ViewpointController GetViewpointController(DependencyObject geoView)
        //{
        //    return (geoView as GeoView)?.GetValue(ViewpointControllerProperty) as ViewpointController;
        //}

        ///// <summary>
        ///// ViewpointControllerProperty setter method
        ///// </summary>
        //public static void SetViewpointController(DependencyObject geoView, ViewpointController ViewpointController)
        //{
        //    (geoView as GeoView)?.SetValue(ViewpointControllerProperty, ViewpointController);
        //}


        public static readonly DependencyProperty ViewpointControllerCollectionProperty =
            BindingFramework.DependencyProperty.Register("ViewpointControllerCollection", typeof(ObservableCollection<ViewpointController>), typeof(GeoView), new PropertyMetadata(null, OnViewpointControllerCollectionChanged));

        private static void OnViewpointControllerCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ObservableCollection<ViewpointController>)
                ((ObservableCollection<ViewpointController>)e.NewValue).CollectionChanged += (s, evt) => 
                {
                    foreach (var item in evt.NewItems)
                    {
                        ((ViewpointController)item).SetGeoView(d as GeoView);
                    }
                };
        }

        public static ObservableCollection<ViewpointController> GetViewpointControllerCollection(DependencyObject geoView)
        {
            var viewpointControllerCollection = (geoView as GeoView).GetValue(ViewpointControllerCollectionProperty) as ObservableCollection<ViewpointController>;

            if (viewpointControllerCollection == null)
            {
                viewpointControllerCollection = new ObservableCollection<ViewpointController>();
                SetViewpointControllerCollection(geoView, viewpointControllerCollection);
            }

            return viewpointControllerCollection;
        }

        public static void SetViewpointControllerCollection(DependencyObject geoView, ObservableCollection<ViewpointController> ViewpointControllerCollection)
        {
            (geoView as GeoView)?.SetValue(ViewpointControllerCollectionProperty, ViewpointControllerCollection);
        }

    }
}
