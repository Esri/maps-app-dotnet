using Esri.ArcGISRuntime.Xamarin.Forms;
using Xamarin.Forms;

namespace MapsApp.Utils
{
    public static class GeoViewExtensions
    {
        public static readonly BindableProperty ViewpointControllerProperty =
            BindableProperty.CreateAttached("ViewpointController", typeof(ViewpointController), typeof(GeoView), null,
                BindingMode.TwoWay, null, OnViewpointControllerChanged);


        private static void OnViewpointControllerChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is ViewpointController)
                ((ViewpointController)newValue).SetGeoView(bindable as GeoView);
        }

        public static ViewpointController GetViewpointController(BindableObject geoView)
        {
            return (geoView as GeoView)?.GetValue(ViewpointControllerProperty) as ViewpointController;
        }

        public static void SetViewpointController(BindableObject geoView, ViewpointController ViewpointController)
        {
            (geoView as GeoView)?.SetValue(ViewpointControllerProperty, ViewpointController);
        }
    }
}
