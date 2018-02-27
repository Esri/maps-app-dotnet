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

using Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Xamarin.Forms;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin
{
    public partial class StartPage : ContentPage
    {
        private BasemapsViewModel _basemapViewModel;
        private UserItemsViewModel _userItemsViewModel;
        private RouteViewModel _routeViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPage"/> class.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();
            InitializeBasemapSwitcher();

            PictureMarkerSymbol endMapPin = CreateMapPin("end.png");
            PictureMarkerSymbol startMapPin = CreateMapPin("start.png");

            var geocodeViewModel = Resources["GeocodeViewModel"] as GeocodeViewModel;
            _routeViewModel = Resources["RouteViewModel"] as RouteViewModel;

            geocodeViewModel.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(GeocodeViewModel.Place):
                        {
                            var graphicsOverlay = MapView.GraphicsOverlays["PlacesOverlay"];
                            graphicsOverlay?.Graphics.Clear();

                            GeocodeResult place = geocodeViewModel.Place;
                            if (place == null)
                            {
                                return;
                            }

                            var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, endMapPin);
                            graphicsOverlay?.Graphics.Add(graphic);

                            break;
                        }
                }
            };

            _routeViewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(RouteViewModel.Route):
                        {
                            var graphicsOverlay = MapView.GraphicsOverlays["RouteOverlay"];

                            // clear existing graphics
                            graphicsOverlay?.Graphics?.Clear();

                            if (_routeViewModel.FromPlace == null || _routeViewModel.ToPlace == null ||
                            _routeViewModel.Route == null || graphicsOverlay == null)
                            {
                                return;
                            }

                            // Add route to map
                            var routeGraphic = new Graphic(_routeViewModel.Route.Routes.FirstOrDefault()?.RouteGeometry);
                            graphicsOverlay?.Graphics.Add(routeGraphic);

                            // Add start and end locations to the map
                            var fromGraphic = new Graphic(_routeViewModel.FromPlace.RouteLocation, startMapPin);
                            var toGraphic = new Graphic(_routeViewModel.ToPlace.RouteLocation, endMapPin);
                            graphicsOverlay?.Graphics.Add(fromGraphic);
                            graphicsOverlay?.Graphics.Add(toGraphic);

                            break;
                        }
                }
            };

            // start location services
            var mapViewModel = Resources["MapViewModel"] as MapViewModel;
            MapView.LocationDisplay.DataSource = mapViewModel.LocationDataSource;
            MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            MapView.LocationDisplay.IsEnabled = true;

#if __IOS__
            // This is necessary because on iOS the SearchBar doesn't get unfocused automatically when a geocode result is selected
            SearchSuggestionsList.ItemSelected += (s, e) =>
            {
                AddressSearchBar.Unfocus();
            };

            FromLocationSuggestionsList.ItemSelected += (s, e) =>
            {
                FromLocationTextBox.Unfocus();
            };

            ToLocationSuggestionsList.ItemSelected += (s, e) =>
            {
                ToLocationTextBox.Unfocus();
            };
