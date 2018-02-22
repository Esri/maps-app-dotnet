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

using Esri.ArcGISRuntime.Mapping;
using System;
#if __ANDROID__ || __IOS__ || NETFX_CORE
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using BindingFramework = Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Helpers;
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Helpers;
using Esri.ArcGISRuntime.Xamarin.Forms;
#else
    using System.Windows;
    using Esri.ArcGISRuntime.UI.Controls;
    using BindingFramework = System.Windows;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Utils
{
    /// <summary>
    /// Provides members for creating a Viewpoint Controller
    /// </summary>
    public class ViewpointController : DependencyObject
    {
        private WeakReference<GeoView> _geoViewWeakRef;
        bool _isGeoViewViewpointChangedEventFiring = false;
        bool _isOnViewpointChangedExecuting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewpointController"/> class.
        /// </summary>
        public ViewpointController() { }

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
                    GeoView.ViewpointChanged -= GeoView_ViewpointChanged;

                if (_geoViewWeakRef == null)
                    _geoViewWeakRef = new WeakReference<GeoView>(value);
                else
                    _geoViewWeakRef.SetTarget(value);

                value.ViewpointChanged += GeoView_ViewpointChanged;
            }
        }

        /// <summary>
        /// Invoked when the  GeoView's ViewPoint value has changed
        /// </summary>
        private void GeoView_ViewpointChanged(object sender, EventArgs e)
        {
            if (!_isOnViewpointChangedExecuting)
            {
                _isGeoViewViewpointChangedEventFiring = true;
                try
                {
                    Viewpoint = (sender as GeoView)?.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                }
                // if unable to get the viewpoint, don't do anything
                catch { }
                _isGeoViewViewpointChangedEventFiring = false;
            }
        }

        /// <summary>
        /// Creates a ViewpointController property
        /// </summary>
        public static readonly DependencyProperty ViewpointProperty = BindingFramework.DependencyProperty.Register(
            "Viewpoint", typeof(Viewpoint), typeof(ViewpointController), new PropertyMetadata(null, OnViewpointChanged));

        /// <summary>
        /// Invoked when the  ViewPoint value has changed
        /// </summary>
        private async static void OnViewpointChanged(DependencyObject bindable, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Viewpoint && !(bindable as ViewpointController)._isGeoViewViewpointChangedEventFiring)
            {
                (bindable as ViewpointController)._isOnViewpointChangedExecuting = true;
                await (bindable as ViewpointController)?.GeoView?.SetViewpointAsync((Viewpoint)e.NewValue);
                (bindable as ViewpointController)._isOnViewpointChangedExecuting = false;
            }
        }

        /// <summary>
        /// Gets or sets the Viewpoint property
        /// </summary>
        public Viewpoint Viewpoint
        {
            get { return GeoView?.GetCurrentViewpoint(ViewpointType.CenterAndScale); }
            set { SetValue(ViewpointProperty, value); }
        }
    }
}
