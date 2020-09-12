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

using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.ViewModels;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using System.Threading.Tasks;
#if __IOS__
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Platforms = Xamarin.Forms.PlatformConfiguration;
#elif __ANDROID__
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Android;
using Views = Android.Views;
using Android.OS;
using AndroidA = Android;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin
{
    public partial class StartPage : ContentPage
    {
        private const double PinImageScaleFactor = 0.5;

        private BasemapsViewModel _basemapViewModel;
        private UserItemsViewModel _userItemsViewModel;
        private RouteViewModel _routeViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPage"/> class.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        private async void Initialize()
        {
            try
            {
                var routeStyle = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0x00, 0x79, 0xc1), 5);
                RouteRenderer.Symbol = routeStyle;

                PictureMarkerSymbol endMapPin = await CreateMapPin("end.png");
                PictureMarkerSymbol startMapPin = await CreateMapPin("start.png");

                var geocodeViewModel = Resources["GeocodeViewModel"] as GeocodeViewModel;
                var fromGeocodeViewModel = Resources["FromGeocodeViewModel"] as GeocodeViewModel;
                var toGeocodeViewModel = Resources["ToGeocodeViewModel"] as GeocodeViewModel;
                _routeViewModel = Resources["RouteViewModel"] as RouteViewModel;
                _basemapViewModel = Resources["BasemapsViewModel"] as BasemapsViewModel;
                _userItemsViewModel = Resources["UserItemsViewModel"] as UserItemsViewModel;

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

                fromGeocodeViewModel.PropertyChanged += (o, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(GeocodeViewModel.Place):
                            {
                                _routeViewModel.FromPlace = fromGeocodeViewModel.Place;
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
                                _routeViewModel.ToPlace = toGeocodeViewModel.Place;
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

                // start location services - only once MapView.LocationDisplay property is ready
                MapView.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == nameof(MapView.LocationDisplay) && MapView.LocationDisplay != null)
                    {
                        var mapViewModel = Resources["MapViewModel"] as MapViewModel;
                        MapView.LocationDisplay.DataSource = mapViewModel.LocationDataSource;
                        MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;

#if __ANDROID__
                        MainActivity.Instance.AskForLocationPermission(MapView);
#else
                    MapView.LocationDisplay.IsEnabled = true;
#endif
                    }
                };

                // Change map when user selects a new basemap
                _basemapViewModel.PropertyChanged += (s, ea) =>
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
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
        private async Task<PictureMarkerSymbol> CreateMapPin(string imageName)
        {
            try
            {
                Assembly assembly = typeof(StartPage).GetTypeInfo().Assembly;

                string imagePath = null;
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        imagePath = "Esri.ArcGISRuntime.OpenSourceApps.MapsApp.iOS.Images." + imageName;
                        break;
                    case Device.Android:
                        imagePath = "Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Android.Images." + imageName;
                        break;
                    case Device.UWP:
                        imagePath = "Esri.ArcGISRuntime.OpenSourceApps.MapsApp.UWP.Images." + imageName;
                        break;
                }

                using (Stream stream = assembly.GetManifestResourceStream(imagePath))
                {
                    long length = stream.Length;
                    var imageData = new byte[length];
                    stream.Read(imageData, 0, (int)length);

                    if (imageData != null)
                    {
                        var rtImage = new RuntimeImage(imageData);

                        // Image must be loaded to ensure dimensions will be available
                        await rtImage.LoadAsync();

                        // OffsetY adjustment is specific to the pin marker symbol, to make sure it is anchored at the pin point, rather than center
                        return new PictureMarkerSymbol(rtImage) { OffsetY = rtImage.Height * PinImageScaleFactor / 2, Height = rtImage.Height * PinImageScaleFactor, Width = rtImage.Width * PinImageScaleFactor };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Display error message 
                await DisplayAlert("Error", ex.ToString(), "OK");
                return null;
            }
        }

        // Load basemap page, reuse viewmodel so the initial loading happens only once
        private async void LoadBasemapControl(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = false;
            NavigationContainerGrid.IsVisible = true;

            if (!_basemapViewModel.Basemaps.Any())
            {
                try
                {
                    await _basemapViewModel.ReloadBasemaps();
                }
                catch
                {
                    // ignore
                }
            }
            BasemapListView.IsVisible = true;
            UserItemListView.IsVisible = false;
        }

        /// <summary>
        /// Loads the AuthUserItemsPage and changes map when user selects an item
        /// </summary>
        private async void LoadUserItems(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = false;
            NavigationContainerGrid.IsVisible = true;
            BasemapListView.IsVisible = false;
            await _userItemsViewModel.LoadUserItems();
            
            UserItemListView.IsVisible = true;
        }

        /// <summary>
        /// Hides the basemp and map lists when the user selects an item
        /// </summary>
        private void Basemap_ItemTapped(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = false;
            NavigationContainerGrid.IsVisible = false;
            BasemapListView.IsVisible = false;
            UserItemListView.IsVisible = false;
        }

        /// <summary>
        /// Implements the close button behavior for the basemap and map picker views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigatedView_CloseClicked(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = false;
            NavigationContainerGrid.IsVisible = false;
            BasemapListView.IsVisible = false;
            UserItemListView.IsVisible = false;
        }

        /// <summary>
        /// Implement the back-navigation behavior when the back button is pressed on Android
        /// </summary>
        /// <returns>true if Xamarin.Forms should prevent the default back navigation (in this case, navigating to the user's home screen)</returns>
        protected override bool OnBackButtonPressed()
        {
            if (NavigationContainerGrid.IsVisible)
            {
                NavigationContainerGrid.IsVisible = false;
                BasemapListView.IsVisible = false;
                UserItemListView.IsVisible = false;
                return true;
            }
            return base.OnBackButtonPressed();
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

        private void Search_Changed(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = false;
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
            {
                _routeViewModel.FromPlace = await geocodeViewModel.GetReverseGeocodedLocationAsync(MapView.LocationDisplay.Location.Position);
                FromLocationTextBox.Text = _routeViewModel?.FromPlace?.Label ?? string.Empty;
            }
            _routeViewModel.ToPlace = geocodeViewModel.Place;
            ToLocationTextBox.Text = _routeViewModel?.ToPlace?.Label ?? string.Empty;

            // clear the Place to hide the search result
            geocodeViewModel.Place = null;
        }

        private void SearchSuggestionsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
#if __iOS__
            ((ListView)sender).ClearValue(ListView.SelectedItemProperty);
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

#if __IOS__
            On<Platforms.iOS>().SetUseSafeArea(false);
            var safeInsets = On<Platforms.iOS>().SafeAreaInsets();
            this.MapView.ViewInsets = safeInsets;
            this.SafeAreaTop.Height = new GridLength(safeInsets.Top);
            this.SafeAreaBottom.Height = new GridLength(safeInsets.Bottom);
            this.SafeAreaLeft.Width = new GridLength(safeInsets.Left);
            this.SafeAreaRight.Width = new GridLength(safeInsets.Right);
            
            TopShade.Color = System.Drawing.Color.FromArgb(80, 0, 0, 0);

            // Simulates extending the attribution into the unsafe area.
            BottomShade.Color = System.Drawing.Color.FromArgb(166, 255, 255, 255);
#elif __ANDROID__
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                try
                {
                    var gestures = MainActivity.Instance.Window.DecorView.RootWindowInsets.MandatorySystemGestureInsets;
                    var cutout = MainActivity.Instance.Window.DecorView.RootWindowInsets.DisplayCutout;
                    var topPlus = MainActivity.Instance.Window.DecorView.RootWindowInsets.SystemWindowInsetTop;
                    var bottomPlus = MainActivity.Instance.Window.DecorView.RootWindowInsets.SystemWindowInsetBottom;
                    var density = MainActivity.Instance.Resources.DisplayMetrics.Density;

                    var top = Math.Max(gestures?.Top ?? 0, Math.Max(cutout?.SafeInsetTop ?? 0, topPlus)) / density;
                    var left = cutout?.SafeInsetLeft ?? 0 / density;
                    var right = cutout?.SafeInsetRight ?? 0 / density;
                    var bottom = Math.Max(gestures?.Bottom ?? 0, Math.Max(cutout?.SafeInsetBottom ?? 0, bottomPlus)) / density;

                    // Status bar is 24 dips by default; doing this in absence of API to easily get status bar height.
                    top = Math.Max(top, 24);

                    SafeAreaTop.Height = new GridLength(top);
                    SafeAreaBottom.Height = new GridLength(bottom);
                    SafeAreaLeft.Width = new GridLength(left);
                    SafeAreaRight.Width = new GridLength(right);
                    MapView.ViewInsets = new Thickness(left, top, right, bottom);

                    // Set the shade color
                    TopShade.Color = System.Drawing.Color.FromArgb(100, 0, 0, 0);
                    BottomShade.Color = System.Drawing.Color.FromArgb(166, 255, 255, 255);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
#endif
        }
    }
}
