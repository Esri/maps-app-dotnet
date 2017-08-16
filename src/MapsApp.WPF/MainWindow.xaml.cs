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
    using System.Windows;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.MapView.LocationDisplay.IsEnabled = true;
            //this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            //this.MapView.LocationDisplay.InitialZoomScale = 1500;
            //this.MapView.ViewpointChanged += MapView_ViewpointChanged;
        }

        //private void MapView_ViewpointChanged(object sender, EventArgs e)
        //{
        //    CompassImage.RenderTransform = new RotateTransform(360 - this.MapView.MapRotation);
        //}


        ///// <summary>
        ///// Event handler for user tapping the Current Location button
        ///// </summary>
        ///// <param name="sender">Sender control.</param>
        ///// <param name="e">Event args</param>
        //private void MoveToCurrentLocation(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Off;
        //    this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
        //    this.MapView.LocationDisplay.IsEnabled = true;
        //}

        //private async void ResetMapRotation(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    await this.MapView.SetViewpointRotationAsync(0).ConfigureAwait(false);
        //}
    }
}
