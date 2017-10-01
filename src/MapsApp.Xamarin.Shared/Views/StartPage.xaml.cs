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

using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels;
using Esri.ArcGISRuntime.Xamarin.Forms;
using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin
{
    public partial class StartPage : ContentPage
    {
        private BasemapsViewModel basemapViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPage"/> class.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();

            PictureMarkerSymbol mapPin = CreateMapPin();

            var geocodeViewModel = Resources["GeocodeViewModel"] as GeocodeViewModel;
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

                            var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, mapPin);
                            graphicsOverlay?.Graphics.Add(graphic);

                            break;
                        }

                    case nameof(GeocodeViewModel.ErrorMessage):
                        {
                            // display error message from viewmodel
                            DisplayAlert("Error", geocodeViewModel.ErrorMessage, "OK");
                            break;
                        }
                }
            };

            // start location services
            var mapViewModel = Resources["MapViewModel"] as MapViewModel;
            MapView.LocationDisplay.DataSource = mapViewModel.LocationDataSource;
            MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            MapView.LocationDisplay.IsEnabled = true;
        }

        /// <summary>
        /// Resets map rotation to North up
        /// </summary>
        private async void ResetMapRotation(object sender, EventArgs e)
        {
            await MapView.SetViewpointRotationAsync(0).ConfigureAwait(false);
        }

        /// <summary>
        /// Create map pin based on platform
        /// </summary>
        private PictureMarkerSymbol CreateMapPin()
        {
            try
            {
                Assembly assembly = typeof(StartPage).GetTypeInfo().Assembly;

                string imagePath = null;
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        imagePath = "Esri.ArcGISRuntime.ExampleApps.MapsApp.iOS.Images.End72.png";
                        break;
                    case Device.Android:
                        imagePath = "Esri.ArcGISRuntime.ExampleApps.MapsApp.Android.Images.End72.png";
                        break;
                    case Device.Windows:
                        imagePath = "Esri.ArcGISRuntime.ExampleApps.MapsApp.UWP.Images.End72.png";
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
            if (basemapViewModel == null)
            {
                basemapViewModel = new BasemapsViewModel();
                // Change map when user selects a new basemap
                basemapViewModel.PropertyChanged += (s, ea) =>
                {
                    switch (ea.PropertyName)
                    {
                        case nameof(BasemapsViewModel.Map):
                            {
                                // Set the viewpoint of the new map to be the same as the old map
                                // Otherwise map is being reset to the world view
                                var mapViewModel = (Resources["MapViewModel"] as MapViewModel);
                                var currentViewpoint = mapViewModel.AreaOfInterest;
                                basemapViewModel.Map.InitialViewpoint = currentViewpoint;
                                mapViewModel.Map = basemapViewModel.Map;
                                break;
                            }
                    }
                };
            }
            await Navigation.PushAsync(new BasemapPage { BindingContext = basemapViewModel });
        }

        /// <summary>
        /// Opens the settings panel when it is closed
        /// Closes the settings panel when it is open
        /// </summary>
        private void OpenCloseSettings(object sender, EventArgs e)
        {
            SettingsPanel.IsVisible = (SettingsPanel.IsVisible == true) ? false : true;
        }
    }
}
