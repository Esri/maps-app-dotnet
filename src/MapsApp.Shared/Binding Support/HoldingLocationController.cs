// /*******************************************************************************
//  * Copyright 2018 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using System;
using Esri.ArcGISRuntime.Geometry;

#if __ANDROID__ || __IOS__ || NETFX_CORE
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using BindingFramework = Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.Helpers;
using Esri.ArcGISRuntime.Xamarin.Forms;
#else
    using System.Windows;
    using Esri.ArcGISRuntime.UI.Controls;
    using BindingFramework = System.Windows;
#endif

/// <summary>
/// Provides members for creating a HoldingLocation Controller
/// </summary>
namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Utils
{
    public class HoldingLocationController : DependencyObject
    {
        private WeakReference<GeoView> _geoViewWeakRef;
        bool _isGeoViewHoldingEventFiring = false;
        bool _isOnHoldingLocationChangedExecuting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoldingLocationController"/> class.
        /// </summary>
        public HoldingLocationController() { }

        /// <summary>
        /// GeoView setter
        /// </summary>
        internal void SetGeoView(GeoView geoView)
        {
            GeoView = geoView;
        }

        /// <summary>
        /// Gets or sets the GeoView
        /// </summary>
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
                    GeoView.GeoViewHolding -= GeoView_GeoViewHolding;

                if (_geoViewWeakRef == null)
                    _geoViewWeakRef = new WeakReference<GeoView>(value);
                else
                    _geoViewWeakRef.SetTarget(value);

                value.GeoViewHolding += GeoView_GeoViewHolding;
            }
        }

        /// <summary>
        /// Invoked when the GeoViewHolding event fires
        /// </summary>
        private void GeoView_GeoViewHolding(object sender, GeoViewInputEventArgs e)
        {
            if (!_isOnHoldingLocationChangedExecuting)
            {
                _isGeoViewHoldingEventFiring = true;
                // get the Location the user is holding from the event args
                HoldingLocation = e.Location;
                _isGeoViewHoldingEventFiring = false;
            }
        }

        /// <summary>
        /// Creates a HoldingLocation property
        /// </summary>
        public static readonly DependencyProperty HoldingLocationProperty = BindingFramework.DependencyProperty.Register(
            "HoldingLocation", typeof(MapPoint), typeof(HoldingLocationController),null);

        /// <summary>
        /// Gets or sets the HoldingLocation property
        /// </summary>
        public MapPoint HoldingLocation
        {
            get { return (MapPoint)GetValue(HoldingLocationProperty); }
            set { SetValue(HoldingLocationProperty, value); }
        }
    }
}
