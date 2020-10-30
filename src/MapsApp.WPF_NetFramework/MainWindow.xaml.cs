// /*******************************************************************************
//  * Copyright 2018 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  https://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.ViewModels;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Windows;
using System.Linq;
using System;
using Esri.ArcGISRuntime.Geometry;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Scale factore for the images so they're a little less prominent
        private const double PinScaleFactor = 0.5;

        // Hold created symbols so they don't need to be recreated each time they're used
        private PictureMarkerSymbol _mapPinSymbol;
        private PictureMarkerSymbol _originPinSymbol;
        private PictureMarkerSymbol _destinationPinSymbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Initialize();

            
        }

        private async void Initialize()
        {
            // Get references to the viewmodels from the resources dictionary
            var geocodeViewModel = Resources["GeocodeViewModel"] as GeocodeViewModel;
            var fromGeocodeViewModel = Resources["FromGeocodeViewModel"] as GeocodeViewModel;
            var toGeocodeViewModel = Resources["ToGeocodeViewModel"] as GeocodeViewModel;
            var routeViewModel = Resources["RouteViewModel"] as RouteViewModel;

            try
            {
                RouteOverlaySymbol.Color = System.Drawing.Color.FromArgb(255, 0, 121, 193);

                // Load images before 
                await ConfigureImages();

                // Subscribe to viewmodel changes
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
                                    var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, _mapPinSymbol);
                                    graphicsOverlay?.Graphics.Add(graphic);
                                }));

                                break;
                            }
                    }
                };

                fromGeocodeViewModel.PropertyChanged += (o, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(GeocodeViewModel.Place):
                            {
                                routeViewModel.FromPlace = fromGeocodeViewModel.Place;
                                break;
                            }
                    }
                };

                toGeocodeViewModel.PropertyChanged += (o, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(GeocodeViewModel.Place):
                            {
                                routeViewModel.ToPlace = toGeocodeViewModel.Place;
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
                basemapViewModel.PropertyChanged += (s, e) =>
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
                userItemsViewModel.PropertyChanged += (s, e) =>
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

                routeViewModel.PropertyChanged += (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(RouteViewModel.Route):
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    var graphicsOverlay = MapView.GraphicsOverlays["RouteOverlay"];

                                    // clear existing graphics
                                    graphicsOverlay?.Graphics?.Clear();

                                    if (routeViewModel.FromPlace == null || routeViewModel.ToPlace == null ||
                                    routeViewModel.Route == null || graphicsOverlay == null)
                                    {
                                        return;
                                    }

                                    // Add route to map
                                    var routeGraphic = new Graphic(routeViewModel.Route.Routes.FirstOrDefault()?.RouteGeometry);
                                    graphicsOverlay?.Graphics.Add(routeGraphic);

                                    // Add start and end locations to the map
                                    var fromGraphic = new Graphic(routeViewModel.FromPlace.RouteLocation, _originPinSymbol);
                                    var toGraphic = new Graphic(routeViewModel.ToPlace.RouteLocation, _destinationPinSymbol);


                                    graphicsOverlay?.Graphics.Add(fromGraphic);
                                    graphicsOverlay?.Graphics.Add(toGraphic);
                                }));

                                break;
                            }
                    }
                };
            }
            catch (Exception ex)
            {
                geocodeViewModel.ErrorMessage = "Couldn't load images";
            }
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
        private async void ShowRoutingPanel(object sender, RoutedEventArgs e)
        {
            var geocodeViewModel = Resources["GeocodeViewModel"] as GeocodeViewModel;
            var routeViewModel = Resources["RouteViewModel"] as RouteViewModel;

            // Set the to and from locations and text boxes
            // the from location will be the current user location 
            if (MapView.LocationDisplay.IsEnabled)
            {
                routeViewModel.FromPlace = await geocodeViewModel.GetReverseGeocodedLocationAsync(MapView.LocationDisplay.Location.Position);
                FromLocationTextBox.Text = routeViewModel?.FromPlace?.Label ?? string.Empty;
            }
            routeViewModel.ToPlace = geocodeViewModel.Place;
            ToLocationTextBox.Text = routeViewModel?.ToPlace?.Label ?? string.Empty;

            // clear the Place to hide the search result
            geocodeViewModel.Place = null;
        }

        /// <summary>
        /// Loads the symbol images and creates symbols, applying scale factor and offsets
        /// </summary>
        private async Task ConfigureImages()
        {
            var originImage = new RuntimeImage(new Uri("pack://application:,,,/MapsApp;component/Images/Depart.png"));
            var destinationImage = new RuntimeImage(new Uri("pack://application:,,,/MapsApp;component/Images/Stop.png"));

            // Images must be loaded to ensure dimensions will be available
            await Task.WhenAll(originImage.LoadAsync(), destinationImage.LoadAsync());

            // OffsetY adjustment is specific to the pin marker symbol, to make sure it is anchored at the pin point, rather than center.
            _originPinSymbol = new PictureMarkerSymbol(originImage) { OffsetY = originImage.Height * PinScaleFactor / 2, Height = originImage.Height * PinScaleFactor, Width = originImage.Width * PinScaleFactor };
            _mapPinSymbol = new PictureMarkerSymbol(originImage) { OffsetY = originImage.Height * PinScaleFactor / 2, Height = originImage.Height * PinScaleFactor, Width = originImage.Width * PinScaleFactor };
            _destinationPinSymbol = new PictureMarkerSymbol(destinationImage) { OffsetY = destinationImage.Height * PinScaleFactor / 2, Height = destinationImage.Height * PinScaleFactor, Width = destinationImage.Width * PinScaleFactor };
        }
    }
}
