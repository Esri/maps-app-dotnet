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
            var routingViewModel = Resources["RoutingViewModel"] as RoutingViewModel;
            geocodeViewModel.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(GeocodeViewModel.Place):
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

                            break;
                        }
                    case nameof(GeocodeViewModel.FromPlace):
                        {
                            routingViewModel.FromPlace = geocodeViewModel.FromPlace;
                            break;
                        }
                    case nameof(GeocodeViewModel.ToPlace):
                        {
                            routingViewModel.ToPlace = geocodeViewModel.ToPlace;
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
							var newMap = new Map(basemapViewModel.SelectedBasemap);

                            // Set the viewpoint of the new map to be the same as the old map
                            // Otherwise map is being reset to the world view
                            var currentViewpoint = mapViewModel.AreaOfInterest;
                            newMap.InitialViewpoint = currentViewpoint;

							// Load the new map
                            await newMap.LoadAsync();
                            mapViewModel.Map = newMap;
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
                            var newMap = new Map(userItemsViewModel.SelectedUserItem);

                            // Set the viewpoint of the new map to be the same as the old map
                            // Otherwise map is being reset to the world view
                            var currentViewpoint = mapViewModel.AreaOfInterest;
                            newMap.InitialViewpoint = currentViewpoint;

                            // Load the new map
                            await newMap.LoadAsync();
                            mapViewModel.Map = newMap;
                            break;
                        }
                }
            };

            routingViewModel.PropertyChanged += async (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(RoutingViewModel.Route):
                        {
                            var graphicsOverlay = MapView.GraphicsOverlays["RouteOverlay"];
                            graphicsOverlay?.Graphics?.Clear();

                            if (routingViewModel.FromPlace == null || routingViewModel.ToPlace == null || routingViewModel.Route == null)
                            {
                                return;
                            }

                            // Add route to map
                            var routeGraphic = new Graphic(routingViewModel.Route.Routes.FirstOrDefault()?.RouteGeometry);
                            graphicsOverlay?.Graphics.Add(routeGraphic);

                            // Add start and end locations to the map
                            var fromMapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp;component/Images/Depart.png")));
                            var toMapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp;component/Images/Stop.png")));
                            var fromGraphic = new Graphic(routingViewModel.FromPlace.DisplayLocation, fromMapPin);
                            var toGraphic = new Graphic(routingViewModel.ToPlace.DisplayLocation, toMapPin);
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
        /// Turns on basemap switcher when button is pushed
        /// </summary>
        private void OpenBasemapSwitcher(object sender, RoutedEventArgs e)
        {
            BasemapSwitcher.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Turns off basemap switcher whwn user hits the X 
        /// </summary>
        private void HideBasemapSwitcher(object sender, RoutedEventArgs e)
        {
            BasemapSwitcher.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Closes the settings panel when the user hits the x
        /// </summary>
        private void ExitSettings(object sender, RoutedEventArgs e)
        {
            SettingsPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Opens the settings panel when the user hits the menu button
        /// </summary>
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Turns on user items switcher when button is pushed
        /// </summary>
        private void OpenUserItemSwitcher(object sender, RoutedEventArgs e)
        {
            UserItemsSwitcher.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Turns off user items switcher whwn user hits the X 
        /// </summary>
        private void HideUserItemSwitcher(object sender, RoutedEventArgs e)
        {
            UserItemsSwitcher.Visibility = Visibility.Collapsed;
        }

        private async void ShowRoutingPanel(object sender, RoutedEventArgs e)
        {
            var geocodeViewModel = (Resources["GeocodeViewModel"] as GeocodeViewModel);

            // Set the to and from locations and text boxes
            var matches = await geocodeViewModel.Locator.ReverseGeocodeAsync(MapView.LocationDisplay.Location.Position);
            geocodeViewModel.FromPlace = matches.First();
            geocodeViewModel.ToPlace = geocodeViewModel.Place;

            // clear the Place to hide the search result
            geocodeViewModel.Place = null;
        }

        /// <summary>
        /// Display suggestions only when the text box is in focus
        /// </summary>
        private void FromLocationTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FromLocationSuggestionsList.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide suggestions when text box loses focus
        /// </summary>
        private void FromLocationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FromLocationSuggestionsList.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Display suggestions only when the text box is in focus
        /// </summary>
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchSuggestionsList.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide suggestions when text box loses focus
        /// </summary>
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchSuggestionsList.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Display suggestions only when the text box is in focus
        /// </summary>
        private void ToLocationTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ToLocationSuggestionsList.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide suggestions when text box loses focus
        /// </summary>
        private void ToLocationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ToLocationSuggestionsList.Visibility = Visibility.Collapsed;
        }
    }
}
