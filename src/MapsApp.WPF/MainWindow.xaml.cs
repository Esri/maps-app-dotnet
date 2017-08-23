// <copyright file="StartPage.cs" company="Esri">
//      Copyright (c) 2017 Esri. All rights reserved.
//
//      Licensed under the Apache License, Version 2.0 (the "License");
//      you may not use this file except in compliance with the License.
//      You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//      Unless required by applicable law or agreed to in writing, software
//      distributed under the License is distributed on an "AS IS" BASIS,
//      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//      See the License for the specific language governing permissions and
//      limitations under the License.
// </copyright>

namespace MapsApp.WPF
{
    using Esri.ArcGISRuntime.Symbology;
    using Esri.ArcGISRuntime.UI;
    using MapsApp.Shared.ViewModels;
    using System.Windows;
    using System.Windows.Media;

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
            geocodeViewModel.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(GeocodeViewModel.Place):
                        {
                            var place = geocodeViewModel.Place;
                            if (place == null)
                                return;

                            var graphicsOverlay = MapView.GraphicsOverlays["PlacesOverlay"];
                            graphicsOverlay?.Graphics.Clear();

                            // create map pin and add it to the map
                            var mapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp.WPF;component/Images/End72.png")));
                            var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, mapPin);
                            graphicsOverlay?.Graphics.Add(graphic);

                            break;
                        }

                    case nameof(GeocodeViewModel.AreaOfInterest):
                        {
                            // rotate the image around its center
                            RotateTransform rotateTransform = new RotateTransform(360 - this.MapView.MapRotation, CompassImage.Height * 0.5, CompassImage.Width * 0.5);
                            CompassImage.RenderTransform = rotateTransform;
                            break;
                        }

                    case nameof(GeocodeViewModel.IsTopBannerVisible):
                        {
                            // clear map pin when user clears search
                            if (geocodeViewModel.IsTopBannerVisible == false)
                            {
                                var graphicsOverlay = MapView.GraphicsOverlays["PlacesOverlay"];
                                graphicsOverlay?.Graphics.Clear();
                            }
                            break;
                        }

                    case nameof(GeocodeViewModel.ErrorMessage):
                        {
                            // display error message from viewmodel
                            MessageBox.Show(geocodeViewModel.ErrorMessage, "Error", MessageBoxButton.OK);
                            break;
                        }
                }
            };

            // start location services
            var mapViewModel = Resources["MapViewModel"] as MapViewModel;
            MapView.LocationDisplay.DataSource = mapViewModel.LocationDataSource;
            MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            MapView.LocationDisplay.IsEnabled = true;

            // set mapviewmodel
            var basemapViewModel = Resources["BasemapsViewModel"] as BasemapsViewModel;
            basemapViewModel.MapViewModel = mapViewModel;

            // change viewpoint to current location
            mapViewModel.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(mapViewModel.AreaOfInterest):
                        {
                            geocodeViewModel.AreaOfInterest = mapViewModel.AreaOfInterest;
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
            await this.MapView.SetViewpointRotationAsync(0).ConfigureAwait(false);
        }

        /// <summary>
        /// Turns on basemap switcher when button is pushed
        /// </summary>
        private void OpenBasemapSwitcher(object sender, RoutedEventArgs e)
        {
            BasemapSwitcher.Visibility = Visibility.Visible;
            
        }

        private void HideBasemapSwitcher(object sender, RoutedEventArgs e)
        {
            BasemapSwitcher.Visibility = Visibility.Collapsed;
        }
    }
}
