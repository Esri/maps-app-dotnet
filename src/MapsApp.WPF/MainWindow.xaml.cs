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

namespace MapsApp.WPF
{
    using System.Windows;
    using System.Windows.Media;
    using Esri.ArcGISRuntime.Symbology;
    using Esri.ArcGISRuntime.UI;
    using MapsApp.Shared.ViewModels;

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
            await MapView.SetViewpointRotationAsync(0).ConfigureAwait(false);
        }
    }
}
