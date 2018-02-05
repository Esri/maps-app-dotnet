// /*******************************************************************************
//  * Copyright 2017 Esri
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
using Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Windows;
using System.Linq;
using System;
using Esri.ArcGISRuntime.Geometry;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var geocodeViewModel = Resources["GeocodeViewModel"] as GeocodeViewModel;
            var routeViewModel = Resources["RouteViewModel"] as RouteViewModel;
            geocodeViewModel.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(GeocodeViewModel.Place):
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                var graphicsOverlay = MapView.GraphicsOverlays["PlacesOverlay"];
                                graphicsOverlay?.Graphics.Clear();

                                var place = geocodeViewModel.Place;

                                if (place == null)
                                {
                                    return;
                                }

                                // create map pin and add it to the map
                                var mapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp;component/Images/Stop.png")));
                                var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, mapPin);
                                graphicsOverlay?.Graphics.Add(graphic);
                            }));

                            break;
                        }
                    case nameof(GeocodeViewModel.FromPlace):
                        {
                            routeViewModel.FromPlace = geocodeViewModel.FromPlace.RouteLocation;
                            break;
                        }
                    case nameof(GeocodeViewModel.ToPlace):
                        {
                            routeViewModel.ToPlace = geocodeViewModel.ToPlace.RouteLocation;
                            break;
                        }
                }
            };

            // start location services
            var mapViewModel = Resources["MapViewModel"] as MapViewModel;
            MapView.LocationDisplay.DataSource = mapViewModel.LocationDataSource;
            MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            MapView.LocationDisplay.IsEnabled = true;

            // Change map when user selects a new basemap
            var basemapViewModel = Resources["BasemapsViewModel"] as BasemapsViewModel;
            basemapViewModel.PropertyChanged += async (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(BasemapsViewModel.SelectedBasemap):
                        {

                            // Set the viewpoint of the new map to be the same as the old map
                            // Otherwise map is being reset to the world view
                            var currentViewpoint = MapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry);

                            try
                            {
                                mapViewModel.Map.Basemap = new Basemap(basemapViewModel.SelectedBasemap);
                            }
                            catch (Exception ex)
                            {
                                mapViewModel.ErrorMessage = "Unable to change basemaps";
                                mapViewModel.StackTrace = ex?.ToString();
                            }

                            break;
                        }
                }
            };

            // Change user item when user selects a new one
            var userItemsViewModel = Resources["UserItemsViewModel"] as UserItemsViewModel;
            userItemsViewModel.PropertyChanged += async (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(UserItemsViewModel.SelectedUserItem):
                        {
                            // Set the viewpoint of the new map to be the same as the old map
                            // Otherwise map is being reset to the default extent of the web map
                            try
                            {
                                var currentViewpoint = MapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                                mapViewModel.Map = new Map(userItemsViewModel.SelectedUserItem);
                            }
                            catch (Exception ex)
                            {
                                mapViewModel.ErrorMessage = "Unable to load user map";
                                mapViewModel.StackTrace = ex?.ToString();
                            }
                            break;
                        }
                }
            };

            routeViewModel.PropertyChanged += async (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(RouteViewModel.Route):
                        {
                            var graphicsOverlay = MapView.GraphicsOverlays["RouteOverlay"];

                            if (routeViewModel.FromPlace == null || routeViewModel.ToPlace == null || 
                            routeViewModel.Route == null || graphicsOverlay == null)
                            {
                                return;
                            }

                            // clear existing graphics
                            graphicsOverlay?.Graphics?.Clear();

                            // Add route to map
                            var routeGraphic = new Graphic(routeViewModel.Route.Routes.FirstOrDefault()?.RouteGeometry);
                            graphicsOverlay?.Graphics.Add(routeGraphic);

                            // Add start and end locations to the map
                            var fromMapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp;component/Images/Depart.png")));
                            var toMapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp;component/Images/Stop.png")));
                            var fromGraphic = new Graphic(routeViewModel.FromPlace, fromMapPin);
                            var toGraphic = new Graphic(routeViewModel.ToPlace, toMapPin);

                            graphicsOverlay?.Graphics.Add(fromGraphic);
                            graphicsOverlay?.Graphics.Add(toGraphic);

                            break;
                        }
                }
            };
        }

        /// <summary>
        /// Resets map rotation to North up
        /// </summary>
        private async void ResetMapRotation(object sender, RoutedEventArgs e)
        {
            await MapView.SetViewpointRotationAsync(0);
        }

        /// <summary>
        /// Display the routing panel when user taps the Route button
        /// </summary>
        private void ShowRoutingPanel(object sender, RoutedEventArgs e)
        {
            var geocodeViewModel = (Resources["GeocodeViewModel"] as GeocodeViewModel);

            // Set the to and from locations and text boxes
            // the from location will be the current user location 
            geocodeViewModel.UserCurrentLocation = MapView.LocationDisplay.Location.Position;
            geocodeViewModel.ToPlace = geocodeViewModel.Place;

            // clear the Place to hide the search result
            geocodeViewModel.Place = null;
        }
    }
}
