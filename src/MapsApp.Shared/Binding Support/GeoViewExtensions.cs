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

#if __IOS__ || __ANDROID__ || NETFX_CORE
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using BindingFramework = Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.Helpers;
using Esri.ArcGISRuntime.Xamarin.Forms;
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.Helpers;
#else
using System.Windows;
    using BindingFramework = System.Windows;
    using Esri.ArcGISRuntime.UI.Controls;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Utils
{
    /// <summary>
    /// Extends GeoView with ViewpointControllerCollection property
    /// </summary>
    public static class GeoViewExtensions
    {
        /// <summary>
        /// Creates a ViewpointControllerCollection property
        /// </summary>
        public static readonly DependencyProperty ViewpointControllerCollectionProperty =
            BindingFramework.DependencyProperty.Register("ViewpointControllerCollection", typeof(ViewpointControllerCollection), typeof(GeoView), new PropertyMetadata(null, OnViewpointControllerCollectionChanged));

        /// <summary>
        /// Invoked when the  ViewpointControllerCollection's value has changed
        /// </summary>
        private static void OnViewpointControllerCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ViewpointControllerCollection collection)
            {
                // Set the initial items in the collection
                foreach(ViewpointController item in collection)
                {
                    item.SetGeoView(d as GeoView);
                }

                // Listen for collection changed, and set the geoview for those new items
                collection.CollectionChanged += (sender, args) =>
                {
                    foreach (ViewpointController item in args.NewItems)
                    {
                        item.SetGeoView(d as GeoView);
                    }
                };
            }
        }

        /// <summary>
        /// ViewpointControllerCollection getter method
        /// </summary>
        public static ViewpointControllerCollection GetViewpointControllerCollection(DependencyObject geoView)
        {
            var viewpointControllerCollection = (geoView as GeoView).GetValue(ViewpointControllerCollectionProperty) as ViewpointControllerCollection;

            if (viewpointControllerCollection == null)
            {
                viewpointControllerCollection = new ViewpointControllerCollection();
                SetViewpointControllerCollection(geoView, viewpointControllerCollection);
            }

            return viewpointControllerCollection;
        }

        /// <summary>
        /// ViewpointControllerCollection setter method
        /// </summary>
        public static void SetViewpointControllerCollection(DependencyObject geoView, ViewpointControllerCollection ViewpointControllerCollection)
        {
            (geoView as GeoView)?.SetValue(ViewpointControllerCollectionProperty, ViewpointControllerCollection);
        }

        /// <summary>
        /// Creates a HoldingLocationController property
        /// </summary>
        public static readonly DependencyProperty HoldingLocationControllerProperty =
          BindingFramework.DependencyProperty.Register("HoldingLocationController", typeof(HoldingLocationController), typeof(GeoView), new PropertyMetadata(null, OnHoldingLocationControllerChanged));

        /// <summary>
        /// Invoked when the  HoldingLocation's value has changed
        /// </summary>
        private static void OnHoldingLocationControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is HoldingLocationController)
                ((HoldingLocationController)e.NewValue).SetGeoView(d as GeoView);
        }

        /// <summary>
        /// HoldingLocationController getter method
        /// </summary>
        public static HoldingLocationController GetHoldingLocationController(DependencyObject geoView)
        {
            return (geoView as GeoView)?.GetValue(HoldingLocationControllerProperty) as HoldingLocationController;
        }

        /// <summary>
        /// HoldingLocationController setter method
        /// </summary>
        public static void SetHoldingLocationController(DependencyObject geoView, HoldingLocationController HoldingLocationController)
        {
            (geoView as GeoView)?.SetValue(HoldingLocationControllerProperty, HoldingLocationController);
        }

    }
}
