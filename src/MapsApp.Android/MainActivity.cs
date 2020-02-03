using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Android
{
    [Activity (Label = "MapsApp", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance;
        private const int LocationPermissionRequestCode = 99;
        private Esri.ArcGISRuntime.Xamarin.Forms.MapView _lastUsedMapView;

		protected override void OnCreate (Bundle bundle)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
            LoadApplication (new Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.App ());
		}

        public async void AskForLocationPermission(Esri.ArcGISRuntime.Xamarin.Forms.MapView myMapView)
        {
            // Save the mapview for later.
            _lastUsedMapView = myMapView;

            // Only check if permission hasn't been granted yet.
            if (ContextCompat.CheckSelfPermission(this, LocationService) != Permission.Granted)
            {
                // Show the standard permission dialog.
                // Once the user has accepted or denied, OnRequestPermissionsResult is called with the result.
                RequestPermissions(new[] {Manifest.Permission.AccessFineLocation}, LocationPermissionRequestCode);
            }
            else
            {
                try
                {
                    // Explicit DataSource.LoadAsync call is used to surface any errors that may arise.
                    await myMapView.LocationDisplay.DataSource.StartAsync();
                    myMapView.LocationDisplay.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
        
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            // Ignore other location requests.
            if (requestCode != LocationPermissionRequestCode)
            {
                return;
            }

            // If the permissions were granted, enable location.
            if (grantResults.Length == 1 && grantResults[0] == Permission.Granted && _lastUsedMapView != null)
            {
                System.Diagnostics.Debug.WriteLine("User affirmatively gave permission to use location. Enabling location.");
                try
                {
                    // Explicit DataSource.LoadAsync call is used to surface any errors that may arise.
                    await _lastUsedMapView.LocationDisplay.DataSource.StartAsync();
                    _lastUsedMapView.LocationDisplay.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Location permissions not granted.", "Failed to start location display.");
            }

            // Reset the mapview.
            _lastUsedMapView = null;
        }
	}
}

