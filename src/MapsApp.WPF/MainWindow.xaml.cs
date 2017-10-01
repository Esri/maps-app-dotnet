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
                            var mapPin = new PictureMarkerSymbol(new RuntimeImage(new System.Uri("pack://application:,,,/MapsApp;component/Images/End72.png")));
                            var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, mapPin);
                            graphicsOverlay?.Graphics.Add(graphic);

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

            // Change map when user selects a new basemap
            var basemapViewModel = Resources["BasemapsViewModel"] as BasemapsViewModel;
            basemapViewModel.PropertyChanged += async (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(BasemapsViewModel.SelectedBasemap):
                        {
                            var newMap = new Map(basemapViewModel.SelectedBasemap);
                            await newMap.LoadAsync();
                            mapViewModel.Map = newMap;
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
    }
}
