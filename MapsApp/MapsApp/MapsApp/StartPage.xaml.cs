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
// <author>Mara Stoica</author>

namespace MapsApp
{
    using Esri.ArcGISRuntime.UI;
    using MapsApp.ViewModels;
    using System;
    using Xamarin.Forms;
    public partial class StartPage : ContentPage
	{
		public StartPage()
		{
			InitializeComponent();
            this.BindingContext = new MapViewModel();
            this.MapView.LocationDisplay.IsEnabled = true;
            this.MapView.LocationDisplay.StatusChanged += LocationDisplay_StatusChanged;
            this.MapView.ViewpointChanged += MapView_ViewpointChanged;
        }

        private void LocationDisplay_StatusChanged(object sender, Esri.ArcGISRuntime.Location.StatusChangedEventArgs e)
        {
            if (e.IsStarted)
            {
                this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
                this.MapView.LocationDisplay.InitialZoomScale = 1500;
            }
        }

        /// <summary>
        /// Event handler for user tapping the Current Location button
        /// </summary>
        /// <param name="sender">Sender control.</param>
        /// <param name="e">Event args</param>
        private void MoveToCurrentLocation(object sender, EventArgs e)
        {
            this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Off;
            this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            this.MapView.LocationDisplay.IsEnabled = true;
        }

        private void MapView_ViewpointChanged(object sender, EventArgs e)
        {
            CompassImage.Rotation = 360 - this.MapView.MapRotation;
        }

        private async void ResetMapRotation(object sender, EventArgs e)
        {
            await this.MapView.SetViewpointRotationAsync(0).ConfigureAwait(false);
        }
    }
}