#endif
        }

        /// <summary>
        /// Initialize basemaps and basemap switching functionality
        /// </summary>
        private void InitializeBasemapSwitcher()
        {
            if (_basemapViewModel == null)
            {
                _basemapViewModel = new BasemapsViewModel();
                // Change map when user selects a new basemap
                _basemapViewModel.PropertyChanged += async (s, ea) =>
                {
                    switch (ea.PropertyName)
                    {
                        case nameof(BasemapsViewModel.SelectedBasemap):
                            {
                                // Set the viewpoint of the new map to be the same as the old map
                                // Otherwise map is being reset to the world view
                                var mapViewModel = Resources["MapViewModel"] as MapViewModel;

                                try
                                {
                                    var currentViewpoint = MapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                                    mapViewModel.Map.Basemap = new Basemap(_basemapViewModel.SelectedBasemap);
                                }
                                catch (Exception ex)
                                {
                                    mapViewModel.ErrorMessage = "Unable to change basemaps";
                                    mapViewModel.StackTrace = ex.ToString();
                                }

                                break;
                            }
                    }
                };
            }
        }

        /// <summary>
        /// Resets map rotation to North up
        /// </summary>
        private async void ResetMapRotation(object sender, EventArgs e)
        {
            await MapView.SetViewpointRotationAsync(0);
        }

        /// <summary>
        /// Create map pin based on platform
        /// </summary>
        private PictureMarkerSymbol CreateMapPin(string imageName)
        {
            try
            {
                Assembly assembly = typeof(StartPage).GetTypeInfo().Assembly;

                string imagePath = null;
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        imagePath = "Esri.ArcGISRuntime.ExampleApps.MapsApp.iOS.Images." + imageName;
                        break;
                    case Device.Android:
                        imagePath = "Esri.ArcGISRuntime.ExampleApps.MapsApp.Android.Images." + imageName;
                        break;
                    case Device.UWP:
                        imagePath = "Esri.ArcGISRuntime.ExampleApps.MapsApp.UWP.Images." + imageName;
                        break;
                }

                using (Stream stream = assembly.GetManifestResourceStream(imagePath))
                {
                    long length = stream.Length;
                    var imageData = new byte[length];
                    stream.Read(imageData, 0, (int)length);

                    if (imageData != null)
                    {
                        return new PictureMarkerSymbol(new RuntimeImage(imageData)); 
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Display error message 
                DisplayAlert("Error", ex.ToString(), "OK");
                return null;
            }
        }

        // Load basemap page, reuse viewmodel so the initial loading happens only once
        private async void LoadBasemapControl(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = false;
            await Navigation.PushAsync(new BasemapPage { BindingContext = _basemapViewModel });
        }


        // Load directions page
        private async void LoadDirectionsControl(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TurnByTurnDirections { BindingContext = _routeViewModel });
        }

        /// <summary>
        /// Opens the settings panel when it is closed
        /// Closes the settings panel when it is open
        /// </summary>
        private void OpenCloseSettings(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = (!SettingsPanel.IsVisible);
        }

        /// <summary>
        /// Loads the AuthUserItemsPage and changes map when user selects an item
        /// </summary>
        private async void LoadUserItems(object sender, EventArgs e)
        {
            _userItemsViewModel = new UserItemsViewModel();
            await _userItemsViewModel.LoadUserItems();

                // Change map when user selects a new user item
                _userItemsViewModel.PropertyChanged += async (s, ea) =>
                {
                    switch (ea.PropertyName)
                    {
                        case nameof(UserItemsViewModel.SelectedUserItem):
                            {
                                // Set the viewpoint of the new map to be the same as the old map
                                // Otherwise map is being reset to the world view
                                var mapViewModel = Resources["MapViewModel"] as MapViewModel;
                                var currentViewpoint = MapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                                try
                                {
                                    var newMap = new Map(_userItemsViewModel.SelectedUserItem)
                                    {
                                        InitialViewpoint = currentViewpoint
                                    };

                                    //Load new map
                                    await newMap.LoadAsync();
                                    mapViewModel.Map = newMap;
                                }
                                catch (Exception ex)
                                {
                                    mapViewModel.ErrorMessage = "Unable to change maps";
                                    mapViewModel.StackTrace = ex.ToString();
                                }

                                break;
                            }
                    }
                };

            SettingsPanel.IsVisible = false;

            // Load the AuthUserItemsPage
            await Navigation.PushAsync(new AuthUserItemsPage { BindingContext = _userItemsViewModel });
        }

        /// <summary>
        /// Display the routing panel when user taps the Route button
        /// </summary>
        private async void ShowRoutingPanel(object sender, EventArgs e)
        {
            var geocodeViewModel = (Resources["GeocodeViewModel"] as GeocodeViewModel);

            // Set the to and from locations and text boxes
            // the from location will be the current user location 
            if (MapView.LocationDisplay.IsEnabled)
                _routeViewModel.FromPlace = await geocodeViewModel.GetReverseGeocodedLocationAsync(MapView.LocationDisplay.Location.Position);
            _routeViewModel.ToPlace = geocodeViewModel.Place;

            // clear the Place to hide the search result
            geocodeViewModel.Place = null;
        }

        private void SearchSuggestionsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
#if __iOS__
            ((ListView)sender).ClearValue(ListView.SelectedItemProperty);
#endif
        }
    }
}
