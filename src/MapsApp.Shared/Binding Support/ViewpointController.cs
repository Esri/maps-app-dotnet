using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using Esri.ArcGISRuntime.Xamarin.Forms;
using Xamarin.Forms;

namespace MapsApp.Utils
{
    public class ViewpointController : BindableObject
    {
        public ViewpointController() { }

        private WeakReference<GeoView> _geoViewWeakRef;

        internal void SetGeoView(GeoView geoView)
        {
            GeoView = geoView;
        }

        private GeoView GeoView
        {
            get
            {
                GeoView geoView = null;
                _geoViewWeakRef?.TryGetTarget(out geoView);
                return geoView;
            }
            set
            {
                if (GeoView != null)
                    GeoView.ViewpointChanged -= GeoView_ViewpointChanged;

                if (_geoViewWeakRef == null)
                    _geoViewWeakRef = new WeakReference<GeoView>(value);
                else
                    _geoViewWeakRef.SetTarget(value);

                // TODO: Change to weak event handler
                value.ViewpointChanged += GeoView_ViewpointChanged;
            }
        }

        bool _isGeoViewViewpointChangedEventFiring = false;
        private void GeoView_ViewpointChanged(object sender, EventArgs e)
        {
            if (!_isOnViewpointChangedExecuting)
            {
                _isGeoViewViewpointChangedEventFiring = true;
                Viewpoint = (sender as GeoView)?.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                _isGeoViewViewpointChangedEventFiring = false;
            }
        }


        public static readonly BindableProperty ViewpointProperty = BindableProperty.Create(
            "Viewpoint", typeof(Viewpoint), typeof(ViewpointController), null, BindingMode.OneWay,
            null, OnViewpointChanged);

        //public static readonly BindableProperty LocationDisplayProperty = BindableProperty.Create("LocationDisplay", typeof(LocationDisplay), typeof(ViewpointController), null, BindingMode.OneWay, null, OnLocationDisplayChanged);

        //private static void OnLocationDisplayChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    if (newValue is Location)
        //    {
        //        var mapView = (bindable as ViewpointController).GeoView as MapView;
        //        mapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
        //        //mapView.LocationDisplay.InitialZoomScale = 1500;
        //    }
        //}

        bool _isOnViewpointChangedExecuting = false;
        private async static void OnViewpointChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is Viewpoint && !(bindable as ViewpointController)._isGeoViewViewpointChangedEventFiring)
            {
                (bindable as ViewpointController)._isOnViewpointChangedExecuting = true;
                await (bindable as ViewpointController)?.GeoView?.SetViewpointAsync((Viewpoint)newValue);
                (bindable as ViewpointController)._isOnViewpointChangedExecuting = false;
            }
        }

        public Viewpoint Viewpoint
        {
            get { return GeoView?.GetCurrentViewpoint(ViewpointType.CenterAndScale); }
            set { SetValue(ViewpointProperty, value); }
        }

        //public LocationDisplay LocationDisplay
        //{
        //    get { return ((MapView)GeoView).LocationDisplay; }
        //    set { SetValue(LocationDisplayProperty, value); }
        //}

    }
}
