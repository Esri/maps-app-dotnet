using Esri.ArcGISRuntime.UI;
using MapsApp.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MapsApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MapViewModel();
            this.MapView.LocationDisplay.IsEnabled = true;
            this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            this.MapView.LocationDisplay.InitialZoomScale = 1500;
            this.MapView.ViewpointChanged += MapView_ViewpointChanged;
        }

        private void MapView_ViewpointChanged(object sender, EventArgs e)
        {
            CompassImage.RenderTransform = new RotateTransform(360 - this.MapView.MapRotation);
        }


        /// <summary>
        /// Event handler for user tapping the Current Location button
        /// </summary>
        /// <param name="sender">Sender control.</param>
        /// <param name="e">Event args</param>
        private void MoveToCurrentLocation(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Off;
            this.MapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
            this.MapView.LocationDisplay.IsEnabled = true;
        }

        private async void ResetMapRotation(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await this.MapView.SetViewpointRotationAsync(0).ConfigureAwait(false);
        }
    }
}
